using UnityEngine;

/// <summary>
/// Enables the skeleton enemy when the North Wall Left is destroyed.
/// Attach this to any GameObject in the scene (or create a manager GameObject).
/// </summary>
public class EnableSkeletonOnWallDestroy : MonoBehaviour
{
    [Header("Skeleton Enemy Reference")]
    [Tooltip("The skeleton enemy GameObject to enable. If null, will search for it.")]
    public GameObject skeletonEnemy;
    
    [Header("Wall Reference")]
    [Tooltip("The North Wall Left GameObject to monitor. If null, will search for it.")]
    public GameObject northWallLeft;
    
    private bool wallDestroyed = false;
    
    void Start()
    {
        // Find skeleton if not assigned
        if (skeletonEnemy == null)
        {
            skeletonEnemy = FindSkeletonEnemy();
        }
        
        // Find wall if not assigned
        if (northWallLeft == null)
        {
            northWallLeft = GameObject.Find("North Wall Left (1)");
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("Walls/North Wall Left (1)");
            }
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("North Wall Left");
            }
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("Walls/North Wall Left");
            }
        }
        
        // Initially disable skeleton if wall exists
        if (skeletonEnemy != null && northWallLeft != null)
        {
            skeletonEnemy.SetActive(false);
            Debug.Log("EnableSkeletonOnWallDestroy: Skeleton enemy disabled until wall is destroyed.");
        }
        else if (skeletonEnemy != null)
        {
            // Wall doesn't exist, so enable skeleton
            skeletonEnemy.SetActive(true);
            Debug.Log("EnableSkeletonOnWallDestroy: Wall not found, skeleton enabled.");
        }
    }
    
    void Update()
    {
        // Check if wall has been destroyed
        if (!wallDestroyed && northWallLeft == null)
        {
            wallDestroyed = true;
            
            if (skeletonEnemy != null)
            {
                skeletonEnemy.SetActive(true);
                Debug.Log("EnableSkeletonOnWallDestroy: Wall destroyed! Skeleton enemy enabled.");
            }
        }
    }
    
    private GameObject FindSkeletonEnemy()
    {
        // First try to find by exact name
        GameObject skeleton = GameObject.Find("Skeleton_death");
        if (skeleton != null)
        {
            return skeleton;
        }
        
        // Search for skeleton by name patterns
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            string name = obj.name;
            if (name.Contains("Skeleton") || name.Contains("skeleton") || 
                name.Contains("110") || name.Contains("Skeleton_110") ||
                name.Contains("SkeletonEnemy") || name == "Skeleton_death")
            {
                return obj;
            }
        }
        
        return null;
    }
}

