using UnityEngine;

public class KillFloor : MonoBehaviour
{
    [Tooltip("The Y position below which the player will be killed/respawned")]
    public float killYPosition = -10f;
    
    [Tooltip("Tag of the player object")]
    public string playerTag = "Player";
    
    private PlayerController playerController;
    
    void Start()
    {
        // Cache player reference at start
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }
    
    private void Update()
    {
        // If player reference is lost, try to find it again
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
        }
        
        // Check if player has fallen below kill floor
        if (playerController != null && playerController.transform.position.y < killYPosition)
        {
            // Trigger respawn
            playerController.Respawn();
        }
    }
    
    // Alternative: Use OnTriggerEnter for collision-based kill floor
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
            }
        }
    }
}
