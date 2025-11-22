using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script to fix NavMeshAgent placement and ensure it's on the NavMesh.
/// This runs in Start() to warp the agent to the NavMesh if needed.
/// </summary>
public class FixSkeletonNavMesh : MonoBehaviour
{
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("FixSkeletonNavMesh: No NavMeshAgent found!");
            return;
        }
        
        // Disable agent temporarily to warp it
        agent.enabled = false;
        
        // Try to find the nearest point on NavMesh
        NavMeshHit hit;
        Vector3 currentPos = transform.position;
        
        if (NavMesh.SamplePosition(currentPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            // Found a valid position on NavMesh
            transform.position = hit.position;
            Debug.Log($"FixSkeletonNavMesh: Warped {gameObject.name} to NavMesh position: {hit.position}");
        }
        else
        {
            Debug.LogWarning($"FixSkeletonNavMesh: Could not find NavMesh near {gameObject.name} at {currentPos}. " +
                           "The NavMesh may not cover the second room. Please rebake the NavMesh to include both rooms.");
        }
        
        // Re-enable agent
        agent.enabled = true;
        
        // Wait a frame and check if on NavMesh
        StartCoroutine(CheckNavMeshStatus());
    }
    
    System.Collections.IEnumerator CheckNavMeshStatus()
    {
        yield return null; // Wait one frame
        
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                Debug.Log($"FixSkeletonNavMesh: {gameObject.name} is now on NavMesh!");
            }
            else
            {
                Debug.LogError($"FixSkeletonNavMesh: {gameObject.name} is still NOT on NavMesh! " +
                             "Please ensure the NavMesh covers the second room and rebake if necessary.");
            }
        }
    }
}

