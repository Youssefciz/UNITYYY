using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script to set up Level2_Terrain scene with terrain, player, UI, enemy, and NavMesh.
/// </summary>
public class SetupTerrainLevel2 : EditorWindow
{
    [MenuItem("Tools/Setup Level2 Terrain Scene")]
    public static void SetupLevel2Terrain()
    {
        // Load the Level2_Terrain scene
        string scenePath = "Assets/Scenes/Level2_Terrain.unity/Level2_Terrain.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        if (!scene.IsValid())
        {
            Debug.LogError("Could not load Level2_Terrain scene! Path: " + scenePath);
            return;
        }
        
        Debug.Log("=== Setting up Level2_Terrain scene ===");
        
        // 1. Configure Terrain
        SetupTerrain();
        
        // 2. Add Directional Light
        SetupLighting();
        
        // 3. Add Player
        SetupPlayer();
        
        // 4. Add UI (Canvas, LivesText, GameOverText)
        SetupUI();
        
        // 5. Add Enemy
        SetupEnemy();
        
        // 6. Setup NavMesh
        SetupNavMesh();
        
        // 7. Add scene loading trigger (if needed)
        SetupSceneLoading();
        
        // Save the scene
        EditorSceneManager.SaveScene(scene);
        Debug.Log("=== Level2_Terrain scene setup complete! ===");
    }
    
    static void SetupTerrain()
    {
        Debug.Log("Setting up Terrain...");
        
        Terrain terrain = Object.FindFirstObjectByType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Terrain not found! Please create a Terrain GameObject first.");
            return;
        }
        
