using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The player to follow")]
    public Transform player;
    
    [Tooltip("Offset from the player (X, Y, Z)")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);
    
    [Tooltip("How fast the camera follows (higher = faster)")]
    public float followSpeed = 5f;
    
    [Tooltip("Should the camera look at the player?")]
    public bool lookAtPlayer = true;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: Could not find player with tag 'Player'");
            }
        }
    }
    
    void LateUpdate()
    {
        if (player == null)
            return;
        
        // Calculate target position
        Vector3 targetPosition = player.position + offset;
        
        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Look at player if enabled
        if (lookAtPlayer)
        {
            transform.LookAt(player.position);
        }
    }
}
