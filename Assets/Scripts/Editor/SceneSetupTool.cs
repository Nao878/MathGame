using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Scene setup tool for automatic scene configuration.
/// Menu: Tools > Setup Math Tower Defense
/// </summary>
public class SceneSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup Math Tower Defense")]
    public static void SetupScene()
    {
        // Create tags
        CreateTag("Enemy");
        CreateTag("Graph");

        // Setup camera
        SetupCamera();

        // Create GridManager
        CreateGridManager();

        // Create Base
        CreateBase();

        // Create EnemySpawner
        CreateEnemySpawner();

        // Create GameManager
        CreateGameManager();

        // Create Canvas and UI
        CreateUI();

        // Mark scene as dirty so changes can be saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("=== Scene setup complete ===");
        Debug.Log("Press Play button to start the game!");
    }

    private static void CreateTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // Check if tag already exists
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName))
            {
                return; // Already exists
            }
        }

        // Add new tag
        tagsProp.InsertArrayElementAtIndex(0);
        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
        newTag.stringValue = tagName;
        tagManager.ApplyModifiedProperties();

        Debug.Log($"Tag created: {tagName}");
    }

    private static void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }

        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 6f;
        // Center camera at origin (0, 0)
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        mainCamera.clearFlags = CameraClearFlags.SolidColor;

        Debug.Log("Camera configured (centered at origin)");
    }

    private static void CreateGridManager()
    {
        // Delete existing GridManager
        GameObject existing = GameObject.Find("GridManager");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        GameObject gridManager = new GameObject("GridManager");
        gridManager.AddComponent<GridRenderer>();

        Debug.Log("GridManager created");
    }

    private static void CreateBase()
    {
        // Delete existing Base
        GameObject existing = GameObject.Find("PlayerBase");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        GameObject baseObj = new GameObject("PlayerBase");
        baseObj.transform.position = Vector3.zero; // Origin at center
        baseObj.AddComponent<Base>();

        Debug.Log("Base created at origin (0, 0)");
    }

    private static void CreateEnemySpawner()
    {
        // Delete existing EnemySpawner
        GameObject existing = GameObject.Find("EnemySpawner");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        GameObject spawner = new GameObject("EnemySpawner");
        spawner.AddComponent<EnemySpawner>();

        Debug.Log("EnemySpawner created");
    }

    private static void CreateGameManager()
    {
        // Delete existing GameManager
        GameObject existing = GameObject.Find("GameManager");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        GameObject manager = new GameObject("GameManager");
        GameManager gm = manager.AddComponent<GameManager>();
        manager.AddComponent<GraphGenerator>();

        Debug.Log("GameManager created");
    }

    private static void CreateUI()
    {
        // Delete existing Canvas
        GameObject existing = GameObject.Find("GameCanvas");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        // Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem (create if not exists)
        if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Bottom panel (input area)
        GameObject panel = CreatePanel(canvasObj.transform, "InputPanel");
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 0);
        panelRect.pivot = new Vector2(0.5f, 0);
        panelRect.sizeDelta = new Vector2(0, 80);
        panelRect.anchoredPosition = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.9f);

        // InputField
        GameObject inputFieldObj = CreateInputField(panel.transform, "ExpressionInput");
        RectTransform inputRect = inputFieldObj.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0, 0.5f);
        inputRect.anchorMax = new Vector2(0.7f, 0.5f);
        inputRect.pivot = new Vector2(0, 0.5f);
        inputRect.sizeDelta = new Vector2(-40, 50);
        inputRect.anchoredPosition = new Vector2(20, 0);

        TMP_InputField inputField = inputFieldObj.GetComponent<TMP_InputField>();
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter expression (e.g., x, 2*x+1, x^2)";

        // Fire button
        GameObject buttonObj = CreateButton(panel.transform, "FireButton", "FIRE!");
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.7f, 0.5f);
        buttonRect.anchorMax = new Vector2(1f, 0.5f);
        buttonRect.pivot = new Vector2(1f, 0.5f);
        buttonRect.sizeDelta = new Vector2(-40, 50);
        buttonRect.anchoredPosition = new Vector2(-20, 0);

        // Set button colors
        Button button = buttonObj.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.8f, 0.2f, 0.2f);
        colors.highlightedColor = new Color(1f, 0.3f, 0.3f);
        colors.pressedColor = new Color(0.6f, 0.1f, 0.1f);
        button.colors = colors;

        // Status text
        GameObject statusObj = CreateText(canvasObj.transform, "StatusText", "Enter expression and FIRE!");
        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0);
        statusRect.pivot = new Vector2(0.5f, 0);
        statusRect.sizeDelta = new Vector2(0, 30);
        statusRect.anchoredPosition = new Vector2(0, 85);

        // Create UIManager
        GameObject uiManagerObj = GameObject.Find("GameManager");
        if (uiManagerObj == null)
        {
            uiManagerObj = new GameObject("GameManager");
        }

        UIManager uiManager = uiManagerObj.GetComponent<UIManager>();
        if (uiManager == null)
        {
            uiManager = uiManagerObj.AddComponent<UIManager>();
        }

        // Set UIManager references
        SerializedObject so = new SerializedObject(uiManager);
        so.FindProperty("expressionInput").objectReferenceValue = inputField;
        so.FindProperty("fireButton").objectReferenceValue = button;
        so.FindProperty("statusText").objectReferenceValue = statusObj.GetComponent<TextMeshProUGUI>();
        so.FindProperty("graphGenerator").objectReferenceValue = uiManagerObj.GetComponent<GraphGenerator>();
        so.ApplyModifiedProperties();

        Debug.Log("UI created");
    }

    private static GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        return panel;
    }

    private static GameObject CreateInputField(Transform parent, string name)
    {
        // Create TMP_InputField
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);

        Image bgImage = inputObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

        // Text Area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        textArea.AddComponent<RectMask2D>();

        // Placeholder
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform, false);
        RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter text...";
        placeholderText.fontSize = 20;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f);
        placeholderText.alignment = TextAlignmentOptions.Left;

        // Input Text
        GameObject inputText = new GameObject("Text");
        inputText.transform.SetParent(textArea.transform, false);
        RectTransform inputTextRect = inputText.AddComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = Vector2.zero;
        inputTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = inputText.AddComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;

        // InputField settings
        inputField.textViewport = textAreaRect;
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;

        return inputObj;
    }

    private static GameObject CreateButton(Transform parent, string name, string buttonText)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.8f, 0.2f, 0.2f);

        Button button = buttonObj.AddComponent<Button>();

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;

        return buttonObj;
    }

    private static GameObject CreateText(Transform parent, string name, string content)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        return textObj;
    }
}