        TerrainData terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            // Create new terrain data
            terrainData = new TerrainData();
            terrainData.size = new Vector3(200, 30, 200); // 200x200 terrain, 30 units high
            terrain.terrainData = terrainData;
        }
        
        // Modify terrain heights to create hills and valleys
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];
        
        // Create varied terrain with hills and valleys
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * 10f;
                float yCoord = (float)y / height * 10f;
                
                // Combine multiple noise functions for varied terrain
                float heightValue = Mathf.PerlinNoise(xCoord * 0.5f, yCoord * 0.5f) * 0.3f;
                heightValue += Mathf.PerlinNoise(xCoord * 1.5f, yCoord * 1.5f) * 0.15f;
                heightValue += Mathf.PerlinNoise(xCoord * 3f, yCoord * 3f) * 0.1f;
                
                // Create a valley in the center
                float centerX = width / 2f;
                float centerY = height / 2f;
                float distFromCenter = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                float maxDist = Mathf.Sqrt(centerX * centerX + centerY * centerY);
                float valleyFactor = 1f - (distFromCenter / maxDist) * 0.5f;
                heightValue *= valleyFactor;
                
                heights[x, y] = heightValue;
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
        Debug.Log("Terrain heights configured with hills and valleys");
        
        // Add terrain textures (grass and rock/dirt)
        TerrainLayer[] terrainLayers = new TerrainLayer[2];
        
        // Layer 1: Grass (base)
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = CreateSimpleTexture(Color.green, "GrassTexture");
        terrainLayers[0].tileSize = new Vector2(15, 15);
        
        // Layer 2: Rock/Dirt
        terrainLayers[1] = new TerrainLayer();
        terrainLayers[1].diffuseTexture = CreateSimpleTexture(new Color(0.5f, 0.4f, 0.3f), "RockTexture");
        terrainLayers[1].tileSize = new Vector2(15, 15);
        
        terrainData.terrainLayers = terrainLayers;
        
        // Paint textures on terrain
        float[,,] alphamaps = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 2];
        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                float normalizedX = (float)x / terrainData.alphamapWidth;
                float normalizedY = (float)y / terrainData.alphamapHeight;
                
                // Base grass layer
                alphamaps[x, y, 0] = 0.7f;
                // Rock layer on higher areas
                float heightAtPos = terrainData.GetHeight(x, y) / terrainData.size.y;
                alphamaps[x, y, 1] = heightAtPos > 0.4f ? 0.5f : 0.1f;
                
                // Normalize
                float total = alphamaps[x, y, 0] + alphamaps[x, y, 1];
                if (total > 0)
                {
                    alphamaps[x, y, 0] /= total;
                    alphamaps[x, y, 1] /= total;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, alphamaps);
        Debug.Log("Terrain textures painted (grass and rock)");
        
        // Add grass details
        DetailPrototype[] detailPrototypes = new DetailPrototype[1];
        detailPrototypes[0] = new DetailPrototype();
        detailPrototypes[0].prototype = CreateGrassPrefab();
        detailPrototypes[0].renderMode = DetailRenderMode.Grass;
        terrainData.detailPrototypes = detailPrototypes;
        
        // Paint grass details
        int[,] details = new int[terrainData.detailWidth, terrainData.detailHeight];
        for (int x = 0; x < terrainData.detailWidth; x++)
        {
            for (int y = 0; y < terrainData.detailHeight; y++)
            {
                // Add grass in flatter areas (not on high hills)
                float heightAtPos = terrainData.GetHeight(x * terrainData.detailWidth / terrainData.heightmapResolution, 
                                                          y * terrainData.detailHeight / terrainData.heightmapResolution) / terrainData.size.y;
                if (heightAtPos < 0.5f && Random.Range(0f, 1f) > 0.3f)
                {
                    details[x, y] = Random.Range(1, 3); // 1-2 grass instances per cell
                }
            }
        }
        terrainData.SetDetailLayer(0, 0, 0, details);
        Debug.Log("Grass details added to terrain");
        
        // Add trees
        TreePrototype[] treePrototypes = new TreePrototype[1];
        treePrototypes[0] = new TreePrototype();
        treePrototypes[0].prefab = CreateTreePrefab();
        terrainData.treePrototypes = treePrototypes;
        
        // Place some trees
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(0.2f, 0.8f) * terrainData.size.x;
            float z = Random.Range(0.2f, 0.8f) * terrainData.size.z;
            float y = terrain.SampleHeight(new Vector3(x, 0, z));
            
            TreeInstance tree = new TreeInstance();
            tree.position = new Vector3(x / terrainData.size.x, y / terrainData.size.y, z / terrainData.size.z);
            tree.prototypeIndex = 0;
            tree.widthScale = Random.Range(0.8f, 1.2f);
            tree.heightScale = Random.Range(0.8f, 1.2f);
            tree.color = Color.white;
            tree.lightmapColor = Color.white;
            
            terrain.AddTreeInstance(tree);
        }
        Debug.Log("Trees added to terrain");
        
        EditorUtility.SetDirty(terrain);
        EditorUtility.SetDirty(terrainData);
    }
    
    static Texture2D CreateSimpleTexture(Color color, string name)
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = name;
        return texture;
    }
    
    static GameObject CreateGrassPrefab()
    {
        // Create a simple grass quad
        GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Quad);
        grass.name = "GrassDetail";
        grass.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        grass.GetComponent<Renderer>().material.color = new Color(0.2f, 0.6f, 0.2f);
        DestroyImmediate(grass.GetComponent<Collider>());
        return grass;
    }
    
    static GameObject CreateTreePrefab()
    {
        // Create a simple tree (cylinder for trunk, sphere for leaves)
        GameObject tree = new GameObject("Tree");
        
        // Trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1, 0);
        trunk.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0.1f);
        
        // Leaves
        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.transform.SetParent(tree.transform);
        leaves.transform.localPosition = new Vector3(0, 2.5f, 0);
        leaves.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        leaves.GetComponent<Renderer>().material.color = new Color(0.2f, 0.5f, 0.2f);
        DestroyImmediate(leaves.GetComponent<Collider>());
        
        return tree;
    }
    
    static void SetupLighting()
    {
        Debug.Log("Setting up lighting...");
        
        // Find or create directional light
        Light dirLight = Object.FindFirstObjectByType<Light>();
        if (dirLight == null || dirLight.type != LightType.Directional)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        dirLight.color = new Color(1f, 0.95f, 0.9f);
        dirLight.intensity = 1f;
        
        // Set skybox to default
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.2f);
        
        Debug.Log("Lighting configured");
    }
    
    static void SetupPlayer()
    {
        Debug.Log("Setting up Player...");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Try to find Player in the first scene and copy it
            Scene firstScene = EditorSceneManager.OpenScene("Assets/Scenes/ok.unity", OpenSceneMode.Additive);
            GameObject playerInFirstScene = GameObject.FindGameObjectWithTag("Player");
            
            if (playerInFirstScene != null)
            {
                // Copy player to new scene
                player = Instantiate(playerInFirstScene);
                player.name = "Player";
                EditorSceneManager.MoveGameObjectToScene(player, EditorSceneManager.GetActiveScene());
                Debug.Log("Copied Player from first scene");
            }
            else
            {
                // Create a basic player if not found
                player = new GameObject("Player");
                player.tag = "Player";
                player.AddComponent<CapsuleCollider>();
                Rigidbody rb = player.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                player.AddComponent<PlayerController>();
                player.AddComponent<PlayerLives>();
                Debug.Log("Created new Player GameObject");
            }
            
            EditorSceneManager.CloseScene(firstScene, false);
        }
        
        // Position player on terrain
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain != null)
        {
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            float height = terrain.SampleHeight(terrainCenter);
            player.transform.position = new Vector3(terrainCenter.x, height + 2f, terrainCenter.z);
        }
        else
        {
            player.transform.position = new Vector3(0, 5, 0);
        }
        
        Debug.Log("Player positioned on terrain");
    }
    
    static void SetupUI()
    {
        Debug.Log("Setting up UI...");
        
        // UI will be set up automatically by LivesUISetup at runtime
        // But we can ensure Canvas exists
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Ensure EventSystem exists
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
        
        Debug.Log("UI Canvas and EventSystem ready (LivesUISetup will configure at runtime)");
    }
    
    static void SetupEnemy()
    {
        Debug.Log("Setting up Enemy...");
        
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null)
        {
            // Try to find Enemy1 in the first scene and copy it
            Scene firstScene = EditorSceneManager.OpenScene("Assets/Scenes/ok.unity", OpenSceneMode.Additive);
            GameObject enemyInFirstScene = GameObject.Find("Enemy1");
            
            if (enemyInFirstScene == null)
            {
                // Try to find any enemy
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies.Length > 0)
                {
                    enemyInFirstScene = enemies[0];
                }
            }
            
            if (enemyInFirstScene != null)
            {
                // Copy enemy to new scene
                enemy = Instantiate(enemyInFirstScene);
                enemy.name = "Enemy1";
                EditorSceneManager.MoveGameObjectToScene(enemy, EditorSceneManager.GetActiveScene());
                Debug.Log("Copied Enemy from first scene");
            }
            else
            {
                // Create a basic enemy if not found
                enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = "Enemy1";
                enemy.tag = "Enemy";
                enemy.transform.localScale = new Vector3(1, 1.5f, 1);
                enemy.GetComponent<Renderer>().material.color = Color.red;
                
                // Add Rigidbody
                Rigidbody rb = enemy.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                
                // Add NavMeshAgent
                NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
                agent.radius = 0.5f;
                agent.height = 2f;
                agent.speed = 3.5f;
                
                // Add EnemyMovement script
                enemy.AddComponent<EnemyMovement>();
                
                Debug.Log("Created new Enemy GameObject");
            }
            
            EditorSceneManager.CloseScene(firstScene, false);
        }
        
        // Position enemy on terrain
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain != null)
        {
            Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size / 2f;
            Vector3 enemyPos = terrainCenter + new Vector3(20, 0, 20);
            float height = terrain.SampleHeight(enemyPos);
            enemy.transform.position = new Vector3(enemyPos.x, height + 1f, enemyPos.z);
        }
        else
        {
            enemy.transform.position = new Vector3(20, 5, 20);
        }
        
        Debug.Log("Enemy positioned on terrain");
    }
    
    static void SetupNavMesh()
    {
        Debug.Log("Setting up NavMesh...");
        
        // Find or create NavMeshSurface
        NavMeshSurface navMeshSurface = Object.FindFirstObjectByType<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            GameObject navMeshObj = new GameObject("NavMeshSurface");
            navMeshSurface = navMeshObj.AddComponent<NavMeshSurface>();
        }
        
        // Configure NavMeshSurface
        navMeshSurface.collectObjects = CollectObjects.All;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        
        // Build NavMesh
        navMeshSurface.BuildNavMesh();
        
        Debug.Log("NavMesh baked for terrain");
    }
    
    static void SetupSceneLoading()
    {
        Debug.Log("Checking scene loading setup...");
        
        // Check if PlayerController has scene loading logic
        // If not, we'll add it via a trigger or modify PlayerController
        // For now, the user mentioned scene loading already works, so we'll just verify
        
        // Add a script to handle scene loading if needed
        GameObject sceneLoader = GameObject.Find("SceneLoader");
        if (sceneLoader == null)
        {
            // Scene loading is handled in PlayerController when count >= 12
            // We just need to make sure it loads Level2_Terrain
            Debug.Log("Scene loading should be handled by PlayerController. Verify it loads 'Level2_Terrain'");
        }
    }
}
