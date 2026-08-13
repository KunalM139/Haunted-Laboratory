using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayModeMenuInjector
{
    static PlayModeMenuInjector()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Debug.LogError("========== EDITOR HOOK: ENTERED PLAY MODE IN MAIN MENU ==========");
                
                if (Object.FindFirstObjectByType<MainMenuManager>() == null)
                {
                    Debug.LogError("========== EDITOR HOOK: AUTO-INJECTING MAIN MENU MANAGER ==========");
                    GameObject go = new GameObject("MainMenuManager");
                    go.AddComponent<MainMenuManager>();
                }
            }
        }
    }
}
