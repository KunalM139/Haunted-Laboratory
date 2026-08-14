# Haunted Laboratory: VR Escape Room
zz
**Haunted Laboratory** is a single-player Escape Room game built as a college project for an AR/VR/MR subject.

## Project Description

You are trapped in an abandoned laboratory after a containment failure. The emergency lockdown has triggered, leaving you in the dark with no obvious way out. You must explore the room, restore the power, solve the security terminal puzzle, and unlock the final exit door before the 10-minute automated purge timer runs out!

## Objective
* Survive the 10-minute timer.
* Find the missing fuse to restore power.
* Uncover clues to crack the keypad code (1984).
* Open the drawer and retrieve the exit key.
* Unlock the final door and escape.

## Features
- Complete Escape Room progression and puzzle mechanics.
- Fully operational UI with a Timer, Volume Settings, Pause Menu, and Game Over system.
- Audio systems including synthesized SFX and Ambient sounds.
- Physical Rigidbodies and gravity.
- Dynamic URP Bloom lighting and emissive materials.
- Time-dilation (slow-motion) climax upon escaping.
- Fully playable with standard keyboard/mouse (PC).

## Controls (PC)
* **W, A, S, D**: Move
* **Mouse**: Look around
* **E**: Interact (Pickup, Insert, Read)
* **Escape**: Pause Menu

## Project Architecture & VR Compatibility
**IMPORTANT**: While the current iteration is configured for **Keyboard and Mouse (PC)** for seamless testing without an HMD, the project’s architecture (specifically `InteractionSystem` and `IInteractable`) is abstracted and VR-ready. It can be easily upgraded to support XR Ray Interactors or Direct Interactors.

## Academic Requirements Demonstrated
1. **Color and texture**: Synthesized textures on primitive geometry.
2. **Parent-child relationship**: Nested objects like drawers and light bulbs.
3. **Collision**: Environment bounded by BoxColliders.
4. **Rigidbody**: Pushable `PhysicsCrates`.
5. **Physics**: Real-time gravity and physical reactions.
6. **Glow / post-processing**: URP Global Volume (Bloom).
7. **Shading**: Emissive HDR and Metallic Lit shaders.
8. **Lighting**: Real-time Directional and Point Lights.
9. **Materials**: Modular URP Lit Materials.
10. **Assets**: Exported internal `.wav`, `.png`, and `.prefab` files.
11. **Prefabs**: Real Prefabs used for Crates, Keys, and Fuses.
12. **Menu**: Interactive Canvas UI (Main, Pause, Game Over).
13. **Timer bar**: Shrinking UI Slider element.
14. **Volume bar**: Volume control via `UIManager` scaling audio listener.
15. **Timer**: 10-minute fail-state timer.
16. **Audio**: C# generated WAV files managed by an `AudioManager`.
17. **Simple C# code**: Highly modular Editor and Runtime scripts.
18. **Terrain**: True Unity Terrain object outside the lab.
19. **Slow motion**: `Time.timeScale` manipulation at the end of the game.

## Setup & Running the Project
### Unity Version
- Built using **Unity 6000.5.8f1** (URP).

### How to Clone / Open
1. `git clone https://github.com/USERNAME/Haunted-Laboratory-VR-Escape-Room.git`
2. Open Unity Hub.
3. Click **Add Project** and select the cloned directory.
4. Allow Unity to resolve dependencies (this may take a few minutes for URP).

### How to Play
1. In the Project window, navigate to `Assets/Scenes/`.
2. Open `MainMenu.unity`.
3. Press **Play** in the editor.

## Team
- **[Team Member Name]** - Development & Design
- **[Team Member Name]** - Audio & Puzzles
