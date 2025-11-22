using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetupSkeletonEnemy))]
public class SetupSkeletonEnemyEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SetupSkeletonEnemy setupScript = (SetupSkeletonEnemy)target;
        
        if (GUILayout.Button("Setup Skeleton Enemy Now"))
        {
            setupScript.SetupSkeleton();
        }
    }
}

#if UNITY_EDITOR
[InitializeOnLoad]
public class AutoSetupSkeletonEnemy
{
    static AutoSetupSkeletonEnemy()
    {
        EditorApplication.delayCall += TrySetupSkeleton;
    }
    
    static void TrySetupSkeleton()
    {
        // Only run in the "ok" scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ok")
        {
            return;
        }
        
        // Check if skeleton is already configured
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool skeletonConfigured = false;
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy.name.Contains("Skeleton") || enemy.name.Contains("skeleton") || 
                enemy.name.Contains("110"))
            {
                // Check if it has EnemyMovement
                if (enemy.GetComponent<EnemyMovement>() != null)
                {
                    skeletonConfigured = true;
                    break;
                }
            }
        }
        
        if (!skeletonConfigured)
        {
            // Try to find and setup skeleton
            GameObject setupObj = GameObject.Find("SkeletonSetupHelper");
            if (setupObj == null)
            {
                setupObj = new GameObject("SkeletonSetupHelper");
                SetupSkeletonEnemy setupScript = setupObj.AddComponent<SetupSkeletonEnemy>();
                setupScript.SetupSkeleton();
            }
        }
    }
}
#endif

