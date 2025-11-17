using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class StartupHelper
    {
        private const string HELPER_SHOWN_KEY = "StartupHelper.DialogShown";
        
        static StartupHelper()
        {
            if (Application.isBatchMode)
                return;

            EditorApplication.delayCall += ShowHelpIfNeeded;
        }

        private static void ShowHelpIfNeeded()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += ShowHelpIfNeeded;
                return;
            }

            if (SessionState.GetBool(HELPER_SHOWN_KEY, false))
                return;

            SessionState.SetBool(HELPER_SHOWN_KEY, true);

            if (EditorUtility.scriptCompilationFailed)
            {
                EditorApplication.delayCall += ShowErrorFixDialog;
            }
        }

        private static void ShowErrorFixDialog()
        {
            int result = EditorUtility.DisplayDialogComplex(
                "🔧 Package Errors Detected - Automated Fix Available!",
                "Your Unity project has corrupted packages causing compilation errors.\n\n" +
                "✅ GOOD NEWS: I've created automated fix scripts!\n\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "CHOOSE YOUR FIX:\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                "🚀 EMERGENCY FIX (Recommended)\n" +
                "   → Fully automated, closes & reopens Unity\n" +
                "   → Menu: Tools > Emergency Fix\n\n" +
                "📦 PACKAGE FIX\n" +
                "   → Reinstalls corrupted packages\n" +
                "   → Menu: Tools > Fix All Package Errors\n\n" +
                "📖 MANUAL FIX\n" +
                "   → Close Unity, delete Library folder, reopen\n" +
                "   → See README_FIX_ERRORS.txt for steps\n\n" +
                "What would you like to do?",
                "Show README",
                "Try Emergency Fix",
                "Open Tools Menu");

            switch (result)
            {
                case 0:
                    HighlightReadme();
                    break;
                case 1:
                    TryRunEmergencyFix();
                    break;
                case 2:
                    EditorApplication.ExecuteMenuItem("Tools/");
                    Debug.Log("[Startup Helper] Check the 'Tools' menu for fix options!");
                    break;
            }
        }

        private static void HighlightReadme()
        {
            var readme = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/README_FIX_ERRORS.txt");
            if (readme != null)
            {
                Selection.activeObject = readme;
                EditorGUIUtility.PingObject(readme);
                Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Debug.Log("📖 Please read README_FIX_ERRORS.txt (now highlighted)");
                Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            }
            else
            {
                var instructions = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/FIX_INSTRUCTIONS.md");
                if (instructions != null)
                {
                    Selection.activeObject = instructions;
                    EditorGUIUtility.PingObject(instructions);
                }
            }
        }

        private static void TryRunEmergencyFix()
        {
            try
            {
                EditorApplication.ExecuteMenuItem("Tools/Emergency Fix - Restart Unity After Package Repair");
            }
            catch
            {
                EditorUtility.DisplayDialog(
                    "Emergency Fix Not Ready",
                    "The Emergency Fix menu item isn't available yet.\n\n" +
                    "This is likely because Unity is still processing the scripts.\n\n" +
                    "Please try again in a few seconds, or use the manual fix:\n" +
                    "1. Close Unity\n" +
                    "2. Delete 'Library' folder\n" +
                    "3. Reopen Unity",
                    "OK");
            }
        }
    }
}
