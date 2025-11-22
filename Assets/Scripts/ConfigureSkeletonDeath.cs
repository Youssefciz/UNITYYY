using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script to configure Skeleton_death GameObject to match Enemy1 settings.
/// This script runs once in Start() to set up all component properties.
/// </summary>
public class ConfigureSkeletonDeath : MonoBehaviour
{
    [Header("Reference to Enemy1")]
    public GameObject enemy1Reference;
    
    void Start()
    {
        // Find Enemy1 if not assigned
        if (enemy1Reference == null)
        {
            enemy1Reference = GameObject.Find("Enemy1");
        }
        
        if (enemy1Reference == null)
        {
            Debug.LogError("ConfigureSkeletonDeath: Could not find Enemy1!");
            return;
        }
        
        // Configure this GameObject (Skeleton_death) to match Enemy1
        ConfigureToMatchEnemy1();
    }
    
    void ConfigureToMatchEnemy1()
    {
        // Get Enemy1 components
        CapsuleCollider enemy1Collider = enemy1Reference.GetComponent<CapsuleCollider>();
        Rigidbody enemy1Rigidbody = enemy1Reference.GetComponent<Rigidbody>();
        NavMeshAgent enemy1NavAgent = enemy1Reference.GetComponent<NavMeshAgent>();
        EnemyMovement enemy1Movement = enemy1Reference.GetComponent<EnemyMovement>();
        
        // Configure CapsuleCollider
        CapsuleCollider myCollider = GetComponent<CapsuleCollider>();
        if (myCollider != null && enemy1Collider != null)
        {
            myCollider.radius = enemy1Collider.radius;
            myCollider.height = enemy1Collider.height;
            myCollider.center = enemy1Collider.center;
            myCollider.isTrigger = enemy1Collider.isTrigger;
            myCollider.direction = enemy1Collider.direction;
            Debug.Log($"ConfigureSkeletonDeath: CapsuleCollider configured - radius={myCollider.radius}, height={myCollider.height}");
        }
        
        // Configure Rigidbody
        Rigidbody myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody != null && enemy1Rigidbody != null)
        {
            myRigidbody.isKinematic = enemy1Rigidbody.isKinematic;
            myRigidbody.useGravity = enemy1Rigidbody.useGravity;
            myRigidbody.freezeRotation = enemy1Rigidbody.freezeRotation;
            myRigidbody.mass = enemy1Rigidbody.mass;
            myRigidbody.linearDamping = enemy1Rigidbody.linearDamping;
            myRigidbody.angularDamping = enemy1Rigidbody.angularDamping;
            myRigidbody.constraints = enemy1Rigidbody.constraints;
            Debug.Log($"ConfigureSkeletonDeath: Rigidbody configured - isKinematic={myRigidbody.isKinematic}, useGravity={myRigidbody.useGravity}, freezeRotation={myRigidbody.freezeRotation}");
        }
        
        // Configure NavMeshAgent
        NavMeshAgent myNavAgent = GetComponent<NavMeshAgent>();
        if (myNavAgent != null && enemy1NavAgent != null)
        {
            myNavAgent.speed = enemy1NavAgent.speed;
            myNavAgent.angularSpeed = enemy1NavAgent.angularSpeed;
            myNavAgent.acceleration = enemy1NavAgent.acceleration;
            myNavAgent.radius = enemy1NavAgent.radius;
            myNavAgent.height = enemy1NavAgent.height;
            myNavAgent.stoppingDistance = enemy1NavAgent.stoppingDistance;
            myNavAgent.obstacleAvoidanceType = enemy1NavAgent.obstacleAvoidanceType;
            myNavAgent.avoidancePriority = enemy1NavAgent.avoidancePriority;
            myNavAgent.baseOffset = enemy1NavAgent.baseOffset;
            Debug.Log($"ConfigureSkeletonDeath: NavMeshAgent configured - speed={myNavAgent.speed}, stoppingDistance={myNavAgent.stoppingDistance}");
        }
        
        // Configure EnemyMovement - set player reference
        EnemyMovement myMovement = GetComponent<EnemyMovement>();
        if (myMovement != null)
        {
            if (enemy1Movement != null && enemy1Movement.player != null)
            {
                myMovement.player = enemy1Movement.player;
                Debug.Log($"ConfigureSkeletonDeath: EnemyMovement player reference set to {enemy1Movement.player.name}");
            }
            else
            {
                // Find player automatically
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    myMovement.player = player.transform;
                    Debug.Log($"ConfigureSkeletonDeath: EnemyMovement player reference set to {player.name} (auto-found)");
                }
            }
        }
        
        Debug.Log("ConfigureSkeletonDeath: Configuration complete!");
    }
}

