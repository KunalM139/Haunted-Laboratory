using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;
    public GameObject settingsPanel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitializeOnLoad()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (FindFirstObjectByType<MainMenuManager>() == null)
            {
                Debug.LogError("========== AUTO-INJECTING MAIN MENU MANAGER ==========");
                GameObject go = new GameObject("MainMenuManager");
                go.AddComponent<MainMenuManager>();
            }
        }
    }

    void Awake()
    {
        Debug.LogError("========== MAIN MENU MANAGER AWAKE ==========");
        Debug.LogError("MainMenuManager GameObject = " + gameObject.name);
        
        EnsureMenuUI();
    }

    void EnsureMenuUI()
    {
        Debug.LogError("========== CREATING RUNTIME MENU ==========");
        
        // 1. Destroy any stray GameManagers to prevent conflicts
        GameManager[] gms = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
        foreach (GameManager gm in gms)
        {
            Debug.LogError("Destroying stale GameManager: " + gm.gameObject.name);
            Destroy(gm.gameObject);
        }

        // 2. Find or Create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("RuntimeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.LogError("Created new RuntimeCanvas");
        }
        else
        {
            Debug.LogError("Reusing existing Canvas: " + canvas.name);
        }

        // Configure Canvas
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.enabled = true;
        canvas.gameObject.SetActive(true);
        
        RectTransform canvasRt = canvas.GetComponent<RectTransform>();
        canvasRt.localScale = Vector3.one;

        // 3. Clear existing Canvas children (hide them)
        foreach (Transform child in canvas.transform)
        {
            if (child.name != "RuntimeTestText")
            {
                child.gameObject.SetActive(false);
            }
        }

        // 4. Create EventSystem if missing
        EventSystem es = FindFirstObjectByType<EventSystem>();
        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        // 5. Create "HAUNTED LABORATORY TEST" Text
        Transform testTextTrans = canvas.transform.Find("RuntimeTestText");
        if (testTextTrans != null) Destroy(testTextTrans.gameObject);

        GameObject testTextObj = new GameObject("RuntimeTestText");
        testTextObj.transform.SetParent(canvas.transform, false);

        Text testText = testTextObj.AddComponent<Text>();
        testText.text = "HAUNTED LABORATORY TEST";
        testText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        testText.fontSize = 60;
        testText.color = Color.white;
        testText.alignment = TextAnchor.MiddleCenter;

        RectTransform testRt = testTextObj.GetComponent<RectTransform>();
        testRt.anchorMin = new Vector2(0.5f, 0.5f);
        testRt.anchorMax = new Vector2(0.5f, 0.5f);
        testRt.pivot = new Vector2(0.5f, 0.5f);
        testRt.anchoredPosition = Vector2.zero;
        testRt.sizeDelta = new Vector2(800, 150);
        testRt.localScale = Vector3.one;

        // 6. Print Proof Logs
        Debug.LogError(
            "RUNTIME MENU CREATED: " +
            "Canvas=" + canvas.name +
            " active=" + canvas.gameObject.activeInHierarchy +
            " enabled=" + canvas.enabled +
            " scale=" + canvas.transform.localScale
        );
        Debug.LogError("RUNTIME UI CHILD COUNT = " + canvas.transform.childCount);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Laboratory");
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    public void ClosePanels()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested");
        Application.Quit();
    }
}
