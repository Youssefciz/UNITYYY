using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SetupSkeletonEnemyMenu
{
    [MenuItem("Tools/Setup Skeleton Enemy")]
    public static void SetupSkeletonEnemy()
    {
        // Make sure we're in the correct scene
        if (SceneManager.GetActiveScene().name != "ok")
        {
            EditorUtility.DisplayDialog("Wrong Scene", 
                "Please open the 'ok' scene first before running this setup.", "OK");
            return;
        }
        
        // Find Enemy1
        GameObject enemy1 = GameObject.Find("Enemy1");
        if (enemy1 == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Could not find Enemy1! Please make sure Enemy1 exists in the scene.", "OK");
            return;
        }
        
        // Find skeleton GameObject
        GameObject skeleton = FindSkeletonInScene();
        
        if (skeleton == null)
        {
            // Try to create from prefab
            GameObject skeletonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/SazenGames/Skeleton/Prefabs/Skeleton_110.prefab");
            
            if (skeletonPrefab != null)
            {
                // Ask user if they want to create it
                bool create = EditorUtility.DisplayDialog("Skeleton Not Found", 
                    "Could not find skeleton GameObject in scene. Would you like to create it from the prefab?", 
                    "Yes", "No");
                
                if (create)
                {
                    skeleton = PrefabUtility.InstantiatePrefab(skeletonPrefab) as GameObject;
                    skeleton.transform.position = new Vector3(0f, 0.5f, 20f); // Second room position
                    skeleton.name = "SkeletonEnemy";
                    Undo.RegisterCreatedObjectUndo(skeleton, "Create Skeleton Enemy");
                }
                else
                {
                    return;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", 
                    "Could not find skeleton prefab at Assets/SazenGames/Skeleton/Prefabs/Skeleton_110.prefab", "OK");
                return;
            }
        }
        
        // Configure skeleton
        ConfigureSkeletonEnemy(skeleton, enemy1);
        
        EditorUtility.DisplayDialog("Success", 
            $"Skeleton enemy '{skeleton.name}' has been configured to match Enemy1!\n\n" +
            $"Components added:\n" +
            $"- Tag: Enemy\n" +
            $"- CapsuleCollider\n" +
            $"- Rigidbody (Kinematic)\n" +
            $"- NavMeshAgent\n" +
            $"- EnemyMovement\n" +
            $"- EnemyCollision", "OK");
        
        // Select the skeleton in the hierarchy
        Selection.activeGameObject = skeleton;
    }
    
    private static GameObject FindSkeletonInScene()
    {
        // Search for skeleton by name patterns
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            string name = obj.name;
            if (name.Contains("Skeleton") || name.Contains("skeleton") || 
                name.Contains("110") || name.Contains("Skeleton_110"))
            {
                return obj;
            }
        }
        
        // Try finding by prefab connection
        GameObject skeletonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/SazenGames/Skeleton/Prefabs/Skeleton_110.prefab");
        
        if (skeletonPrefab != null)
        {
            foreach (GameObject obj in allObjects)
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == skeletonPrefab)
                {
                    return obj;
                }
            }
        }
        
        return null;
    }
    
    private static void ConfigureSkeletonEnemy(GameObject skeleton, GameObject enemy1)
    {
        Undo.RecordObject(skeleton, "Configure Skeleton Enemy");
        
        // Get Enemy1 components
        CapsuleCollider enemy1Collider = enemy1.GetComponent<CapsuleCollider>();
        Rigidbody enemy1Rigidbody = enemy1.GetComponent<Rigidbody>();
        UnityEngine.AI.NavMeshAgent enemy1NavAgent = enemy1.GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMovement enemy1Movement = enemy1.GetComponent<EnemyMovement>();
        
        // Set tag to "Enemy"
        skeleton.tag = "Enemy";
        
        // Add or configure CapsuleCollider
        CapsuleCollider skeletonCollider = skeleton.GetComponent<CapsuleCollider>();
        if (skeletonCollider == null)
        {
            skeletonCollider = Undo.AddComponent<CapsuleCollider>(skeleton);
        }
        
        if (enemy1Collider != null)
        {
            skeletonCollider.radius = enemy1Collider.radius;
            skeletonCollider.height = enemy1Collider.height;
            skeletonCollider.center = enemy1Collider.center;
            skeletonCollider.isTrigger = enemy1Collider.isTrigger;
            skeletonCollider.direction = enemy1Collider.direction;
        }
        else
        {
            skeletonCollider.radius = 0.5f;
            skeletonCollider.height = 2.0f;
            skeletonCollider.center = Vector3.zero;
            skeletonCollider.isTrigger = false;
            skeletonCollider.direction = 1;
        }
        
        // Add or configure Rigidbody
        Rigidbody skeletonRigidbody = skeleton.GetComponent<Rigidbody>();
        if (skeletonRigidbody == null)
        {
            skeletonRigidbody = Undo.AddComponent<Rigidbody>(skeleton);
        }
        
        if (enemy1Rigidbody != null)
        {
            skeletonRigidbody.isKinematic = enemy1Rigidbody.isKinematic;
            skeletonRigidbody.useGravity = enemy1Rigidbody.useGravity;
            skeletonRigidbody.freezeRotation = enemy1Rigidbody.freezeRotation;
            skeletonRigidbody.mass = enemy1Rigidbody.mass;
            skeletonRigidbody.linearDamping = enemy1Rigidbody.linearDamping;
            skeletonRigidbody.angularDamping = enemy1Rigidbody.angularDamping;
            skeletonRigidbody.constraints = enemy1Rigidbody.constraints;
        }
        else
        {
            skeletonRigidbody.isKinematic = true;
            skeletonRigidbody.useGravity = false;
            skeletonRigidbody.freezeRotation = true;
            skeletonRigidbody.mass = 1.0f;
            skeletonRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Add or configure NavMeshAgent
        UnityEngine.AI.NavMeshAgent skeletonNavAgent = skeleton.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (skeletonNavAgent == null)
        {
            skeletonNavAgent = Undo.AddComponent<UnityEngine.AI.NavMeshAgent>(skeleton);
        }
        
        if (enemy1NavAgent != null)
        {
            skeletonNavAgent.speed = enemy1NavAgent.speed;
            skeletonNavAgent.angularSpeed = enemy1NavAgent.angularSpeed;
            skeletonNavAgent.acceleration = enemy1NavAgent.acceleration;
            skeletonNavAgent.radius = enemy1NavAgent.radius;
            skeletonNavAgent.height = enemy1NavAgent.height;
            skeletonNavAgent.stoppingDistance = enemy1NavAgent.stoppingDistance;
            skeletonNavAgent.obstacleAvoidanceType = enemy1NavAgent.obstacleAvoidanceType;
            skeletonNavAgent.avoidancePriority = enemy1NavAgent.avoidancePriority;
            skeletonNavAgent.baseOffset = enemy1NavAgent.baseOffset;
        }
        else
        {
            skeletonNavAgent.speed = 3.0f;
            skeletonNavAgent.angularSpeed = 120.0f;
            skeletonNavAgent.acceleration = 8.0f;
            skeletonNavAgent.radius = 0.5f;
            skeletonNavAgent.height = 2.0f;
            skeletonNavAgent.stoppingDistance = 0.5f;
            skeletonNavAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            skeletonNavAgent.avoidancePriority = 50;
        }
        
        // Add EnemyMovement script
        EnemyMovement skeletonMovement = skeleton.GetComponent<EnemyMovement>();
        if (skeletonMovement == null)
        {
            skeletonMovement = Undo.AddComponent<EnemyMovement>(skeleton);
        }
        
        // Set player reference
        if (enemy1Movement != null && enemy1Movement.player != null)
        {
            skeletonMovement.player = enemy1Movement.player;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                skeletonMovement.player = player.transform;
            }
        }
        
        // Add EnemyCollision script
        EnemyCollision skeletonCollision = skeleton.GetComponent<EnemyCollision>();
        if (skeletonCollision == null)
        {
            skeletonCollision = Undo.AddComponent<EnemyCollision>(skeleton);
        }
        
        EditorUtility.SetDirty(skeleton);
        Debug.Log($"SetupSkeletonEnemyMenu: Configured {skeleton.name} to match Enemy1");
    }
}

