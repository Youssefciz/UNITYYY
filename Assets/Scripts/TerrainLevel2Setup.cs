using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using sc.terrain.proceduralpainter;

/// <summary>
/// Runtime script that sets up the Level2_Terrain scene when it loads.
/// Configures terrain, adds player/enemy if missing, sets up UI, and bakes NavMesh.
/// </summary>
public class TerrainLevel2Setup : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== TerrainLevel2Setup: Starting setup ===");
        
        // Setup will happen automatically:
        // 1. Terrain should already be created manually or via editor
        // 2. LivesUISetup will handle UI setup automatically
        // 3. We just need to ensure Player and Enemy exist, and NavMesh is baked
        
        SetupCamera();
        SetupPlayer();
        SetupNavMesh(); // Bake NavMesh BEFORE positioning enemy
        SetupEnemy(); // Position enemy AFTER NavMesh is ready
        SetupEnvironment();
        SetupTerrainPainter(); // Setup Procedural Terrain Painter
        
        Debug.Log("=== TerrainLevel2Setup: Setup complete ===");
    }
    
    void SetupCamera()
    {
        // Find existing camera with MainCamera tag
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
        
        if (mainCamera == null)
        {
            // Find any camera in the scene
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            // Create a new camera if none exists
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();
            
            Debug.Log("TerrainLevel2Setup: Created new Main Camera");
        }
        else
        {
            // Ensure the camera GameObject has the MainCamera tag
            mainCamera.gameObject.tag = "MainCamera";
        }
        
        // Ensure camera is enabled
        mainCamera.enabled = true;
        mainCamera.gameObject.SetActive(true);
        
        // Position camera to view the terrain and player
        Terrain terrain = FindFirstObjectByType<Terrain>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (terrain != null)
        {
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            float height = terrain.SampleHeight(terrainCenter);
            
            if (player != null)
            {
                // Position camera behind and above player
                Vector3 playerPos = player.transform.position;
                mainCamera.transform.position = playerPos + new Vector3(0, 10, -10);
                mainCamera.transform.LookAt(playerPos);
            }
            else
            {
                // Position camera to view terrain center
                mainCamera.transform.position = terrainCenter + new Vector3(0, 20, -20);
                mainCamera.transform.LookAt(terrainCenter);
            }
        }
        else
        {
            // Default camera position
            mainCamera.transform.position = new Vector3(0, 10, -10);
            mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
        }
        
        Debug.Log($"TerrainLevel2Setup: Camera set up at {mainCamera.transform.position}");
    }
    
    void SetupPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("TerrainLevel2Setup: Player not found! Please add Player to the scene.");
            return;
        }
        
        // Position player on terrain
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain != null)
        {
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            float height = terrain.SampleHeight(terrainCenter);
            player.transform.position = new Vector3(terrainCenter.x, height + 2f, terrainCenter.z);
            Debug.Log($"TerrainLevel2Setup: Positioned Player at {player.transform.position}");
        }
        
        // Ensure PlayerLives component exists and is configured
        PlayerLives playerLives = player.GetComponent<PlayerLives>();
        if (playerLives == null)
        {
            playerLives = player.AddComponent<PlayerLives>();
            Debug.Log("TerrainLevel2Setup: Added PlayerLives component to Player");
        }
    }
    
    void SetupEnemy()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null)
        {
            Debug.LogWarning("TerrainLevel2Setup: Enemy not found! Creating basic enemy...");
            
            // Create a basic enemy
            enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy1";
            enemy.tag = "Enemy";
            enemy.transform.localScale = new Vector3(1, 1.5f, 1);
            enemy.GetComponent<Renderer>().material.color = Color.red;
            
            // Add Rigidbody - MUST be kinematic for NavMeshAgent to work properly
            Rigidbody rb = enemy.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Critical: NavMeshAgent needs kinematic Rigidbody
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Add NavMeshAgent BEFORE positioning
            NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
            agent.radius = 0.5f;
            agent.height = 2f;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 360f;
            agent.stoppingDistance = 1.5f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            
            // Add EnemyMovement script
            enemy.AddComponent<EnemyMovement>();
            
            Debug.Log("TerrainLevel2Setup: Created new Enemy GameObject");
        }
        else
        {
            // Ensure existing enemy has proper Rigidbody setup
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Ensure kinematic for NavMeshAgent
            }
            else
            {
                rb = enemy.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            
            // Ensure NavMeshAgent exists and is configured
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = enemy.AddComponent<NavMeshAgent>();
                agent.radius = 0.5f;
                agent.height = 2f;
                agent.speed = 3.5f;
            }
            agent.acceleration = 8f;
            agent.angularSpeed = 360f;
            agent.stoppingDistance = 1.5f;
        }
        
        // Position enemy on terrain and warp to NavMesh
        Terrain terrain = FindFirstObjectByType<Terrain>();
        NavMeshAgent agentToWarp = enemy.GetComponent<NavMeshAgent>();
        
        if (terrain != null)
        {
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            Vector3 enemyPos = terrainCenter + new Vector3(20, 0, 20);
            float height = terrain.SampleHeight(enemyPos);
            Vector3 targetPosition = new Vector3(enemyPos.x, height + 1f, enemyPos.z);
            
            // Warp to NavMesh if agent exists
            if (agentToWarp != null)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPosition, out hit, 10f, NavMesh.AllAreas))
                {
                    // Disable agent, set position, re-enable
                    bool wasEnabled = agentToWarp.enabled;
                    agentToWarp.enabled = false;
                    enemy.transform.position = hit.position;
                    agentToWarp.enabled = wasEnabled;
                    Debug.Log($"TerrainLevel2Setup: Positioned Enemy on NavMesh at {hit.position}");
                }
                else
                {
                    // Fallback: just set position
                    enemy.transform.position = targetPosition;
                    Debug.LogWarning($"TerrainLevel2Setup: Could not find NavMesh near {targetPosition}, positioned enemy anyway");
                }
            }
            else
            {
                enemy.transform.position = targetPosition;
                Debug.Log($"TerrainLevel2Setup: Positioned Enemy at {targetPosition} (no NavMeshAgent)");
            }
        }
        else
        {
            enemy.transform.position = new Vector3(20, 5, 20);
        }
        
        // Start coroutine to verify NavMesh connection after a frame
        StartCoroutine(VerifyEnemyNavMesh(enemy));
    }
    
    private System.Collections.IEnumerator VerifyEnemyNavMesh(GameObject enemy)
    {
        yield return null; // Wait one frame for NavMesh to be ready
        
        NavMeshAgent agent = enemy?.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                Debug.Log($"TerrainLevel2Setup: Enemy {enemy.name} is on NavMesh and ready to move!");
            }
            else
            {
                Debug.LogError($"TerrainLevel2Setup: Enemy {enemy.name} is NOT on NavMesh! Attempting to warp...");
                // Try to warp again
                NavMeshHit hit;
                if (NavMesh.SamplePosition(enemy.transform.position, out hit, 50f, NavMesh.AllAreas))
                {
                    bool wasEnabled = agent.enabled;
                    agent.enabled = false;
                    enemy.transform.position = hit.position;
                    agent.enabled = wasEnabled;
                    Debug.Log($"TerrainLevel2Setup: Warped enemy to NavMesh at {hit.position}");
                }
            }
        }
    }
    
    void SetupNavMesh()
    {
        // Find or create NavMeshSurface
        NavMeshSurface navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            GameObject navMeshObj = new GameObject("NavMeshSurface");
            navMeshSurface = navMeshObj.AddComponent<NavMeshSurface>();
            Debug.Log("TerrainLevel2Setup: Created new NavMeshSurface");
        }
        
        // Configure NavMeshSurface for terrain
        navMeshSurface.collectObjects = CollectObjects.All;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        
        // Configure agent settings for terrain navigation
        navMeshSurface.agentTypeID = 0; // Humanoid agent type
        navMeshSurface.defaultArea = 0; // Walkable area
        
        // Build NavMesh
        navMeshSurface.BuildNavMesh();
        
        Debug.Log($"TerrainLevel2Setup: NavMesh baked for terrain. Surface bounds: {navMeshSurface.size}");
        
        // Verify NavMesh was created
        if (navMeshSurface.navMeshData != null)
        {
            Debug.Log("TerrainLevel2Setup: NavMesh data created successfully");
        }
        else
        {
            Debug.LogWarning("TerrainLevel2Setup: NavMesh data is null - NavMesh may not have been baked correctly");
        }
    }
    
    void SetupEnvironment()
    {
        // Add some environment objects (rocks, crates, etc.)
        // This is optional - you can add more objects manually in the editor
        
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain == null) return;
        
        // Add a few simple environment objects
        for (int i = 0; i < 5; i++)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Rock" + i;
            rock.transform.localScale = Vector3.one * Random.Range(0.5f, 2f);
            rock.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
            
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            Vector3 rockPos = terrainCenter + new Vector3(
                Random.Range(-50f, 50f),
                0,
                Random.Range(-50f, 50f)
            );
            float height = terrain.SampleHeight(rockPos);
            rock.transform.position = new Vector3(rockPos.x, height + rock.transform.localScale.y / 2f, rockPos.z);
        }
        
        Debug.Log("TerrainLevel2Setup: Added environment objects");
    }
    
    void SetupTerrainPainter()
    {
        // Find terrain in the scene
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("TerrainLevel2Setup: No terrain found for TerrainPainter setup");
            return;
        }
        
        // Check if TerrainPainter already exists
        TerrainPainter terrainPainter = FindFirstObjectByType<TerrainPainter>();
        
        if (terrainPainter == null)
        {
            // Create a new GameObject for the TerrainPainter component
            GameObject painterObj = new GameObject("TerrainPainter");
            terrainPainter = painterObj.AddComponent<TerrainPainter>();
            Debug.Log("TerrainLevel2Setup: Created TerrainPainter GameObject");
        }
        
        // Assign the terrain to the TerrainPainter
        if (terrainPainter.terrains == null || terrainPainter.terrains.Length == 0 || !System.Array.Exists(terrainPainter.terrains, t => t == terrain))
        {
            // Create array with the terrain
            terrainPainter.terrains = new Terrain[] { terrain };
            terrainPainter.RecalculateBounds();
            Debug.Log($"TerrainLevel2Setup: Assigned terrain '{terrain.name}' to TerrainPainter");
        }
        
        // Note: Layer settings need to be configured manually in the Inspector
        // The user should:
        // 1. Select the TerrainPainter GameObject
        // 2. In Inspector, click "Add Layer" or drag terrain layers
        // 3. Configure modifiers (Height, Slope, Noise, etc.)
        // 4. Click "Repaint All" to apply
        
        Debug.Log("TerrainLevel2Setup: TerrainPainter is ready. Configure layers in Inspector and click 'Repaint All' to paint the terrain.");
    }
}
