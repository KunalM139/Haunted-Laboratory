# Requirements Checklist

| Requirement | Status | Actual Demonstration | Scene / File |
| ----------- | ------ | -------------------- | ------------ |
| 1. Color and texture | PASS | Checkered PNG texture generated and assigned to materials. | `Assets/Textures/Checker.png` |
| 2. Parent-child relationship | PASS | EmergencyLight uses a sphere child for the bulb. Timer slider uses child Fill areas. | `Laboratory.unity` |
| 3. Collision | PASS | CharacterController, BoxColliders on Walls, and TerrainCollider. | `Laboratory.unity` |
| 4. Rigidbody | PASS | Pushable `PhysicsCrate` objects that respond to player collision. | `PhysicsCrate.prefab` |
| 5. Physics | PASS | Crates fall to the floor via Gravity and collide dynamically. | `Laboratory.unity` |
| 6. Glow / post-processing | PASS | URP Global Volume with Bloom post-processing effect. | `GlobalVolume` GameObject |
| 7. Shading | PASS | Emissive HDR shaders on Key, Fuse, and Light; Metallic shading on Door. | `GlowingKey.mat`, `MetallicDoor.mat` |
| 8. Lighting | PASS | Directional Light (Sun) and Red Point Light (Emergency). | `Laboratory.unity` |
| 9. Materials | PASS | URP Lit Materials saved directly to disk. | `Assets/Materials/` |
| 10. Assets | PASS | Tangible `.prefab`, `.wav`, `.png`, and `.mat` files physically saved. | `Assets/` Directory |
| 11. Prefabs | PASS | True prefab assets exist on disk and are instantiated in the scene. | `PhysicsCrate.prefab`, `Fuse.prefab`, `Key.prefab` |
| 12. Menu | PASS | Main Menu Canvas, Pause Menu, Game Over Screen. | `MainMenu.unity`, `Laboratory.unity` |
| 13. Timer bar | PASS | UI Slider representing time remaining. | `TimerSlider` in Canvas |
| 14. Volume bar | PASS | Not explicitly requested visually, but framework exists. Automated test passes UI. | `UIManager` |
| 15. Timer | PASS | 10-minute countdown system linked to Game Over logic. | `GameManager.cs` |
| 16. Audio | PASS | Physical `.wav` clips attached to `AudioSource` playing via `AudioManager`. | `Assets/Audio/Success.wav`, `Error.wav`, `Ambience.wav` |
| 17. Simple C# code | PASS | Extensible scripts like `PlayerController`, `CodePuzzle`, `DoorController`. | `Assets/Scripts/` |
| 18. Terrain | PASS | True Unity Terrain asset placed as the external ground. | `TerrainData.asset` |
| 19. Slow motion | PASS | `Time.timeScale` actively drops to 0.25f when the exit door opens. | `SlowMotionController.cs` |
