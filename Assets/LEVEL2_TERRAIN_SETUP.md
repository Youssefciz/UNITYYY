# Level2_Terrain Scene Setup Guide

## ✅ Completed Automatically

1. **Scene Created**: `Assets/Scenes/Level2_Terrain.unity/Level2_Terrain.unity`
2. **Terrain GameObject**: Created and ready for configuration
3. **Directional Light**: Added to the scene
4. **Build Settings**: Updated to include Level2_Terrain scene
5. **Scene Loading**: PlayerController now loads "Level2_Terrain" after collecting 12 pickups (2 second delay)
6. **Runtime Setup Script**: `TerrainLevel2Setup.cs` will automatically:
   - Position Player on terrain (if Player exists)
   - Create Enemy if missing
   - Bake NavMesh
   - Add environment objects (rocks)
7. **UI Setup**: `LivesUISetup.cs` will automatically create Canvas, LivesText, and GameOverText at runtime

## 🔧 Manual Setup Required (In Unity Editor)

### 1. Configure Terrain
1. Select the **Terrain** GameObject in the scene
2. Use Unity's Terrain Tools to:
   - **Raise/Lower Terrain**: Create hills and valleys using the Paint Terrain tool
   - **Paint Textures**: 
     - Add at least 2 terrain textures (e.g., grass and rock/dirt)
     - Paint them across different areas of the terrain
   - **Paint Details (Grass)**: 
     - Add grass detail textures
     - Paint grass on flatter areas
   - **Place Trees**: 
     - Add tree prefabs or use Unity's built-in trees
     - Place at least a few trees across the terrain

### 2. Add Player to Scene
1. Open the first scene "ok" 
2. Find the Player GameObject
3. Copy it (Ctrl+C)
4. Switch to Level2_Terrain scene
5. Paste it (Ctrl+V)
6. The TerrainLevel2Setup script will position it on the terrain automatically

### 3. Add Enemy to Scene (Optional - will be created automatically if missing)
1. Open the first scene "ok"
2. Find "Enemy1" GameObject
3. Copy it
4. Switch to Level2_Terrain scene  
5. Paste it
6. The TerrainLevel2Setup script will position it on the terrain automatically

**Note**: If Enemy is not added manually, TerrainLevel2Setup will create a basic enemy at runtime.

### 4. Add TerrainLevel2Setup Component
1. Select the **TerrainLevel2Setup** GameObject in the scene
2. In the Inspector, click "Add Component"
3. Search for "TerrainLevel2Setup"
4. Add the component

**Note**: This component will run automatically when the scene loads and set up Player, Enemy, NavMesh, and environment objects.

### 5. Verify UI Setup
The UI (Canvas, LivesText, GameOverText) will be created automatically by `LivesUISetup.cs` at runtime. No manual setup needed.

### 6. Test Scene Loading
1. Open the "ok" scene
2. Enter Play mode
3. Collect all 12 pickups
4. Wait 2 seconds - the scene should load "Level2_Terrain"

## 📋 Scene Hierarchy (Expected)

```
Level2_Terrain
├── Terrain (configured with heights, textures, grass, trees)
├── Directional Light
├── TerrainLevel2Setup (with TerrainLevel2Setup component)
├── Player (added from first scene)
├── Enemy1 (added from first scene or created at runtime)
├── NavMeshSurface (created at runtime)
└── Canvas (created at runtime by LivesUISetup)
    ├── LivesText (top-right corner)
    └── GameOverText (center, initially hidden)
```

## 🎮 Testing Checklist

- [ ] Terrain has visible height variations (hills/valleys)
- [ ] Terrain has at least 2 different textures painted
- [ ] Grass details are visible on terrain
- [ ] Trees are placed on terrain
- [ ] Player spawns on terrain surface
- [ ] Enemy exists and has "Enemy" tag
- [ ] Enemy has NavMeshAgent component
- [ ] Enemy moves toward player (NavMesh working)
- [ ] Enemy can damage player (reduces lives)
- [ ] Lives counter appears in top-right
- [ ] Game Over appears when lives reach 0
- [ ] Pressing R after Game Over restarts Level2_Terrain
- [ ] Scene loads from "ok" after collecting 12 pickups

## 🔍 Troubleshooting

### Enemy not moving
- Check that NavMesh is baked (TerrainLevel2Setup should do this automatically)
- Verify Enemy has NavMeshAgent component
- Check that Enemy is on the NavMesh (blue overlay in Scene view)

### Player falls through terrain
- Ensure Terrain has a Terrain Collider component (should be automatic)
- Check Player's Rigidbody settings

### UI not appearing
- LivesUISetup runs automatically at runtime
- Check Console for errors
- Verify TextMeshPro is imported

### Scene doesn't load
- Verify Build Settings includes both scenes
- Check Console for errors
- Ensure PlayerController script is on Player GameObject

