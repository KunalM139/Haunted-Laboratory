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
            // Clean up game managers that shouldn't be in the main menu
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null) Destroy(gm.gameObject);
            
            UIManager um = FindAnyObjectByType<UIManager>();
            if (um != null) Destroy(um.gameObject);

            if (FindAnyObjectByType<MainMenuManager>() == null)
            {
                Debug.Log("MainMenuManager missing from scene. Auto-injecting...");
                GameObject go = new GameObject("MainMenuManager");
                go.AddComponent<MainMenuManager>();
            }
        }
    }

    void Start()
    {
        RunDiagnostics();
        EnsureMenuUI();
        RunDiagnostics(); // run again after EnsureMenuUI
    }

    void RunDiagnostics()
    {
        Debug.Log("========== MAIN MENU RUNTIME DIAGNOSTICS ==========");
        
        Debug.Log($"MainMenuManager: exists=true, enabled={enabled}, active={gameObject.activeInHierarchy}");

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("Canvas: NULL");
        }
        else
        {
            RectTransform crt = canvas.GetComponent<RectTransform>();
            Debug.Log($"Canvas: enabled={canvas.enabled}, renderMode={canvas.renderMode}");
            Debug.Log($"Canvas Transform: localScale={canvas.transform.localScale}");
            Debug.Log($"Canvas RectTransform: rect={crt.rect}, sizeDelta={crt.sizeDelta}");
            Debug.Log($"Canvas: pixelRect={canvas.pixelRect}");

            Debug.Log("--- Canvas Children ---");
            LogChildRecursive(canvas.transform, 0);
        }

        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = FindAnyObjectByType<Camera>();
        if (mainCam != null)
        {
            Debug.Log($"MainCamera: enabled={mainCam.enabled}, pos={mainCam.transform.position}, rot={mainCam.transform.rotation.eulerAngles}");
            Debug.Log($"MainCamera: projection={(mainCam.orthographic ? "Orthographic" : "Perspective")}, near={mainCam.nearClipPlane}, far={mainCam.farClipPlane}");
        }
        else
        {
            Debug.Log("MainCamera: NULL");
        }
        Debug.Log("===================================================");
    }

    void LogChildRecursive(Transform parent, int depth)
    {
        foreach (Transform child in parent)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            string indent = new string('-', depth * 2);
            string active = child.gameObject.activeInHierarchy ? "ACTIVE" : "INACTIVE";
            
            string rtInfo = rt != null ? $"localScale={rt.localScale}, pos={rt.position}, anchoredPos={rt.anchoredPosition}, sizeDelta={rt.sizeDelta}" : "NO RECT";
            
            string textInfo = "";
            Text txt = child.GetComponent<Text>();
            if (txt != null)
            {
                textInfo = $" [Text: '{txt.text}', font={(txt.font != null ? txt.font.name : "NULL")}, size={txt.fontSize}, color={txt.color}, alpha={txt.color.a}]";
            }
            
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            string cgInfo = cg != null ? $" [CanvasGroup alpha={cg.alpha}]" : "";

            Debug.Log($"{indent} {child.name} ({active}) | {rtInfo}{textInfo}{cgInfo}");
            LogChildRecursive(child, depth + 1);
        }
    }

    void EnsureMenuUI()
    {
        Debug.Log("Ensuring Runtime Menu UI...");
        
        // 1. Find or Create Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("RuntimeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.enabled = true;
        canvas.gameObject.SetActive(true);
        
        // Explicitly fix Canvas RectTransform
        RectTransform canvasRt = canvas.GetComponent<RectTransform>();
        canvasRt.localScale = Vector3.one;

        // 2. Disable existing broken UI to prevent overlap
        foreach (Transform child in canvas.transform)
        {
            if (child.name != "RuntimeBackground" && child.name != "RuntimeMenuRoot")
            {
                child.gameObject.SetActive(false);
            }
        }

        // 3. Ensure EventSystem exists
        EventSystem es = FindAnyObjectByType<EventSystem>();
        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 4. Create Background
        Transform bgTrans = canvas.transform.Find("RuntimeBackground");
        if (bgTrans == null)
        {
            GameObject bgObj = new GameObject("RuntimeBackground");
            bgObj.transform.parent = canvas.transform;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.15f, 1f); // Dark blue-grey visible background
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            bgRt.anchoredPosition = Vector2.zero;
            bgRt.localScale = Vector3.one;
        }

        // 5. Create Menu Root
        Transform rootTrans = canvas.transform.Find("RuntimeMenuRoot");
        if (rootTrans != null) Destroy(rootTrans.gameObject); // Rebuild it fresh

        GameObject rootObj = new GameObject("RuntimeMenuRoot");
        rootObj.transform.parent = canvas.transform;
        RectTransform rootRt = rootObj.AddComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.sizeDelta = Vector2.zero;
        rootRt.anchoredPosition = Vector2.zero;
        rootRt.localScale = Vector3.one;

        // 6. Create Title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.parent = rootTrans ?? rootObj.transform;
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "HAUNTED LABORATORY";
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = 80;
        titleText.color = Color.white;
        titleText.font = defaultFont;
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 1f);
        titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0, -120);
        titleRt.sizeDelta = new Vector2(800, 100);
        titleRt.localScale = Vector3.one;

        // Subtitle
        GameObject subObj = new GameObject("SubtitleText");
        subObj.transform.parent = rootTrans ?? rootObj.transform;
        Text subText = subObj.AddComponent<Text>();
        subText.text = "VR ESCAPE ROOM";
        subText.alignment = TextAnchor.MiddleCenter;
        subText.fontSize = 40;
        subText.color = Color.gray;
        subText.font = defaultFont;
        RectTransform subRt = subObj.GetComponent<RectTransform>();
        subRt.anchorMin = new Vector2(0.5f, 1f);
        subRt.anchorMax = new Vector2(0.5f, 1f);
        subRt.anchoredPosition = new Vector2(0, -200);
        subRt.sizeDelta = new Vector2(600, 60);
        subRt.localScale = Vector3.one;

        // 7. Create Buttons
        CreateRuntimeButton(rootObj.transform, "StartGameButton", "START GAME", new Vector2(0, 100), StartGame, defaultFont);
        CreateRuntimeButton(rootObj.transform, "InstructionsButton", "INSTRUCTIONS", new Vector2(0, 10), ShowInstructions, defaultFont);
        CreateRuntimeButton(rootObj.transform, "SettingsButton", "SETTINGS", new Vector2(0, -80), ShowSettings, defaultFont);
        CreateRuntimeButton(rootObj.transform, "QuitButton", "QUIT", new Vector2(0, -170), QuitGame, defaultFont);

        // Panels
        instructionsPanel = CreateRuntimePanel(rootObj.transform, "InstructionsPanel", "WASD to Move\nMouse to Look\nE to Interact", defaultFont);
        settingsPanel = CreateRuntimePanel(rootObj.transform, "SettingsPanel", "Settings Menu\nVolume Control Coming Soon", defaultFont);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        Debug.Log("MAIN MENU RUNTIME UI CREATED");
    }

    void CreateRuntimeButton(Transform parent, string name, string text, Vector2 pos, UnityEngine.Events.UnityAction action, Font font)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.parent = parent;
        
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Visible gray
        
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(400, 70);
        rt.localScale = Vector3.one;

        GameObject textObj = new GameObject("Text");
        textObj.transform.parent = btnObj.transform;
        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 30;
        txt.font = font;

        RectTransform trt = textObj.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;
        trt.anchoredPosition = Vector2.zero;
        trt.localScale = Vector3.one;
    }

    GameObject CreateRuntimePanel(Transform parent, string name, string text, Font font)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.parent = parent;
        Image bgImg = panelObj.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.9f);
        RectTransform rt = panelObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        GameObject textObj = new GameObject("Text");
        textObj.transform.parent = panelObj.transform;
        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 40;
        txt.font = font;
        RectTransform trt = textObj.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.5f, 0.5f);
        trt.anchorMax = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(800, 600);
        trt.anchoredPosition = Vector2.zero;
        trt.localScale = Vector3.one;

        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.parent = panelObj.transform;
        Image closeImg = closeBtn.AddComponent<Image>();
        closeImg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(ClosePanels);
        RectTransform brt = closeBtn.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 0.2f);
        brt.anchorMax = new Vector2(0.5f, 0.2f);
        brt.sizeDelta = new Vector2(200, 60);
        brt.anchoredPosition = Vector2.zero;
        brt.localScale = Vector3.one;

        GameObject cTextObj = new GameObject("Text");
        cTextObj.transform.parent = closeBtn.transform;
        Text cTxt = cTextObj.AddComponent<Text>();
        cTxt.text = "CLOSE";
        cTxt.alignment = TextAnchor.MiddleCenter;
        cTxt.color = Color.white;
        cTxt.fontSize = 24;
        cTxt.font = font;
        RectTransform ctrt = cTextObj.GetComponent<RectTransform>();
        ctrt.anchorMin = Vector2.zero;
        ctrt.anchorMax = Vector2.one;
        ctrt.sizeDelta = Vector2.zero;
        ctrt.anchoredPosition = Vector2.zero;
        ctrt.localScale = Vector3.one;

        return panelObj;
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
