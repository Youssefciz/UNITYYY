# Fix Instructions for MCP and Package Errors

## Current Issues Detected

### 1. Corrupted Unity Packages
Your project has corrupted Unity packages causing compilation errors:
- **com.unity.timeline** - Missing types: IPropertyPreview, ICurvesOwner, IntervalTree, RuntimeElement, etc.
- **com.unity.inputsystem** - Missing namespaces: Layouts, Controls, Processors, etc.

### 2. MCP Tools Not Visible
The MCP for Unity package is installed but not configured, which is why you don't see the tools.

---

## AUTOMATED FIX (Recommended)

Once the scripts compile, you can use the automated fix tools:

### Step 1: Fix Corrupted Packages
1. In Unity, go to **Tools > Fix All Package Errors**
2. Click "Yes, Fix It" in the dialog
3. Wait for the process to complete (may take 2-5 minutes)
4. Unity will recompile scripts automatically

### Step 2: Setup MCP Tools
1. After packages are fixed, go to **Tools > Open MCP for Unity Window**
2. In the MCP for Unity window, click **Auto-Setup**
3. Follow any prompts to install dependencies (Python, uv, etc.)
4. Click **Start Bridge** if it's not running
5. Your MCP tools will now be visible in your IDE (Cursor, VS Code, etc.)

---

## MANUAL FIX (If automated fix fails)

### Fix 1: Clear Library Folder (Most Effective)

**IMPORTANT: This will close Unity. Save your work first!**

1. Close Unity Editor completely
2. Navigate to your project folder: `ProjectA14`
3. Delete the **Library** folder
4. Delete the **Packages** folder (optional, but recommended)
5. Reopen the project in Unity
6. Unity will reimport everything (this may take 5-10 minutes)
7. All packages should be fixed

### Fix 2: Clear Global Package Cache

If Fix 1 doesn't work:

1. Close all Unity Editors
2. Navigate to: `C:\Users\<YourUsername>\AppData\Local\Unity\cache` (Windows)
   - Or: `~/Library/Unity/cache` (Mac)
   - Or: `~/.config/unity3d/cache` (Linux)
3. Delete the **cache** folder
4. Reopen Unity and let it re-download packages

### Fix 3: Reinstall Packages via Package Manager

1. In Unity, go to **Window > Package Manager**
2. Select **Unity Registry** from the packages dropdown
3. Search for "Timeline" and click **Update** or **Remove** then **Add**
4. Repeat for "Input System"
5. Wait for compilation to complete

---

## Setup MCP for Unity After Fixing Packages

Once compilation errors are gone:

1. Go to **Window > MCP for Unity**
2. Click **Auto-Setup** button
3. If prompted:
   - Install Python if not detected
   - Install `uv` package manager
   - For Claude Code users: install Claude CLI
4. Select your MCP client (Cursor, VS Code, Windsurf, or Claude Code)
5. Click **Auto Configure** for your selected client
6. Click **Start Bridge** if it shows "Stopped"
7. Done! MCP tools should now be available in your IDE

---

## Understanding the MCP Tools

Once configured, MCP provides these tools to your AI assistant:
- **manage_script** - Create, read, and edit C# scripts
- **manage_scene** - Work with Unity scenes
- **manage_gameobject** - Manipulate GameObjects in scenes
- **manage_asset** - Handle Unity assets (prefabs, materials, etc.)
- **manage_editor** - Control Unity Editor operations
- **read_console** - Read Unity Console messages
- **run_tests** - Execute Unity tests
- And more...

These tools allow your AI assistant (like me in Cursor or Claude) to directly interact with your Unity project!

---

## Troubleshooting

### Issue: "uv Not Found" in MCP Window
- **Solution**: Install `uv` package manager:
  - Windows: `pip install uv` or download from https://github.com/astral-sh/uv
  - Mac/Linux: `curl -LsSf https://astral.sh/uv/install.sh | sh`

### Issue: "Python Not Found"
- **Solution**: Install Python 3.8 or later from https://www.python.org/downloads/
- Make sure to check "Add Python to PATH" during installation

### Issue: "Claude CLI Not Found" (Claude Code users only)
- **Solution**: Install Claude Code CLI as per Claude documentation
- Use "Choose Claude Install Location" in MCP window to manually select it

### Issue: Compilation Errors Still Present After Fix
- **Solution**: Try Fix 1 (Clear Library Folder) from the Manual Fix section above

### Issue: MCP Bridge Won't Start
- **Solution**: 
  1. Check Unity Console for error messages
  2. Verify Python and uv are installed correctly
  3. Try "Rebuild MCP Server" in the MCP window
  4. Check that ports 6500 is not in use by another application

---

## Quick Reference

### File Locations
- **MCP Package**: `/Packages/com.coplaydev.unity-mcp`
- **MCP Server**: `/Packages/com.coplaydev.unity-mcp/UnityMcpServer~/src`
- **Fix Scripts**: `/Assets/Scripts/Editor/PackageFixUtility.cs`

### Unity Menu Items Added
- **Tools > Fix All Package Errors** - Automated package fix
- **Tools > Clear Library and Reimport (Advanced)** - Nuclear option
- **Tools > Open MCP for Unity Window** - Open MCP configuration
- **Window > MCP for Unity** - Main MCP window

### Documentation Links
- MCP for Unity GitHub: https://github.com/CoplayDev/unity-mcp
- Fix Guide (Cursor/VS Code): https://github.com/CoplayDev/unity-mcp/wiki/1.-Fix-Unity-MCP-and-Cursor,-VSCode-&-Windsurf
- Fix Guide (Claude Code): https://github.com/CoplayDev/unity-mcp/wiki/2.-Fix-Unity-MCP-and-Claude-Code

---

## What I've Done For You

I've created two automated fix scripts:

1. **PackageFixUtility.cs** - Provides menu items to:
   - Automatically remove and reinstall corrupted packages
   - Clear Library folder and reimport (advanced option)

2. **MCPAutoSetupTrigger.cs** - Provides:
   - Automatic opening of MCP window on Unity startup
   - Menu item to manually open MCP window

**IMPORTANT**: These scripts won't work until the compilation errors are fixed!

---

## Recommended Action Plan

Since you want a fully automated solution with no manual steps:

**The scripts I created will do everything automatically ONCE Unity can compile again.**

To break the current deadlock:

1. **Option A (Fastest)**: Use the automated **Tools > Clear Library and Reimport** menu item once Unity recognizes the scripts
   - This requires Unity to compile the fix scripts first
   - But Unity can't compile because of package errors
   - This is a chicken-and-egg problem

2. **Option B (Most Reliable)**: Manually clear the Library folder ONCE
   - Close Unity
   - Delete the `Library` folder from your project directory
   - Reopen Unity
   - After reimport completes, run **Tools > Fix All Package Errors**
   - Then run **Tools > Open MCP for Unity Window** and click Auto-Setup
   - You'll never have to do manual steps again!

Unfortunately, due to the severity of the package corruption, Unity cannot execute ANY scripts (including my automated fix scripts) until the packages are repaired. This is a fundamental limitation of Unity's compilation system.

**The single manual action required is: Delete the Library folder while Unity is closed.**

After that, all my automated scripts will work and you won't need to do anything manually again.

---

## Need Help?

If you're still having issues:
1. Check the Unity Console for specific error messages
2. Visit the MCP for Unity Discord: https://discord.com/invite/AqfRZQUXqY
3. Review the GitHub wiki: https://github.com/CoplayDev/unity-mcp/wiki

Good luck! 🚀
