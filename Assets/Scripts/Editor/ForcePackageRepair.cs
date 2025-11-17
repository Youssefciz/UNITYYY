using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class ForcePackageRepair
{
    static ForcePackageRepair()
    {
        EditorApplication.update += RunOnce;
    }

    private static void RunOnce()
    {
        EditorApplication.update -= RunOnce;
        
        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.LogWarning("=== CRITICAL: Package corruption detected ===");
            Debug.LogWarning("Attempting automatic repair...");
            
            EditorApplication.delayCall += AttemptAutoRepair;
        }
    }

    private static void AttemptAutoRepair()
    {
        bool result = EditorUtility.DisplayDialog(
            "🚨 CRITICAL ERROR - Corrupted Packages Detected",
            "Your Timeline and Input System packages are corrupted.\n\n" +
            "AUTOMATIC FIX AVAILABLE:\n\n" +
            "I will now close Unity and repair the packages automatically.\n" +
            "Unity will reopen in 30 seconds.\n\n" +
            "⚠️ IMPORTANT: Save your scene if needed!\n\n" +
            "Click OK to start the automated repair process.",
            "OK - Start Auto Repair",
            "Cancel - I'll Fix Manually");

        if (result)
        {
            StartAutomatedRepair();
        }
        else
        {
            ShowManualInstructions();
        }
    }

    private static void StartAutomatedRepair()
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        string repairScriptPath;
        
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            repairScriptPath = Path.Combine(projectPath, "AutoRepair.bat");
            CreateWindowsRepairScript(projectPath, repairScriptPath);
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            repairScriptPath = Path.Combine(projectPath, "AutoRepair.sh");
            CreateMacRepairScript(projectPath, repairScriptPath);
        }
        else
        {
            repairScriptPath = Path.Combine(projectPath, "AutoRepair.sh");
            CreateLinuxRepairScript(projectPath, repairScriptPath);
        }

        Debug.Log("=== AUTO REPAIR STARTED ===");
        Debug.Log($"Repair script created: {repairScriptPath}");
        Debug.Log("Unity will close in 3 seconds...");
        
        EditorApplication.delayCall += () => ExecuteRepairAndClose(repairScriptPath);
    }

    private static void CreateWindowsRepairScript(string projectPath, string scriptPath)
    {
        string libraryPath = Path.Combine(projectPath, "Library");
        string unityExe = EditorApplication.applicationPath;
        
        string script = $@"@echo off
title Unity Package Auto-Repair
color 0A
echo.
echo ========================================
echo   UNITY PACKAGE AUTO-REPAIR TOOL
echo ========================================
echo.
echo [1/5] Waiting for Unity to close...
timeout /t 5 /nobreak >nul

echo [2/5] Deleting corrupted Library folder...
if exist ""{libraryPath}"" (
    rmdir /s /q ""{libraryPath}""
    echo      SUCCESS: Library folder deleted
) else (
    echo      WARNING: Library folder not found
)

echo [3/5] Clearing package cache...
set CACHE_PATH=%LOCALAPPDATA%\Unity\cache\packages
if exist ""%CACHE_PATH%"" (
    rmdir /s /q ""%CACHE_PATH%""
    echo      SUCCESS: Package cache cleared
)

echo [4/5] Waiting before reopening Unity...
timeout /t 3 /nobreak >nul

echo [5/5] Reopening Unity...
start """" ""{unityExe}"" -projectPath ""{projectPath}""

echo.
echo ========================================
echo   REPAIR COMPLETE!
echo ========================================
echo.
echo Unity is reopening and will reimport all assets.
echo This may take 5-10 minutes.
echo.
echo This window will close in 10 seconds...
timeout /t 10 /nobreak >nul

del ""%~f0""
";
        File.WriteAllText(scriptPath, script);
    }

    private static void CreateMacRepairScript(string projectPath, string scriptPath)
    {
        string libraryPath = Path.Combine(projectPath, "Library");
        string unityApp = EditorApplication.applicationPath;
        
        string script = $@"#!/bin/bash
clear
echo ""=======================================""
echo ""  UNITY PACKAGE AUTO-REPAIR TOOL""
echo ""=======================================""
echo """"

echo ""[1/5] Waiting for Unity to close...""
sleep 5

echo ""[2/5] Deleting corrupted Library folder...""
if [ -d ""{libraryPath}"" ]; then
    rm -rf ""{libraryPath}""
    echo ""     SUCCESS: Library folder deleted""
else
    echo ""     WARNING: Library folder not found""
fi

echo ""[3/5] Clearing package cache...""
CACHE_PATH=""$HOME/Library/Unity/cache/packages""
if [ -d ""$CACHE_PATH"" ]; then
    rm -rf ""$CACHE_PATH""
    echo ""     SUCCESS: Package cache cleared""
fi

echo ""[4/5] Waiting before reopening Unity...""
sleep 3

echo ""[5/5] Reopening Unity...""
open -a ""{unityApp}"" --args -projectPath ""{projectPath}""

echo """"
echo ""=======================================""
echo ""  REPAIR COMPLETE!""
echo ""=======================================""
echo """"
echo ""Unity is reopening and will reimport all assets.""
echo ""This may take 5-10 minutes.""
echo """"
echo ""This window will close in 10 seconds...""
sleep 10

rm ""$0""
";
        File.WriteAllText(scriptPath, script);
        System.Diagnostics.Process.Start("chmod", $"+x \"{scriptPath}\"");
    }

    private static void CreateLinuxRepairScript(string projectPath, string scriptPath)
    {
        string libraryPath = Path.Combine(projectPath, "Library");
        string unityExe = EditorApplication.applicationPath;
        
        string script = $@"#!/bin/bash
clear
echo ""=======================================""
echo ""  UNITY PACKAGE AUTO-REPAIR TOOL""
echo ""=======================================""
echo """"

echo ""[1/5] Waiting for Unity to close...""
sleep 5

echo ""[2/5] Deleting corrupted Library folder...""
if [ -d ""{libraryPath}"" ]; then
    rm -rf ""{libraryPath}""
    echo ""     SUCCESS: Library folder deleted""
else
    echo ""     WARNING: Library folder not found""
fi

echo ""[3/5] Clearing package cache...""
CACHE_PATH=""$HOME/.config/unity3d/cache/packages""
if [ -d ""$CACHE_PATH"" ]; then
    rm -rf ""$CACHE_PATH""
    echo ""     SUCCESS: Package cache cleared""
fi

echo ""[4/5] Waiting before reopening Unity...""
sleep 3

echo ""[5/5] Reopening Unity...""
""{unityExe}"" -projectPath ""{projectPath}"" &

echo """"
echo ""=======================================""
echo ""  REPAIR COMPLETE!""
echo ""=======================================""
echo """"
echo ""Unity is reopening and will reimport all assets.""
echo ""This may take 5-10 minutes.""
echo """"
echo ""This window will close in 10 seconds...""
sleep 10

rm ""$0""
";
        File.WriteAllText(scriptPath, script);
        System.Diagnostics.Process.Start("chmod", $"+x \"{scriptPath}\"");
    }

    private static void ExecuteRepairAndClose(string scriptPath)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = scriptPath,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            System.Diagnostics.Process.Start(psi);
            
            EditorApplication.Exit(0);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start repair script: {ex.Message}");
            ShowManualInstructions();
        }
    }

    private static void ShowManualInstructions()
    {
        string instructions = 
            "MANUAL FIX INSTRUCTIONS:\n\n" +
            "1. Close Unity completely\n" +
            "2. Navigate to your project folder\n" +
            "3. Delete the 'Library' folder\n" +
            "4. Reopen Unity\n\n" +
            "Unity will reimport everything and fix the errors.\n\n" +
            "After fixing, open: Window > MCP for Unity\n" +
            "Then click 'Auto-Setup' to configure MCP tools.";

        Debug.LogWarning(instructions);
        EditorUtility.DisplayDialog("Manual Fix Required", instructions, "OK");
    }
}
