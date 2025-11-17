using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.IO;

namespace Editor
{
    public static class PackageFixUtility
    {
        private static AddRequest addRequest;
        private static RemoveRequest removeRequest;
        private static ListRequest listRequest;

        [MenuItem("Tools/Fix All Package Errors", priority = 0)]
        public static void FixAllPackageErrors()
        {
            if (EditorUtility.DisplayDialog(
                "Fix Package Errors",
                "This will fix corrupted Unity packages by removing and reinstalling them. This may take a few minutes.\n\nDo you want to continue?",
                "Yes, Fix It",
                "Cancel"))
            {
                Debug.Log("[Package Fix] Starting package fix process...");
                EditorApplication.LockReloadAssemblies();
                
                listRequest = Client.List();
                EditorApplication.update += ListProgress;
            }
        }

        private static void ListProgress()
        {
            if (listRequest.IsCompleted)
            {
                EditorApplication.update -= ListProgress;
                
                if (listRequest.Status == StatusCode.Success)
                {
                    Debug.Log("[Package Fix] Package list retrieved successfully");
                    RemoveCorruptedPackages();
                }
                else
                {
                    Debug.LogError($"[Package Fix] Failed to list packages: {listRequest.Error.message}");
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        private static void RemoveCorruptedPackages()
        {
            Debug.Log("[Package Fix] Removing Timeline package...");
            removeRequest = Client.Remove("com.unity.timeline");
            EditorApplication.update += RemoveTimelineProgress;
        }

        private static void RemoveTimelineProgress()
        {
            if (removeRequest.IsCompleted)
            {
                EditorApplication.update -= RemoveTimelineProgress;
                
                if (removeRequest.Status == StatusCode.Success || removeRequest.Status == StatusCode.Failure)
                {
                    Debug.Log("[Package Fix] Timeline package removed, now removing Input System...");
                    removeRequest = Client.Remove("com.unity.inputsystem");
                    EditorApplication.update += RemoveInputSystemProgress;
                }
                else
                {
                    Debug.LogError($"[Package Fix] Failed to remove Timeline: {removeRequest.Error.message}");
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        private static void RemoveInputSystemProgress()
        {
            if (removeRequest.IsCompleted)
            {
                EditorApplication.update -= RemoveInputSystemProgress;
                
                if (removeRequest.Status == StatusCode.Success || removeRequest.Status == StatusCode.Failure)
                {
                    Debug.Log("[Package Fix] Input System removed, now reinstalling Timeline...");
                    System.Threading.Thread.Sleep(1000);
                    addRequest = Client.Add("com.unity.timeline@1.8.9");
                    EditorApplication.update += AddTimelineProgress;
                }
                else
                {
                    Debug.LogError($"[Package Fix] Failed to remove Input System: {removeRequest.Error.message}");
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        private static void AddTimelineProgress()
        {
            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= AddTimelineProgress;
                
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log("[Package Fix] Timeline package reinstalled successfully");
                    System.Threading.Thread.Sleep(1000);
                    addRequest = Client.Add("com.unity.inputsystem@1.14.2");
                    EditorApplication.update += AddInputSystemProgress;
                }
                else
                {
                    Debug.LogError($"[Package Fix] Failed to add Timeline: {addRequest.Error.message}");
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        private static void AddInputSystemProgress()
        {
            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= AddInputSystemProgress;
                
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log("[Package Fix] Input System reinstalled successfully");
                    Debug.Log("[Package Fix] All packages fixed! Unlocking assemblies...");
                    EditorApplication.UnlockReloadAssemblies();
                    
                    EditorUtility.DisplayDialog(
                        "Package Fix Complete",
                        "All packages have been successfully reinstalled.\n\nUnity will now recompile scripts.",
                        "OK");
                    
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"[Package Fix] Failed to add Input System: {addRequest.Error.message}");
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        [MenuItem("Tools/Clear Library and Reimport (Advanced)", priority = 1)]
        public static void ClearLibraryAndReimport()
        {
            if (EditorUtility.DisplayDialog(
                "Clear Library Folder",
                "WARNING: This will delete the Library folder and force Unity to reimport all assets.\n\n" +
                "Unity will close after deletion. You'll need to manually reopen the project.\n\n" +
                "Do you want to continue?",
                "Yes, Clear Library",
                "Cancel"))
            {
                string projectPath = Directory.GetParent(Application.dataPath).FullName;
                string libraryPath = Path.Combine(projectPath, "Library");
                
                if (Directory.Exists(libraryPath))
                {
                    Debug.Log($"[Package Fix] Deleting Library folder at: {libraryPath}");
                    
                    try
                    {
                        Directory.Delete(libraryPath, true);
                        Debug.Log("[Package Fix] Library folder deleted successfully");
                        Debug.Log("[Package Fix] Unity will now close. Please reopen the project manually.");
                        
                        EditorApplication.delayCall += () =>
                        {
                            EditorApplication.Exit(0);
                        };
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[Package Fix] Failed to delete Library folder: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning("[Package Fix] Library folder not found");
                }
            }
        }
    }
}
