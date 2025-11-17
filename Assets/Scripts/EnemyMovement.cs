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
        
        // If player is not assigned, try to find it
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    // Update is called once per frame.
    void Update()
    {
        if (player != null && navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}