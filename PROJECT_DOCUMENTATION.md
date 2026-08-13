# Project Documentation
## Haunted Laboratory: VR Escape Room

### Objective
To create a fully functional, immersive 3D escape room in Unity demonstrating 19 specific technical requirements for a college AR/VR/MR course.

### Gameplay Concept
The player wakes up in an abandoned laboratory with failing power. The final exit is locked. Within 10 minutes, the player must:
1. Find the scattered fuse and restore power.
2. Observe clues to solve a keypad code puzzle.
3. Open a secured drawer to collect the final exit key.
4. Unlock the main door and escape before time runs out.

### Software Requirements
- Unity 6000.x or later (URP)
- Input System
- TextMeshPro

### Technical Architecture
The project avoids a monolithic script structure. Logic is distributed into focused components:
- **Game Management**: `GameManager.cs`, `UIManager.cs`, `AudioManager.cs`
- **Player Controller**: `PlayerController.cs` (CharacterController), `PlayerLook.cs` (Mouse Look)
- **Interaction System**: Uses an `IInteractable` interface and raycasting (`InteractionSystem.cs`).
- **Puzzles/Interactables**: `FusePuzzle.cs`, `CodePuzzle.cs`, `DoorController.cs`, `DrawerController.cs`, `KeyItem.cs`.

### Unity Components Used
- `CharacterController`: For smooth first-person collision and movement.
- `Rigidbody` / `Collider`: For interactive physics props scattered in the lab.
- `Light` (Point, Spot, Directional): Used for emergency (red) lighting and main laboratory lighting.
- `Canvas`, `Slider`, `TextMeshPro`: For HUD (Timer Bar, Status Text) and Menus.
- `Terrain`: For the exterior entrance environment.

### Testing Procedure
- Move using WASD and look with the Mouse.
- Interact with `E`.
- Start the game from the MainMenu.
- Esc to Pause/Unpause.
- Let the timer run out to verify Game Over.
- Complete all puzzles and escape to verify Victory and Slow Motion.
- Restart to confirm the Game Manager resets variables.

### Future Improvements
- Integrate `XR Interaction Toolkit` for full VR headset compatibility.
- Add advanced Post Processing (Bloom, Vignette) for stronger horror atmosphere.
- Add footstep audio.
