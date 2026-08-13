using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelBuilder
{
    [MenuItem("Tools/Build Level")]
    public static void BuildLevel()
    {
        Debug.Log("Starting automated level build...");

        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Settings")) AssetDatabase.CreateFolder("Assets", "Settings");

        BuildPrefabs();
        BuildLaboratoryScene();
        BuildMainMenuScene();
        BuildEscapeEndingScene();

        Debug.Log("Automated level build complete.");
    }

    static void BuildPrefabs()
    {
        // Physics Crate
        GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crate.name = "PhysicsCrate";
        crate.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("Wood", Color.yellow, false);
        Rigidbody rb = crate.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.mass = 5f;
        PrefabUtility.SaveAsPrefabAsset(crate, "Assets/Prefabs/PhysicsCrate.prefab");
        Object.DestroyImmediate(crate);

        // Fuse
        GameObject fuse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fuse.name = "Fuse";
        fuse.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        fuse.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("GlowingFuse", Color.cyan, true);
        fuse.AddComponent<FusePuzzle>();
        PrefabUtility.SaveAsPrefabAsset(fuse, "Assets/Prefabs/Fuse.prefab");
        Object.DestroyImmediate(fuse);

        // Key
        GameObject keyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        keyObj.name = "Key";
        keyObj.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        keyObj.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("GlowingKey", Color.yellow, true);
        keyObj.AddComponent<KeyItem>();
        PrefabUtility.SaveAsPrefabAsset(keyObj, "Assets/Prefabs/Key.prefab");
        Object.DestroyImmediate(keyObj);
    }

    static Material GetMaterial(string name, Color color, bool isEmissive = false, bool isMetallic = false)
    {
        string path = $"Assets/Materials/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            if (isEmissive)
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                mat.SetColor("_EmissionColor", color * 5f);
            }
            if (isMetallic)
            {
                mat.SetFloat("_Metallic", 1.0f);
                mat.SetFloat("_Smoothness", 0.8f);
            }
            
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Checker.png");
            if (tex != null) mat.mainTexture = tex;

            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    static void BuildLaboratoryScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, "Assets/Settings/PostProcessProfile.asset");
        Bloom bloom = profile.Add<Bloom>();
        AssetDatabase.AddObjectToAsset(bloom, profile);
        bloom.active = true;
        bloom.intensity.Override(3f);
        bloom.threshold.Override(0.5f);
        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();

        GameObject volumeObj = new GameObject("GlobalVolume");
        Volume v = volumeObj.AddComponent<Volume>();
        v.isGlobal = true;
        v.sharedProfile = profile;

        // Environment
        GameObject dirLightObj = new GameObject("Directional Light");
        Light dLight = dirLightObj.AddComponent<Light>();
        dLight.type = LightType.Directional;
        dLight.intensity = 0.2f;
        dirLightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

        GameObject terrainObj = new GameObject("Terrain");
        TerrainData tData = new TerrainData();
        tData.size = new Vector3(100, 10, 100);
        AssetDatabase.CreateAsset(tData, "Assets/TerrainData.asset");
        TerrainCollider tc = terrainObj.AddComponent<TerrainCollider>();
        tc.terrainData = tData;
        Terrain t = terrainObj.AddComponent<Terrain>();
        t.terrainData = tData;
        terrainObj.transform.position = new Vector3(-50, -1, -50);

        GameObject room = new GameObject("LaboratoryRoom");
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.parent = room.transform;
        floor.transform.localScale = new Vector3(20, 1, 20);
        floor.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("FloorMat", Color.gray);

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = room.transform;
            wall.transform.localScale = new Vector3(20, 10, 1);
            wall.transform.position = new Vector3(
                i == 0 ? 0 : (i == 1 ? 0 : (i == 2 ? 10 : -10)),
                4.5f,
                i == 0 ? 10 : (i == 1 ? -10 : 0)
            );
            if (i >= 2) wall.transform.rotation = Quaternion.Euler(0, 90, 0);
            wall.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("WallMat", Color.darkGray);
        }

        // Instantiating Physics Crates
        GameObject cratePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PhysicsCrate.prefab");
        for (int i = 0; i < 3; i++)
        {
            GameObject instCrate = (GameObject)PrefabUtility.InstantiatePrefab(cratePrefab);
            instCrate.transform.position = new Vector3(-5, 2 + (i * 1.5f), 5);
        }

        // Instantiating Fuse and Key
        GameObject fusePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Fuse.prefab");
        GameObject instFuse = (GameObject)PrefabUtility.InstantiatePrefab(fusePrefab);
        instFuse.transform.position = new Vector3(8, 1, -8);

        GameObject keyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Key.prefab");
        GameObject instKey = (GameObject)PrefabUtility.InstantiatePrefab(keyPrefab);
        instKey.transform.position = new Vector3(-8, 1, -8);

        // Final Door
        GameObject finalDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finalDoor.name = "FinalDoor";
        finalDoor.transform.localScale = new Vector3(4, 8, 0.5f);
        finalDoor.transform.position = new Vector3(0, 4, 9.5f);
        finalDoor.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("MetallicDoor", Color.white, false, true);
        DoorController dc = finalDoor.AddComponent<DoorController>();

        // Interactables setup (CodePuzzle, Drawer, EmLight)
        GameObject keypad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keypad.name = "Keypad";
        keypad.transform.position = new Vector3(3, 4, 9.5f);
        CodePuzzle cp = keypad.AddComponent<CodePuzzle>();

        GameObject drawer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        drawer.name = "Drawer";
        drawer.transform.position = new Vector3(5, 1, 8);
        drawer.AddComponent<DrawerController>();

        GameObject emLightObj = new GameObject("EmergencyLight");
        emLightObj.transform.position = new Vector3(0, 9, 0);
        Light eLight = emLightObj.AddComponent<Light>();
        eLight.type = LightType.Point;
        eLight.color = Color.red;
        eLight.intensity = 2f;
        
        GameObject lightBulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightBulb.transform.parent = emLightObj.transform;
        lightBulb.transform.localPosition = Vector3.zero;
        lightBulb.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial("GlowingEmLight", Color.red, true);

        // Player
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 2, 0);
        CharacterController cc = player.AddComponent<CharacterController>();
        PlayerController pc = player.AddComponent<PlayerController>();

        GameObject camObj = new GameObject("Main Camera");
        camObj.transform.parent = player.transform;
        camObj.transform.localPosition = new Vector3(0, 0.6f, 0);
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<AudioListener>();
        PlayerLook pl = camObj.AddComponent<PlayerLook>();
        pl.playerBody = player.transform;

        InteractionSystem isys = camObj.AddComponent<InteractionSystem>();
        isys.playerCamera = cam;

        // Managers
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gm = gameManagerObj.AddComponent<GameManager>();
        AudioManager am = gameManagerObj.AddComponent<AudioManager>();
        gameManagerObj.AddComponent<SlowMotionController>();

        // UI
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        UIManager um = canvasObj.AddComponent<UIManager>();
        
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // Attach audio clips to AudioManager
        AudioClip successClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Success.wav");
        AudioClip errorClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Error.wav");
        AudioClip doorClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Door.wav");
        AudioClip victoryClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Victory.wav");
        AudioClip ambientClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience.wav");
        
        AudioSource bgMusic = gameManagerObj.AddComponent<AudioSource>();
        bgMusic.clip = ambientClip;
        bgMusic.loop = true;
        bgMusic.playOnAwake = true;
        
        AudioSource sfx = gameManagerObj.AddComponent<AudioSource>();
        am.backgroundMusic = bgMusic;
        am.sfxSource = sfx;
        am.successSound = successClip;
        am.errorSound = errorClip;
        am.doorOpenSound = doorClip;

        // UI Setup logic (TimerText, Slider, PauseMenu, GameOver Menu)
        GameObject timerTextObj = new GameObject("TimerText");
        timerTextObj.transform.parent = canvasObj.transform;
        Text timerText = timerTextObj.AddComponent<Text>();
        timerTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        timerTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        timerTextObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        timerTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -20);
        um.timerText = timerText;

        GameObject sliderObj = new GameObject("TimerSlider");
        sliderObj.transform.parent = canvasObj.transform;
        Slider slider = sliderObj.AddComponent<Slider>();
        sliderObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        sliderObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        sliderObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        sliderObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -60);
        sliderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);
        um.timerBar = slider;

        GameObject sliderBg = new GameObject("Background");
        sliderBg.transform.parent = sliderObj.transform;
        Image bgImg = sliderBg.AddComponent<Image>();
        bgImg.color = Color.black;
        sliderBg.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        sliderBg.GetComponent<RectTransform>().anchorMax = Vector2.one;
        sliderBg.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        GameObject sliderFillArea = new GameObject("Fill Area");
        sliderFillArea.transform.parent = sliderObj.transform;
        sliderFillArea.AddComponent<RectTransform>();
        sliderFillArea.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        sliderFillArea.GetComponent<RectTransform>().anchorMax = Vector2.one;
        sliderFillArea.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        GameObject sliderFill = new GameObject("Fill");
        sliderFill.transform.parent = sliderFillArea.transform;
        Image fillImg = sliderFill.AddComponent<Image>();
        fillImg.color = Color.red;
        slider.fillRect = sliderFill.GetComponent<RectTransform>();

        GameObject intTextObj = new GameObject("InteractionText");
        intTextObj.transform.parent = canvasObj.transform;
        Text intText = intTextObj.AddComponent<Text>();
        intTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        intTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        intTextObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        isys.interactionText = intText;

        // Menus
        GameObject pauseMenu = new GameObject("PauseMenu");
        pauseMenu.transform.parent = canvasObj.transform;
        pauseMenu.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Image pBg = pauseMenu.AddComponent<Image>();
        pBg.color = new Color(0, 0, 0, 0.8f);
        pauseMenu.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        pauseMenu.GetComponent<RectTransform>().anchorMax = Vector2.one;
        GameObject pTextObj = new GameObject("PauseText");
        pTextObj.transform.parent = pauseMenu.transform;
        Text pText = pTextObj.AddComponent<Text>();
        pText.text = "PAUSED";
        pTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
        um.pauseMenu = pauseMenu;
        pauseMenu.SetActive(false);

        GameObject gameOverMenu = new GameObject("GameOverMenu");
        gameOverMenu.transform.parent = canvasObj.transform;
        gameOverMenu.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Image goBg = gameOverMenu.AddComponent<Image>();
        goBg.color = new Color(0.5f, 0, 0, 0.8f);
        gameOverMenu.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        gameOverMenu.GetComponent<RectTransform>().anchorMax = Vector2.one;
        GameObject goTextObj = new GameObject("GameOverText");
        goTextObj.transform.parent = gameOverMenu.transform;
        Text goText = goTextObj.AddComponent<Text>();
        goText.text = "GAME OVER";
        goTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
        um.gameOverMenu = gameOverMenu;
        gameOverMenu.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Laboratory.unity");
    }

    static void BuildMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<AudioListener>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler cs = canvasObj.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        GameObject gmObj = new GameObject("MainMenuManager");
        MainMenuManager mmm = gmObj.AddComponent<MainMenuManager>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.parent = canvasObj.transform;
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "HAUNTED LABORATORY\n<size=50%>VR ESCAPE ROOM</size>";
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = 80;
        if (defaultFont != null) titleText.font = defaultFont;
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.8f);
        titleRt.anchorMax = new Vector2(0.5f, 0.8f);
        titleRt.anchoredPosition = Vector2.zero;
        titleRt.sizeDelta = new Vector2(800, 200);

        // Buttons
        string[] btnNames = { "START GAME", "INSTRUCTIONS", "SETTINGS", "QUIT" };
        for (int i = 0; i < btnNames.Length; i++)
        {
            GameObject btnObj = new GameObject(btnNames[i] + "Button");
            btnObj.transform.parent = canvasObj.transform;
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = Color.gray;
            Button btn = btnObj.AddComponent<Button>();
            
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, 50 - (i * 80));
            rt.sizeDelta = new Vector2(300, 60);

            GameObject textObj = new GameObject("Text");
            textObj.transform.parent = btnObj.transform;
            Text btnText = textObj.AddComponent<Text>();
            btnText.text = btnNames[i];
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.white;
            if (defaultFont != null) btnText.font = defaultFont;
            
            RectTransform trt = textObj.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;
            trt.anchoredPosition = Vector2.zero;

            // Hook up events
            if (i == 0) UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mmm.StartGame);
            else if (i == 1) UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mmm.ShowInstructions);
            else if (i == 2) UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mmm.ShowSettings);
            else if (i == 3) UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mmm.QuitGame);
        }

        // Panels
        GameObject instPanel = CreatePanel(canvasObj, "InstructionsPanel", "WASD to Move\nMouse to Look\nE to Interact", defaultFont, mmm);
        GameObject setPanel = CreatePanel(canvasObj, "SettingsPanel", "Settings Menu\nVolume Control Coming Soon", defaultFont, mmm);
        mmm.instructionsPanel = instPanel;
        mmm.settingsPanel = setPanel;
        instPanel.SetActive(false);
        setPanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    static GameObject CreatePanel(GameObject canvas, string name, string text, Font font, MainMenuManager mmm)
    {
        GameObject panel = new GameObject(name);
        panel.transform.parent = canvas.transform;
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        GameObject textObj = new GameObject("Text");
        textObj.transform.parent = panel.transform;
        Text tmp = textObj.AddComponent<Text>();
        tmp.text = text;
        tmp.alignment = TextAnchor.MiddleCenter;
        tmp.fontSize = 40;
        if (font != null) tmp.font = font;
        RectTransform trt = textObj.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.5f, 0.5f);
        trt.anchorMax = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(800, 600);
        trt.anchoredPosition = Vector2.zero;

        GameObject btnObj = new GameObject("CloseButton");
        btnObj.transform.parent = panel.transform;
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = Color.red;
        Button btn = btnObj.AddComponent<Button>();
        RectTransform brt = btnObj.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 0.2f);
        brt.anchorMax = new Vector2(0.5f, 0.2f);
        brt.sizeDelta = new Vector2(200, 60);
        brt.anchoredPosition = Vector2.zero;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mmm.ClosePanels);

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.parent = btnObj.transform;
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "CLOSE";
        btnText.alignment = TextAnchor.MiddleCenter;
        if (font != null) btnText.font = font;
        RectTransform btrt = btnTextObj.GetComponent<RectTransform>();
        btrt.anchorMin = Vector2.zero;
        btrt.anchorMax = Vector2.one;
        btrt.sizeDelta = Vector2.zero;
        btrt.anchoredPosition = Vector2.zero;

        return panel;
    }

    static void BuildEscapeEndingScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<AudioListener>();
        
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        GameObject titleObj = new GameObject("VictoryText");
        titleObj.transform.parent = canvasObj.transform;
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "YOU ESCAPED!";
        titleText.color = Color.green;
        titleObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        titleObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        titleObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 100);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/EscapeEnding.unity");
    }
}
