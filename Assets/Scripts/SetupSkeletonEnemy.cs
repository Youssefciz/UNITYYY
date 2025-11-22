using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script to automatically find and configure the skeleton enemy to match Enemy1's setup.
/// Attach this to any GameObject in the scene, or run it via menu item.
/// </summary>
public class SetupSkeletonEnemy : MonoBehaviour
{
    [Header("Skeleton Enemy Setup")]
    [Tooltip("Name or partial name of the skeleton GameObject to configure")]
    public string skeletonName = "Skeleton";
    
    [Tooltip("Reference to Enemy1 to copy settings from")]
    public GameObject enemy1Reference;
    
    [Tooltip("Position in second room (north of first room)")]
    public Vector3 skeletonPosition = new Vector3(0f, 0.5f, 20f);
    
    void Start()
    {
        SetupSkeleton();
    }
    
    [ContextMenu("Setup Skeleton Enemy")]
    public void SetupSkeleton()
    {
        // Find Enemy1 if not assigned
        if (enemy1Reference == null)
        {
            enemy1Reference = GameObject.Find("Enemy1");
            if (enemy1Reference == null)
            {
                Debug.LogError("SetupSkeletonEnemy: Could not find Enemy1! Cannot copy settings.");
                return;
            }
        }
        
        // Find skeleton GameObject
        GameObject skeleton = FindSkeletonGameObject();
        
        if (skeleton == null)
        {
            Debug.LogWarning("SetupSkeletonEnemy: Could not find skeleton GameObject. Creating from prefab...");
            skeleton = CreateSkeletonFromPrefab();
        }
        
        if (skeleton == null)
        {
            Debug.LogError("SetupSkeletonEnemy: Failed to find or create skeleton GameObject!");
            return;
        }
        
        Debug.Log("SetupSkeletonEnemy: Found skeleton at " + skeleton.name + ", configuring...");
        
        // Configure skeleton to match Enemy1
        ConfigureSkeletonEnemy(skeleton, enemy1Reference);
        
        Debug.Log("SetupSkeletonEnemy: Skeleton enemy configured successfully!");
    }
    
    private GameObject FindSkeletonGameObject()
    {
        // Try multiple search patterns
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Skeleton") || obj.name.Contains("skeleton") || 
                obj.name.Contains("110") || obj.name.Contains("Skeleton_110"))
            {
                // Check if it's in the second room area (north, z > 10)
                if (obj.transform.position.z > 10f)
                {
                    return obj;
                }
            }
        }
        
        // Also try finding by prefab
        GameObject skeletonPrefab = Resources.Load<GameObject>("Skeleton_110");
        if (skeletonPrefab == null)
        {
            // Try loading from Assets path
            #if UNITY_EDITOR
            skeletonPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/SazenGames/Skeleton/Prefabs/Skeleton_110.prefab");
            #endif
        }
        
        if (skeletonPrefab != null)
        {
            // Check if there's an instance in the scene
            GameObject[] prefabInstances = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in prefabInstances)
            {
                #if UNITY_EDITOR
                if (UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(obj) == skeletonPrefab)
                {
                    return obj;
                }
                #endif
            }
        }
        
        return null;
    }
    
    private GameObject CreateSkeletonFromPrefab()
    {
        #if UNITY_EDITOR
        GameObject skeletonPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/SazenGames/Skeleton/Prefabs/Skeleton_110.prefab");
        
        if (skeletonPrefab != null)
        {
            GameObject instance = UnityEditor.PrefabUtility.InstantiatePrefab(skeletonPrefab) as GameObject;
            instance.transform.position = skeletonPosition;
            instance.name = "SkeletonEnemy";
            return instance;
        }
        #endif
        
        return null;
    }
    
    private void ConfigureSkeletonEnemy(GameObject skeleton, GameObject enemy1)
    {
        // Get Enemy1 components
        CapsuleCollider enemy1Collider = enemy1.GetComponent<CapsuleCollider>();
        Rigidbody enemy1Rigidbody = enemy1.GetComponent<Rigidbody>();
        NavMeshAgent enemy1NavAgent = enemy1.GetComponent<NavMeshAgent>();
        EnemyMovement enemy1Movement = enemy1.GetComponent<EnemyMovement>();
        
        // Set tag to "Enemy"
        skeleton.tag = "Enemy";
        
        // Add or configure CapsuleCollider
        CapsuleCollider skeletonCollider = skeleton.GetComponent<CapsuleCollider>();
        if (skeletonCollider == null)
        {
            skeletonCollider = skeleton.AddComponent<CapsuleCollider>();
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
            // Default values matching Enemy1
            skeletonCollider.radius = 0.5f;
            skeletonCollider.height = 2.0f;
            skeletonCollider.center = Vector3.zero;
            skeletonCollider.isTrigger = false;
            skeletonCollider.direction = 1; // Y-axis
        }
        
        // Add or configure Rigidbody
        Rigidbody skeletonRigidbody = skeleton.GetComponent<Rigidbody>();
        if (skeletonRigidbody == null)
        {
            skeletonRigidbody = skeleton.AddComponent<Rigidbody>();
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
            // Default values matching Enemy1
            skeletonRigidbody.isKinematic = true;
            skeletonRigidbody.useGravity = false;
            skeletonRigidbody.freezeRotation = true;
            skeletonRigidbody.mass = 1.0f;
            skeletonRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Add or configure NavMeshAgent
        NavMeshAgent skeletonNavAgent = skeleton.GetComponent<NavMeshAgent>();
        if (skeletonNavAgent == null)
        {
            skeletonNavAgent = skeleton.AddComponent<NavMeshAgent>();
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
            // Default values matching Enemy1
            skeletonNavAgent.speed = 3.0f;
            skeletonNavAgent.angularSpeed = 120.0f;
            skeletonNavAgent.acceleration = 8.0f;
            skeletonNavAgent.radius = 0.5f;
            skeletonNavAgent.height = 2.0f;
            skeletonNavAgent.stoppingDistance = 0.5f;
            skeletonNavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            skeletonNavAgent.avoidancePriority = 50;
        }
        
        // Add EnemyMovement script
        EnemyMovement skeletonMovement = skeleton.GetComponent<EnemyMovement>();
        if (skeletonMovement == null)
        {
            skeletonMovement = skeleton.AddComponent<EnemyMovement>();
        }
        
        // Set player reference
        if (enemy1Movement != null && enemy1Movement.player != null)
        {
            skeletonMovement.player = enemy1Movement.player;
        }
        else
        {
            // Find player automatically
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                skeletonMovement.player = player.transform;
            }
        }
        
        // Add EnemyCollision script (optional, but for consistency)
        EnemyCollision skeletonCollision = skeleton.GetComponent<EnemyCollision>();
        if (skeletonCollision == null)
        {
            skeletonCollision = skeleton.AddComponent<EnemyCollision>();
        }
        
        Debug.Log($"SetupSkeletonEnemy: Configured {skeleton.name} with tag '{skeleton.tag}', " +
                 $"CapsuleCollider (r={skeletonCollider.radius}, h={skeletonCollider.height}), " +
                 $"Rigidbody (kinematic={skeletonRigidbody.isKinematic}), " +
                 $"NavMeshAgent (speed={skeletonNavAgent.speed}), " +
                 $"EnemyMovement, EnemyCollision");
    }
}

