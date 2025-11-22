using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // Reference to the player's transform.
    public Transform player;

    // Reference to the NavMeshAgent component for pathfinding.
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the NavMeshAgent component attached to this object.
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        // Debug logging to identify which enemy is running
        Debug.Log($"[EnemyMovement] Start called on {gameObject.name}");
        
        // If player is not assigned, try to find it
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"[EnemyMovement] {gameObject.name} found Player: {player.name}");
            }
            else
            {
                Debug.LogWarning($"[EnemyMovement] {gameObject.name} could not find Player!");
            }
        }
        else
        {
            Debug.Log($"[EnemyMovement] {gameObject.name} has Player assigned: {player.name}");
        }
        
        // Log NavMeshAgent status
        if (navMeshAgent != null)
        {
            Debug.Log($"[EnemyMovement] {gameObject.name} NavMeshAgent - isOnNavMesh: {navMeshAgent.isOnNavMesh}, enabled: {navMeshAgent.enabled}, speed: {navMeshAgent.speed}");
            
            // If agent is not on NavMesh, try to warp it
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning($"[EnemyMovement] {gameObject.name} is NOT on NavMesh! Attempting to warp...");
                WarpToNavMesh();
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyMovement] {gameObject.name} NavMeshAgent is NULL!");
        }
    }
    
    /// <summary>
    /// Warps the NavMeshAgent to the nearest valid position on the NavMesh.
    /// Tries progressively larger search radii and fallback positions.
    /// </summary>
    private void WarpToNavMesh()
    {
        if (navMeshAgent == null) return;
        
        NavMeshHit hit;
        Vector3 currentPos = transform.position;
        
        // Try progressively larger search radii
        float[] searchRadii = { 10.0f, 20.0f, 50.0f, 100.0f };
        bool foundNavMesh = false;
        
        foreach (float radius in searchRadii)
        {
            if (NavMesh.SamplePosition(currentPos, out hit, radius, NavMesh.AllAreas))
            {
                // Found NavMesh - warp to it
                bool wasEnabled = navMeshAgent.enabled;
                navMeshAgent.enabled = false;
                transform.position = hit.position;
                navMeshAgent.enabled = wasEnabled;
                
                Debug.Log($"[EnemyMovement] {gameObject.name} warped to NavMesh position: {hit.position} (found within {radius} units)");
                foundNavMesh = true;
                
                // Check again after a frame
                StartCoroutine(CheckNavMeshAfterWarp());
                return;
            }
        }
        
        // If not found at current position, try fallback positions
        if (!foundNavMesh)
        {
            // Try near player position
            if (player != null)
            {
                Vector3 playerPos = player.position;
                foreach (float radius in searchRadii)
                {
                    if (NavMesh.SamplePosition(playerPos, out hit, radius, NavMesh.AllAreas))
                    {
                        // Move to a position near player but on NavMesh
                        Vector3 offsetPos = hit.position + (currentPos - playerPos).normalized * 5f;
                        if (NavMesh.SamplePosition(offsetPos, out hit, 10.0f, NavMesh.AllAreas))
                        {
                            bool wasEnabled = navMeshAgent.enabled;
                            navMeshAgent.enabled = false;
                            transform.position = hit.position;
                            navMeshAgent.enabled = wasEnabled;
                            
                            Debug.LogWarning($"[EnemyMovement] {gameObject.name} warped to NavMesh near player area: {hit.position}");
                            foundNavMesh = true;
                            StartCoroutine(CheckNavMeshAfterWarp());
                            return;
                        }
                    }
                }
            }
            
            // Try at origin or common positions
            Vector3[] fallbackPositions = {
                Vector3.zero,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 15), // Middle of first room
                new Vector3(0, 0, 30)  // Middle of second room
            };
            
            foreach (Vector3 fallbackPos in fallbackPositions)
            {
                foreach (float radius in searchRadii)
                {
                    if (NavMesh.SamplePosition(fallbackPos, out hit, radius, NavMesh.AllAreas))
                    {
                        bool wasEnabled = navMeshAgent.enabled;
                        navMeshAgent.enabled = false;
                        transform.position = hit.position;
                        navMeshAgent.enabled = wasEnabled;
                        
                        Debug.LogWarning($"[EnemyMovement] {gameObject.name} warped to NavMesh at fallback position: {hit.position}");
                        foundNavMesh = true;
                        StartCoroutine(CheckNavMeshAfterWarp());
                        return;
                    }
                }
            }
        }
        
        // If still not found, log error with helpful message
        if (!foundNavMesh)
        {
            Debug.LogError($"[EnemyMovement] {gameObject.name} could not find NavMesh within 100 units of {currentPos}. " +
                         "The NavMesh may not cover the second room. Please:\n" +
                         "1. Open the Scene view and check if blue NavMesh overlay covers both rooms\n" +
                         "2. Use 'Tools > Rebake NavMesh (Include Both Rooms)' to rebake the NavMesh\n" +
                         "3. Ensure floor geometry in the second room is included in the NavMesh bake");
        }
    }
    
    private System.Collections.IEnumerator CheckNavMeshAfterWarp()
    {
        yield return null; // Wait one frame
        
        if (navMeshAgent != null)
        {
            if (navMeshAgent.isOnNavMesh)
            {
                Debug.Log($"[EnemyMovement] {gameObject.name} is now on NavMesh after warp!");
            }
            else
            {
                Debug.LogError($"[EnemyMovement] {gameObject.name} is still NOT on NavMesh after warp! " +
                             "The NavMesh may not cover this area. Please rebake the NavMesh.");
            }
        }
    }

    // Update is called once per frame.
    void Update()
    {
        if (player != null && navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
            
            // Debug logging for destination and velocity
            if (navMeshAgent.hasPath)
            {
                Vector3 velocity = navMeshAgent.velocity;
                float speed = velocity.magnitude;
                if (speed > 0.1f) // Only log when actually moving
                {
                    Debug.Log($"[EnemyMovement] {gameObject.name} moving to Player at {player.position}, velocity: {speed:F2} m/s, destination: {navMeshAgent.destination}");
                }
            }
        }
    }
}