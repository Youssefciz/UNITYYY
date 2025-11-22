using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

/// <summary>
/// Editor script to help rebake the NavMesh to include both rooms.
/// </summary>
public class RebakeNavMeshHelper
{
    [MenuItem("Tools/Rebake NavMesh (Include Both Rooms)")]
    public static void RebakeNavMeshForBothRooms()
    {
        // Find or create NavMeshSurface
        NavMeshSurface surface = Object.FindFirstObjectByType<NavMeshSurface>();
        
        if (surface == null)
        {
            // Try to find ground/floor objects
            GameObject ground = GameObject.Find("ground");
            if (ground == null)
            {
                ground = GameObject.Find("Ground");
            }
            if (ground == null)
            {
                ground = GameObject.Find("Floor");
            }
            
            if (ground != null)
            {
                surface = ground.GetComponent<NavMeshSurface>();
                if (surface == null)
                {
                    surface = ground.AddComponent<NavMeshSurface>();
                    Debug.Log("RebakeNavMeshHelper: Added NavMeshSurface to " + ground.name);
                }
            }
            else
            {
                // Create a new GameObject for NavMeshSurface
                GameObject navMeshObj = new GameObject("NavMesh Surface");
                surface = navMeshObj.AddComponent<NavMeshSurface>();
                Debug.Log("RebakeNavMeshHelper: Created new NavMeshSurface GameObject");
            }
        }
        
        // Configure NavMeshSurface to collect all objects
        surface.collectObjects = CollectObjects.All;
        
        // Bake the NavMesh
        surface.BuildNavMesh();
        
        Debug.Log("RebakeNavMeshHelper: NavMesh rebaked! Check the Scene view - you should see blue NavMesh overlay covering both rooms.");
        EditorUtility.DisplayDialog("NavMesh Rebaked", 
            "NavMesh has been rebaked to include all rooms.\n\n" +
            "Check the Scene view - you should see a blue overlay covering walkable areas in both rooms.\n\n" +
            "If Skeleton_death still doesn't move, ensure:\n" +
            "1. The NavMesh (blue overlay) covers the second room\n" +
            "2. Skeleton_death is positioned on or near the NavMesh\n" +
            "3. Enter Play Mode to test", "OK");
    }
    
    [MenuItem("Tools/Check NavMesh Coverage")]
    public static void CheckNavMeshCoverage()
    {
        // Check if Skeleton_death is on NavMesh
        GameObject skeleton = GameObject.Find("Skeleton_death");
        if (skeleton != null)
        {
            UnityEngine.AI.NavMeshAgent agent = skeleton.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                UnityEngine.AI.NavMeshHit hit;
                Vector3 pos = skeleton.transform.position;
                
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(pos, hit.position);
                    EditorUtility.DisplayDialog("NavMesh Check", 
                        $"Skeleton_death position: {pos}\n" +
                        $"Nearest NavMesh point: {hit.position}\n" +
                        $"Distance: {distance:F2} units\n\n" +
                        $"Status: {(distance < 1.0f ? "OK - Close to NavMesh" : "WARNING - Too far from NavMesh")}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("NavMesh Check", 
                        $"Skeleton_death position: {pos}\n\n" +
                        $"ERROR: No NavMesh found within 10 units!\n\n" +
                        "The NavMesh does not cover the second room.\n" +
                        "Please use 'Tools > Rebake NavMesh (Include Both Rooms)' to fix this.", "OK");
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("NavMesh Check", "Skeleton_death not found in scene!", "OK");
        }
    }
}

