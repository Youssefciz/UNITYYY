using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AutoFixOnImport : AssetPostprocessor
    {
        private static bool hasShownDialog = false;

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (hasShownDialog)
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            foreach (string asset in importedAssets)
            {
                if (asset.Contains("README_FIX_ERRORS.txt") || 
                    asset.Contains("EmergencyPackageFix.cs"))
                {
                    EditorApplication.delayCall += ShowFixDialog;
                    hasShownDialog = true;
                    break;
                }
            }
        }

        private static void ShowFixDialog()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += ShowFixDialog;
                return;
            }

            bool hasErrors = CheckForCompilationErrors();
            
            if (hasErrors)
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    "Package Errors Detected!",
                    "Your project has corrupted Unity packages causing compilation errors.\n\n" +
                    "I've created automated fix scripts for you:\n\n" +
                    "• Emergency Fix - Fully automated (closes and reopens Unity)\n" +
                    "• Package Fix - Removes and reinstalls packages\n\n" +
                    "What would you like to do?",
                    "Show Fix Instructions",
                    "I'll Fix It Manually",
                    "Try Emergency Fix Now");

                switch (choice)
                {
                    case 0:
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/README_FIX_ERRORS.txt");
                        EditorGUIUtility.PingObject(Selection.activeObject);
                        Debug.Log("[Auto Fix] Please read README_FIX_ERRORS.txt for fix instructions!");
                        break;
                    case 1:
                        Debug.Log("[Auto Fix] Manual fix: Close Unity, delete Library folder, reopen Unity.");
                        break;
                    case 2:
                        TryEmergencyFix();
                        break;
                }
            }
            else
            {
                Debug.Log("[Auto Fix] No compilation errors detected. Checking MCP setup...");
                EditorApplication.delayCall += CheckMCPSetup;
            }
        }

        private static bool CheckForCompilationErrors()
        {
            var assemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            return EditorUtility.scriptCompilationFailed;
        }

        private static void TryEmergencyFix()
        {
            try
            {
                var emergencyFixType = System.Type.GetType("Editor.EmergencyPackageFix,Assembly-CSharp-Editor");
                if (emergencyFixType != null)
                {
                    var method = emergencyFixType.GetMethod("EmergencyFix", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                    else
                    {
                        Debug.LogWarning("[Auto Fix] Emergency fix method not found. Scripts may not be compiled yet.");
                        ShowManualInstructions();
                    }
                }
                else
                {
                    Debug.LogWarning("[Auto Fix] Emergency fix script not compiled yet.");
                    ShowManualInstructions();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Auto Fix] Failed to execute emergency fix: {ex.Message}");
                ShowManualInstructions();
            }
        }

        private static void ShowManualInstructions()
        {
            EditorUtility.DisplayDialog(
                "Manual Fix Required",
                "The automated fix cannot run due to compilation errors.\n\n" +
                "Please do this manually:\n\n" +
                "1. Close Unity completely\n" +
                "2. Delete the 'Library' folder in your project\n" +
                "3. Reopen Unity\n\n" +
                "After that, the automated tools will work!",
                "OK");
        }

        private static void CheckMCPSetup()
        {
            bool shouldOpenMCP = !EditorPrefs.GetBool("MCPForUnity.SetupCompleted", false) &&
                                 !EditorPrefs.GetBool("MCPForUnity.SetupDismissed", false);

            if (shouldOpenMCP)
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    "MCP for Unity Setup",
                    "Your packages are fixed!\n\n" +
                    "Now you need to setup MCP for Unity to access the tools.\n\n" +
                    "Would you like to open the MCP setup window?",
                    "Yes, Open MCP Window",
                    "Not Now",
                    "More Info");

                switch (choice)
                {
                    case 0:
                        EditorApplication.ExecuteMenuItem("Window/MCP for Unity");
                        Debug.Log("[Auto Fix] MCP window opened. Click 'Auto-Setup' to configure!");
                        break;
                    case 1:
                        Debug.Log("[Auto Fix] You can open MCP window later from: Window > MCP for Unity");
                        break;
                    case 2:
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/FIX_INSTRUCTIONS.md");
                        EditorGUIUtility.PingObject(Selection.activeObject);
                        Debug.Log("[Auto Fix] See FIX_INSTRUCTIONS.md for MCP setup details!");
                        break;
                }
            }
        }
    }
}
