using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class RuntimeInvestigator : MonoBehaviour
{
    [MenuItem("Tools/Investigate Main Menu")]
    public static void Investigate()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        Debug.Log("--- RUNTIME INVESTIGATION ---");
        
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("No Canvas found!"); return; }
        
        Debug.Log($"Canvas Render Mode: {canvas.renderMode}");
        Debug.Log($"Canvas enabled: {canvas.enabled}");
        
        int childCount = 0;
        foreach (Transform child in canvas.transform)
        {
            childCount++;
            Debug.Log($"Child: {child.name} (Active: {child.gameObject.activeInHierarchy})");
            Text tmp = child.GetComponent<Text>();
            if (tmp != null)
            {
                Debug.Log($"  TMP Text: {tmp.text}");
                Debug.Log($"  TMP Font: {(tmp.font != null ? tmp.font.name : "NULL")}");
                Debug.Log($"  TMP Color: {tmp.color}");
            }
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                Debug.Log($"  RectTransform: anchoredPosition={rt.anchoredPosition}, sizeDelta={rt.sizeDelta}");
            }
        }
        
        Debug.Log($"Total UI elements under canvas: {childCount}");
        Debug.Log("-----------------------------");
    }
}
