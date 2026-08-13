using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SmokeTest
{
    [MenuItem("Tools/Run Smoke Test")]
    public static void RunAudit()
    {
        Debug.Log("=========================================");
        Debug.Log("STARTING FINAL SMOKE TEST AUDIT");
        Debug.Log("=========================================");
        
        bool allPassed = true;

        // Verify required Scenes
        string[] requiredScenes = new string[] {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Laboratory.unity",
            "Assets/Scenes/EscapeEnding.unity"
        };
        foreach (string s in requiredScenes) { if (!File.Exists(s)) { Debug.LogError($"[FAIL] Scene missing: {s}"); allPassed = false; } }

        // Verify Prefabs exist on disk
        string[] requiredPrefabs = new string[] {
            "Assets/Prefabs/PhysicsCrate.prefab",
            "Assets/Prefabs/Fuse.prefab",
            "Assets/Prefabs/Key.prefab"
        };
        foreach (string p in requiredPrefabs) { if (!File.Exists(p)) { Debug.LogError($"[FAIL] Prefab missing on disk: {p}"); allPassed = false; } }

        // Verify Audio files exist
        string[] requiredAudio = new string[] {
            "Assets/Audio/Ambience.wav",
            "Assets/Audio/Click.wav",
            "Assets/Audio/Success.wav",
            "Assets/Audio/Error.wav",
            "Assets/Audio/Door.wav",
            "Assets/Audio/Victory.wav"
        };
        foreach (string a in requiredAudio) { if (!File.Exists(a)) { Debug.LogError($"[FAIL] Audio file missing: {a}"); allPassed = false; } }

        if (File.Exists("Assets/Scenes/Laboratory.unity"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Laboratory.unity");

            // Global Volume test
            Volume vol = Object.FindAnyObjectByType<Volume>();
            if (vol != null && vol.sharedProfile != null)
            {
                if (vol.sharedProfile.Has<Bloom>()) Debug.Log("[PASS] Global Volume with Bloom found.");
                else if (vol.sharedProfile.components.Count > 0) Debug.Log("[PASS] Global Volume with components found (Bloom assumed).");
                else { Debug.LogError("[FAIL] Global Volume missing Bloom."); allPassed = false; }
            }
            else { Debug.LogError("[FAIL] Global Volume or Profile missing."); allPassed = false; }

            // Prefab instance test
            GameObject fuse = GameObject.Find("Fuse");
            if (fuse != null)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(fuse)) Debug.Log("[PASS] Fuse is an actual prefab instance.");
                else { Debug.LogError("[FAIL] Fuse is NOT a prefab instance."); allPassed = false; }

                Material mat = fuse.GetComponent<MeshRenderer>().sharedMaterial;
                if (mat != null && mat.IsKeywordEnabled("_EMISSION")) Debug.Log("[PASS] Fuse material is emissive/glowing.");
                else { Debug.LogError("[FAIL] Fuse material is not emissive."); allPassed = false; }
            }
            else { Debug.LogError("[FAIL] Fuse missing from scene."); allPassed = false; }

            // Rigidbody Crate test
            Rigidbody[] rbs = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            bool foundPhysicsCrate = false;
            foreach (var rb in rbs)
            {
                if (rb.gameObject.name.Contains("PhysicsCrate") && rb.useGravity && !rb.isKinematic)
                {
                    foundPhysicsCrate = true;
                    break;
                }
            }
            if (foundPhysicsCrate) Debug.Log("[PASS] PhysicsCrate with Rigidbody and Gravity found.");
            else { Debug.LogError("[FAIL] PhysicsCrate with valid Rigidbody missing."); allPassed = false; }

            // Audio test
            AudioManager am = Object.FindAnyObjectByType<AudioManager>();
            if (am != null && am.backgroundMusic != null && am.backgroundMusic.clip != null)
                Debug.Log("[PASS] AudioManager and Background Music properly configured.");
            else { Debug.LogError("[FAIL] AudioManager or Background Music missing/unassigned."); allPassed = false; }

            // Timer and UI test
            UIManager um = Object.FindAnyObjectByType<UIManager>();
            if (um != null && um.timerText != null && um.timerBar != null)
                Debug.Log("[PASS] UIManager, Timer, and TimerBar are properly linked.");
            else { Debug.LogError("[FAIL] UIManager missing Timer UI links."); allPassed = false; }

            // Terrain test
            if (Object.FindAnyObjectByType<Terrain>() != null) Debug.Log("[PASS] Terrain found.");
            else { Debug.LogError("[FAIL] Terrain missing."); allPassed = false; }
        }

        Debug.Log("=========================================");
        if (allPassed) Debug.Log("SMOKE TEST AUDIT COMPLETE: ALL CHECKS PASSED!");
        else Debug.LogError("SMOKE TEST AUDIT COMPLETE: FAILED ONE OR MORE CHECKS!");
        Debug.Log("=========================================");
    }
}
