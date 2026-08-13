using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RuntimeMainMenuTest : MonoBehaviour
{
    [MenuItem("Tools/Test Main Menu")]
    public static void RunTest()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");

        bool allPassed = true;
        int totalTests = 0;
        int passed = 0;

        // Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[FAIL] No Canvas found."); allPassed = false; totalTests++; }
        else
        {
            totalTests++;
            Debug.Log($"[PASS] Canvas found. enabled={canvas.enabled} renderMode={canvas.renderMode}");
            passed++;

            RectTransform crt = canvas.GetComponent<RectTransform>();
            totalTests++;
            if (crt.localScale == Vector3.zero)
            {
                Debug.LogError("[FAIL] Canvas RectTransform localScale is (0,0,0) — THIS IS THE INVISIBLE UI BUG!");
                allPassed = false;
            }
            else
            {
                Debug.Log($"[PASS] Canvas localScale = {crt.localScale}");
                passed++;
            }

            // Check children
            string[] expectedNames = { "TitleText", "START GAMEButton", "INSTRUCTIONSButton", "SETTINGSButton", "QUITButton", "InstructionsPanel", "SettingsPanel" };
            foreach (string name in expectedNames)
            {
                totalTests++;
                Transform child = FindDeep(canvas.transform, name);
                if (child == null)
                {
                    Debug.LogError($"[FAIL] Missing UI element: {name}");
                    allPassed = false;
                    continue;
                }

                if (!child.gameObject.activeInHierarchy && !name.Contains("Panel"))
                {
                    Debug.LogError($"[FAIL] {name} is not active.");
                    allPassed = false;
                    continue;
                }

                RectTransform rt = child.GetComponent<RectTransform>();
                if (rt == null)
                {
                    Debug.LogError($"[FAIL] {name} has no RectTransform.");
                    allPassed = false;
                    continue;
                }

                if (rt.sizeDelta == Vector2.zero && rt.anchorMin == rt.anchorMax)
                {
                    Debug.LogError($"[FAIL] {name} has zero size and anchored at a point.");
                    allPassed = false;
                    continue;
                }

                // Check text content
                Text txt = child.GetComponent<Text>();
                if (txt != null)
                {
                    if (string.IsNullOrEmpty(txt.text))
                    {
                        Debug.LogError($"[FAIL] {name} Text is empty.");
                        allPassed = false;
                        continue;
                    }
                    if (txt.font == null)
                    {
                        Debug.LogError($"[FAIL] {name} font is NULL — text will be invisible!");
                        allPassed = false;
                        continue;
                    }
                    if (txt.color.a <= 0)
                    {
                        Debug.LogError($"[FAIL] {name} text alpha is 0 — invisible!");
                        allPassed = false;
                        continue;
                    }
                }

                Debug.Log($"[PASS] {name}: active={child.gameObject.activeSelf} size={rt.sizeDelta} pos={rt.anchoredPosition}");
                passed++;
            }

            // Check button text children have fonts
            string[] buttonNames = { "START GAMEButton", "INSTRUCTIONSButton", "SETTINGSButton", "QUITButton" };
            foreach (string btnName in buttonNames)
            {
                totalTests++;
                Transform btn = FindDeep(canvas.transform, btnName);
                if (btn == null) { Debug.LogError($"[FAIL] Button {btnName} not found"); allPassed = false; continue; }
                
                Transform textChild = btn.Find("Text");
                if (textChild == null) { Debug.LogError($"[FAIL] Button {btnName} has no Text child"); allPassed = false; continue; }
                
                Text btnText = textChild.GetComponent<Text>();
                if (btnText == null) { Debug.LogError($"[FAIL] {btnName}/Text has no Text component"); allPassed = false; continue; }
                if (btnText.font == null) { Debug.LogError($"[FAIL] {btnName}/Text font is NULL"); allPassed = false; continue; }
                if (string.IsNullOrEmpty(btnText.text)) { Debug.LogError($"[FAIL] {btnName}/Text has empty text"); allPassed = false; continue; }
                
                Debug.Log($"[PASS] {btnName}/Text: text=\"{btnText.text}\" font={btnText.font.name} color={btnText.color}");
                passed++;
            }
        }

        // Camera
        totalTests++;
        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = FindAnyObjectByType<Camera>();
        if (mainCam != null)
        {
            Debug.Log($"[PASS] Camera: pos={mainCam.transform.position} near={mainCam.nearClipPlane} far={mainCam.farClipPlane} clearFlags={mainCam.clearFlags}");
            passed++;
        }
        else
        {
            Debug.LogError("[FAIL] No camera found.");
            allPassed = false;
        }

        // MainMenuManager
        totalTests++;
        MainMenuManager mmm = FindAnyObjectByType<MainMenuManager>();
        if (mmm != null)
        {
            Debug.Log($"[PASS] MainMenuManager found on '{mmm.gameObject.name}'");
            passed++;
        }
        else
        {
            Debug.LogError("[FAIL] MainMenuManager not found.");
            allPassed = false;
        }

        Debug.Log($"===== MAIN MENU TEST: {passed}/{totalTests} passed =====");
        if (allPassed) Debug.Log("MAIN MENU TEST: ALL CHECKS PASSED!");
        else Debug.LogError("MAIN MENU TEST: FAILED ONE OR MORE CHECKS!");
    }

    static Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeep(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
