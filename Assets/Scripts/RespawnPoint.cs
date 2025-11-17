using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Tooltip("The order/priority of this respawn point (lower numbers = earlier checkpoints)")]
    public int checkpointOrder = 0;
    
    [Tooltip("Visual indicator when this is the active respawn point")]
    public GameObject visualIndicator;
    
    private bool isActiveRespawnPoint = false;
    
    void Start()
    {
        // Hide visual indicator by default
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(false);
        }
    }
    
    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
    
    public Quaternion GetRespawnRotation()
    {
        return transform.rotation;
    }
    
    public void SetAsActiveRespawnPoint(bool active)
    {
        isActiveRespawnPoint = active;
        
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(active);
        }
    }
    
    // Called when player touches this respawn point
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetRespawnPoint(this);
            }
        }
    }
}
