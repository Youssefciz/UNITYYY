using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

namespace Editor
{
    public static class EmergencyPackageFix
    {
        [MenuItem("Tools/Emergency Fix - Restart Unity After Package Repair", priority = 0)]
        public static void EmergencyFix()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Emergency Package Fix",
                "This emergency fix will:\n\n" +
                "1. Create a script to delete Library folder\n" +
                "2. Close Unity\n" +
                "3. Execute the script\n" +
                "4. Reopen Unity\n\n" +
                "Your project will be automatically repaired.\n\n" +
                "IMPORTANT: Save your scene before continuing!\n\n" +
                "Do you want to proceed?",
                "Yes, Fix Now",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string libraryPath = Path.Combine(projectPath, "Library");
            string unityPath = EditorApplication.applicationPath;
            string projectArgument = $"-projectPath \"{projectPath}\"";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ExecuteWindowsFix(projectPath, libraryPath, unityPath, projectArgument);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                ExecuteMacFix(projectPath, libraryPath, unityPath, projectArgument);
            }
            else if (Application.platform == RuntimePlatform.LinuxEditor)
            {
                ExecuteLinuxFix(projectPath, libraryPath, unityPath, projectArgument);
            }
        }

        private static void ExecuteWindowsFix(string projectPath, string libraryPath, string unityPath, string projectArgument)
        {
            string batchFilePath = Path.Combine(projectPath, "FixPackages.bat");
            string batchContent = $@"@echo off
echo Unity Package Emergency Fix
echo ============================
echo.
echo Waiting for Unity to close...
timeout /t 3 /nobreak >nul
echo.
echo Deleting Library folder...
if exist ""{libraryPath}"" (
    rmdir /s /q ""{libraryPath}""
    echo Library folder deleted successfully!
) else (
    echo Library folder not found.
)
echo.
echo Waiting before reopening Unity...
timeout /t 2 /nobreak >nul
echo.
echo Reopening Unity...
start """" ""{unityPath}"" {projectArgument}
echo.
echo Done! Unity should reopen and reimport all assets.
echo This window will close in 5 seconds...
timeout /t 5 /nobreak >nul
del ""%~f0""
";
            ExecuteFix(batchFilePath, batchContent);
        }

        private static void ExecuteMacFix(string projectPath, string libraryPath, string unityPath, string projectArgument)
        {
            string scriptFilePath = Path.Combine(projectPath, "FixPackages.sh");
            string scriptContent = $@"#!/bin/bash
echo ""Unity Package Emergency Fix""
echo ""============================""
echo """"
echo ""Waiting for Unity to close...""
sleep 3
echo """"
echo ""Deleting Library folder...""
if [ -d ""{libraryPath}"" ]; then
    rm -rf ""{libraryPath}""
    echo ""Library folder deleted successfully!""
else
    echo ""Library folder not found.""
fi
echo """"
echo ""Waiting before reopening Unity...""
sleep 2
echo """"
echo ""Reopening Unity...""
open -a ""{unityPath}"" --args {projectArgument}
echo """"
echo ""Done! Unity should reopen and reimport all assets.""
sleep 5
rm ""$0""
";
            ExecuteFix(scriptFilePath, scriptContent, true);
        }

        private static void ExecuteLinuxFix(string projectPath, string libraryPath, string unityPath, string projectArgument)
        {
            string scriptFilePath = Path.Combine(projectPath, "FixPackages.sh");
            string scriptContent = $@"#!/bin/bash
echo ""Unity Package Emergency Fix""
echo ""============================""
echo """"
echo ""Waiting for Unity to close...""
sleep 3
echo """"
echo ""Deleting Library folder...""
if [ -d ""{libraryPath}"" ]; then
    rm -rf ""{libraryPath}""
    echo ""Library folder deleted successfully!""
else
    echo ""Library folder not found.""
fi
echo """"
echo ""Waiting before reopening Unity...""
sleep 2
echo """"
echo ""Reopening Unity...""
""{unityPath}"" {projectArgument} &
echo """"
echo ""Done! Unity should reopen and reimport all assets.""
sleep 5
rm ""$0""
";
            ExecuteFix(scriptFilePath, scriptContent, true);
        }

        private static void ExecuteFix(string scriptPath, string scriptContent, bool makeExecutable = false)
        {
            try
            {
                File.WriteAllText(scriptPath, scriptContent);
                
                if (makeExecutable && (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
                {
                    ProcessStartInfo chmodInfo = new ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = $"+x \"{scriptPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(chmodInfo)?.WaitForExit();
                }

                UnityEngine.Debug.Log($"[Emergency Fix] Script created at: {scriptPath}");
                UnityEngine.Debug.Log("[Emergency Fix] Unity will close in 2 seconds...");

                EditorUtility.DisplayDialog(
                    "Emergency Fix Started",
                    "Unity will now close and repair itself.\n\n" +
                    "Please wait for Unity to reopen automatically.\n\n" +
                    "This may take 5-10 minutes for the first reimport.",
                    "OK");

                EditorApplication.delayCall += () =>
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = scriptPath,
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };

                    Process.Start(processInfo);
                    EditorApplication.Exit(0);
                };
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[Emergency Fix] Failed to create script: {ex.Message}");
                EditorUtility.DisplayDialog(
                    "Emergency Fix Failed",
                    $"Failed to create repair script:\n\n{ex.Message}\n\n" +
                    "Please manually close Unity, delete the Library folder, and reopen the project.",
                    "OK");
            }
        }
    }
}
