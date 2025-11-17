using UnityEngine;
using UnityEngine.AI;

public class PetFollower : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;
    
    [Header("Follow Settings")]
    [Tooltip("How far the pet will stay from the player")]
    public float followDistance = 3f;
    
    [Tooltip("How fast the pet moves")]
    public float followSpeed = 4f;
    
    [Tooltip("How quickly the pet accelerates")]
    public float acceleration = 8f;
    
    [Tooltip("How close the pet gets before stopping")]
    public float stoppingDistance = 2f;
    
    [Tooltip("How often to update the destination (in seconds)")]
    public float updateInterval = 0.5f;

    private NavMeshAgent navAgent;
    private float lastUpdateTime = 0f;

    void Start()
    {
        // Handle Rigidbody - NavMeshAgent works better without Rigidbody interference
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Make Rigidbody kinematic so NavMeshAgent can control movement
            rb.isKinematic = true;
        }

        // Get or add NavMeshAgent component
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Configure NavMeshAgent for smooth pet following
        navAgent.speed = followSpeed;
        navAgent.acceleration = acceleration;
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.angularSpeed = 360f; // Smooth rotation
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        navAgent.height = 1.5f; // Match pet height
        navAgent.radius = 0.4f; // Match pet radius
        
        // Make sure agent can rotate
        navAgent.updateRotation = true;
        navAgent.updateUpAxis = true;

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Pet found player: " + player.name);
            }
            else
            {
                Debug.LogWarning("Pet could not find player with tag 'Player'");
            }
        }
        else
        {
            Debug.Log("Pet has player assigned: " + player.name);
        }
    }

    void Update()
    {
        // Check if we have a valid player and NavMeshAgent
        if (player == null || navAgent == null)
        {
            return;
        }

        // Check if agent is on NavMesh
        if (!navAgent.isOnNavMesh)
        {
            Debug.LogWarning("Pet NavMeshAgent is not on NavMesh!");
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only update destination if player is far enough away and enough time has passed
        if (distanceToPlayer > followDistance && Time.time - lastUpdateTime >= updateInterval)
        {
            navAgent.SetDestination(player.position);
            lastUpdateTime = Time.time;
        }
        // If player is close, stop moving
        else if (distanceToPlayer <= followDistance)
        {
            if (navAgent.hasPath)
            {
                navAgent.ResetPath();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize follow distance in editor
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, followDistance);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
