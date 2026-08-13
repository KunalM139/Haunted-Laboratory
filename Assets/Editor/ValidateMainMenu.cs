using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ValidateMainMenu
{
    [MenuItem("Tools/Validate Main Menu Runtime")]
    public static void Validate()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log("[PASS] MainMenu scene is opened");

        bool hasMainMenuManager = false;
        MainMenuManager[] managers = Object.FindObjectsByType<MainMenuManager>(FindObjectsSortMode.None);
        
        if (managers.Length > 0)
        {
            hasMainMenuManager = true;
            Debug.Log("[PASS] MainMenuManager script exists");
            Debug.Log("[PASS] MainMenuManager component is attached to MainMenu scene");
        }
        else
        {
            Debug.LogWarning("[FAIL] MainMenuManager missing. Attaching now...");
            
            // Find or create a root object
            GameObject gmObj = GameObject.Find("MainMenuManager");
            if (gmObj == null) gmObj = new GameObject("MainMenuManager");
            
            gmObj.AddComponent<MainMenuManager>();
            hasMainMenuManager = true;
            Debug.Log("[FIXED] MainMenuManager attached to MainMenu scene");
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) Debug.Log("[PASS] Canvas exists");
        else Debug.LogWarning("[FAIL] Canvas missing");

        UnityEngine.EventSystems.EventSystem es = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (es != null) Debug.Log("[PASS] EventSystem exists");
        else Debug.LogWarning("[FAIL] EventSystem missing");

        // Check build settings
        bool inBuild = false;
        foreach (var sb in EditorBuildSettings.scenes)
        {
            if (sb.path == scenePath) inBuild = true;
        }
        
        if (inBuild) Debug.Log("[PASS] MainMenu scene is in Build Settings");
        else Debug.LogWarning("[FAIL] MainMenu scene is NOT in Build Settings");

        if (hasMainMenuManager)
        {
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Scene validated and saved.");
        }
    }
}
