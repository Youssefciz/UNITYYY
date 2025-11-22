using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

/// <summary>
/// Editor script to immediately fix Skeleton_death configuration and NavMesh placement.
/// </summary>
public class FixSkeletonNow
{
    [MenuItem("Tools/Fix Skeleton_death Now")]
    public static void FixSkeleton()
    {
        // Find Skeleton_death
        GameObject skeleton = GameObject.Find("Skeleton_death");
        if (skeleton == null)
        {
            EditorUtility.DisplayDialog("Error", "Skeleton_death not found in scene!", "OK");
            return;
        }
        
        // Find Enemy1
        GameObject enemy1 = GameObject.Find("Enemy1");
        if (enemy1 == null)
        {
            EditorUtility.DisplayDialog("Error", "Enemy1 not found in scene!", "OK");
            return;
        }
        
        Undo.RecordObject(skeleton, "Fix Skeleton_death Configuration");
        
        // Get Enemy1 components
        Rigidbody enemy1Rb = enemy1.GetComponent<Rigidbody>();
        NavMeshAgent enemy1Nav = enemy1.GetComponent<NavMeshAgent>();
        EnemyMovement enemy1Movement = enemy1.GetComponent<EnemyMovement>();
        CapsuleCollider enemy1Collider = enemy1.GetComponent<CapsuleCollider>();
        
        // Fix CapsuleCollider
        CapsuleCollider skeletonCollider = skeleton.GetComponent<CapsuleCollider>();
        if (skeletonCollider != null && enemy1Collider != null)
        {
            skeletonCollider.height = enemy1Collider.height;
            skeletonCollider.radius = enemy1Collider.radius;
            Debug.Log("Fixed CapsuleCollider height to " + skeletonCollider.height);
        }
        
        // Fix Rigidbody
        Rigidbody skeletonRb = skeleton.GetComponent<Rigidbody>();
        if (skeletonRb != null && enemy1Rb != null)
        {
            skeletonRb.isKinematic = enemy1Rb.isKinematic;
            skeletonRb.useGravity = enemy1Rb.useGravity;
            skeletonRb.freezeRotation = enemy1Rb.freezeRotation;
            skeletonRb.constraints = enemy1Rb.constraints;
            Debug.Log("Fixed Rigidbody - isKinematic: " + skeletonRb.isKinematic + ", useGravity: " + skeletonRb.useGravity);
        }
        
        // Fix NavMeshAgent
        NavMeshAgent skeletonNav = skeleton.GetComponent<NavMeshAgent>();
        if (skeletonNav != null && enemy1Nav != null)
        {
            skeletonNav.speed = enemy1Nav.speed;
            skeletonNav.stoppingDistance = enemy1Nav.stoppingDistance;
            skeletonNav.angularSpeed = enemy1Nav.angularSpeed;
            skeletonNav.acceleration = enemy1Nav.acceleration;
            skeletonNav.radius = enemy1Nav.radius;
            skeletonNav.height = enemy1Nav.height;
            Debug.Log("Fixed NavMeshAgent - speed: " + skeletonNav.speed + ", stoppingDistance: " + skeletonNav.stoppingDistance);
        }
        
        // Fix EnemyMovement player reference
        EnemyMovement skeletonMovement = skeleton.GetComponent<EnemyMovement>();
        if (skeletonMovement != null)
        {
            if (enemy1Movement != null && enemy1Movement.player != null)
            {
                skeletonMovement.player = enemy1Movement.player;
                Debug.Log("Set Player reference from Enemy1");
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    skeletonMovement.player = player.transform;
                    Debug.Log("Set Player reference (auto-found)");
                }
            }
        }
        
        // Try to warp to NavMesh
        Vector3 currentPos = skeleton.transform.position;
        NavMeshHit hit;
        
        bool foundNavMesh = false;
        string message = "";
        
        // Try current position first
        if (NavMesh.SamplePosition(currentPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            skeleton.transform.position = hit.position;
            foundNavMesh = true;
            message = $"Warped Skeleton_death to NavMesh at {hit.position}";
        }
        else
        {
            // Try larger radius
            if (NavMesh.SamplePosition(currentPos, out hit, 20.0f, NavMesh.AllAreas))
            {
                skeleton.transform.position = hit.position;
                foundNavMesh = true;
                message = $"Warped Skeleton_death to NavMesh (found within 20 units) at {hit.position}";
            }
            else
            {
                // Try near Enemy1's area
                Vector3 enemy1Pos = enemy1.transform.position;
                if (NavMesh.SamplePosition(enemy1Pos, out hit, 5.0f, NavMesh.AllAreas))
                {
                    // Move to second room but on NavMesh
                    Vector3 secondRoomPos = new Vector3(hit.position.x, hit.position.y, hit.position.z + 20f);
                    if (NavMesh.SamplePosition(secondRoomPos, out hit, 10.0f, NavMesh.AllAreas))
                    {
                        skeleton.transform.position = hit.position;
                        foundNavMesh = true;
                        message = $"Moved Skeleton_death to NavMesh in second room at {hit.position}";
                    }
                }
            }
        }
        
        if (!foundNavMesh)
        {
            message = $"ERROR: Could not find NavMesh near Skeleton_death at {currentPos}.\n\n" +
                     "The NavMesh may not cover the second room.\n" +
                     "Please ensure:\n" +
                     "1. The NavMesh (blue overlay) is visible in the Scene view covering both rooms\n" +
                     "2. The floor in the second room is included in the NavMesh bake\n" +
                     "3. Try rebaking the NavMesh with 'Tools > Rebake NavMesh (Include Both Rooms)'";
        }
        
        EditorUtility.SetDirty(skeleton);
        EditorUtility.DisplayDialog("Fix Skeleton_death", 
            "Configuration applied!\n\n" +
            "Fixed:\n" +
            "- CapsuleCollider height\n" +
            "- Rigidbody (Kinematic, No Gravity)\n" +
            "- NavMeshAgent (Speed, Stopping Distance)\n" +
            "- EnemyMovement Player reference\n\n" +
            message, "OK");
        
        Debug.Log("FixSkeletonNow: " + message);
    }
}

