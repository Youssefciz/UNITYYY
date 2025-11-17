using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class MCPAutoSetupTrigger
    {
        static MCPAutoSetupTrigger()
        {
            EditorApplication.delayCall += TriggerMCPSetup;
        }

        private static void TriggerMCPSetup()
        {
            EditorApplication.delayCall -= TriggerMCPSetup;
            // Disabled auto-trigger to prevent errors
            // Use Window > MCP for Unity manually if needed
        }

        [MenuItem("Tools/Open MCP for Unity Window", priority = 10)]
        public static void OpenMCPWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/MCP for Unity");
            
            Debug.Log("[MCP Setup] MCP for Unity window opened. Please click 'Auto-Setup' button in the window to configure MCP tools.");
            Debug.Log("[MCP Setup] If you don't see the MCP window, go to: Window > MCP for Unity");
        }
    }
}
