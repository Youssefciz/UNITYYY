using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Immediate fix script that configures Skeleton_death and ensures it's on NavMesh.
/// This runs in Awake() to fix everything before Start().
/// </summary>
public class FixSkeletonImmediate : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("FixSkeletonImmediate: Starting immediate fix for " + gameObject.name);
        
        // Find Enemy1
        GameObject enemy1 = GameObject.Find("Enemy1");
        if (enemy1 == null)
        {
            Debug.LogError("FixSkeletonImmediate: Could not find Enemy1!");
            return;
        }
        
        // Get Enemy1 components
        Rigidbody enemy1Rb = enemy1.GetComponent<Rigidbody>();
        NavMeshAgent enemy1Nav = enemy1.GetComponent<NavMeshAgent>();
        EnemyMovement enemy1Movement = enemy1.GetComponent<EnemyMovement>();
        
        // Fix Rigidbody IMMEDIATELY
        Rigidbody myRb = GetComponent<Rigidbody>();
        if (myRb != null && enemy1Rb != null)
        {
            myRb.isKinematic = enemy1Rb.isKinematic;
            myRb.useGravity = enemy1Rb.useGravity;
            myRb.freezeRotation = enemy1Rb.freezeRotation;
            myRb.constraints = enemy1Rb.constraints;
            Debug.Log($"FixSkeletonImmediate: Fixed Rigidbody - kinematic={myRb.isKinematic}, gravity={myRb.useGravity}");
        }
        
        // Fix NavMeshAgent IMMEDIATELY
        NavMeshAgent myNav = GetComponent<NavMeshAgent>();
        if (myNav != null && enemy1Nav != null)
        {
            myNav.speed = enemy1Nav.speed;
            myNav.stoppingDistance = enemy1Nav.stoppingDistance;
            myNav.angularSpeed = enemy1Nav.angularSpeed;
            myNav.acceleration = enemy1Nav.acceleration;
            Debug.Log($"FixSkeletonImmediate: Fixed NavMeshAgent - speed={myNav.speed}, stoppingDistance={myNav.stoppingDistance}");
        }
        
        // Fix EnemyMovement player reference
        EnemyMovement myMovement = GetComponent<EnemyMovement>();
        if (myMovement != null)
        {
            if (enemy1Movement != null && enemy1Movement.player != null)
            {
                myMovement.player = enemy1Movement.player;
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    myMovement.player = player.transform;
                    Debug.Log($"FixSkeletonImmediate: Set Player reference to {player.name}");
                }
            }
        }
        
        // Try to warp to NavMesh
        WarpToNavMesh();
    }
    
    void WarpToNavMesh()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;
        
        // Disable agent
        bool wasEnabled = agent.enabled;
        agent.enabled = false;
        
        Vector3 currentPos = transform.position;
        NavMeshHit hit;
        
        // Try to find NavMesh within 20 units (larger search radius)
        if (NavMesh.SamplePosition(currentPos, out hit, 20.0f, NavMesh.AllAreas))
        {
            // Found NavMesh - move to it
            transform.position = hit.position;
            Debug.Log($"FixSkeletonImmediate: Warped {gameObject.name} to NavMesh at {hit.position}");
        }
        else
        {
            // Try to find NavMesh near Enemy1's position (first room)
            GameObject enemy1 = GameObject.Find("Enemy1");
            if (enemy1 != null)
            {
                Vector3 enemy1Pos = enemy1.transform.position;
                if (NavMesh.SamplePosition(enemy1Pos, out hit, 5.0f, NavMesh.AllAreas))
                {
                    // Move to a position near Enemy1 but in the second room direction
                    Vector3 newPos = new Vector3(hit.position.x, hit.position.y, hit.position.z + 15f);
                    if (NavMesh.SamplePosition(newPos, out hit, 10.0f, NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                        Debug.Log($"FixSkeletonImmediate: Moved {gameObject.name} to NavMesh near second room at {hit.position}");
                    }
                    else
                    {
                        Debug.LogWarning($"FixSkeletonImmediate: Could not find NavMesh in second room. Position: {currentPos}");
                    }
                }
            }
        }
        
        // Re-enable agent
        agent.enabled = wasEnabled;
        
        // Check after a frame
        StartCoroutine(VerifyNavMesh());
    }
    
    System.Collections.IEnumerator VerifyNavMesh()
    {
        yield return null;
        
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                Debug.Log($"FixSkeletonImmediate: SUCCESS! {gameObject.name} is now on NavMesh!");
            }
            else
            {
                Debug.LogError($"FixSkeletonImmediate: FAILED! {gameObject.name} is still NOT on NavMesh at {transform.position}. " +
                             "The NavMesh may not cover this area. Please ensure the NavMesh extends to z=28.64");
            }
        }
    }
}

