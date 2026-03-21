using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private GameObject panel;

    private void Start()
    {
        if (LivesManager.Instance == null)
        {
            Debug.LogWarning("[GameOverUI] LivesManager が見つかりません。");
            return;
        }

        BuildUI();
        panel.SetActive(false);
        LivesManager.Instance.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        if (LivesManager.Instance != null)
            LivesManager.Instance.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void BuildUI()
    {
        // EventSystem（未存在時のみ生成）
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<InputSystemUIInputModule>();
        }

        // Canvas
        GameObject canvasGO = new GameObject("GameOverCanvas");
        canvasGO.transform.SetParent(transform);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        // 背景パネル（半透明の黒）
        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform);

        var panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);

        // GAME OVER テキスト
        CreateLabel(panel.transform, "GAME OVER", 0f, 120f, 60);

        // Retry ボタン
        CreateButton(panel.transform, "Retry", 0f, -40f, () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        // Title ボタン
        CreateButton(panel.transform, "Title", 0f, -120f, () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("TitleScene");
        });
    }

    private void CreateLabel(Transform parent, string text, float x, float y, float fontSize)
    {
        GameObject go = new GameObject(text);
        go.transform.SetParent(parent);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600f, 100f);
        rt.anchoredPosition = new Vector2(x, y);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    private void CreateButton(Transform parent, string label, float x, float y, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonGO = new GameObject($"Button_{label}");
        buttonGO.transform.SetParent(parent);

        var rt = buttonGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(240f, 60f);
        rt.anchoredPosition = new Vector2(x, y);

        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.15f);

        var button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;

        var colors = button.colors;
        colors.highlightedColor = new Color(0.35f, 0.35f, 0.35f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f);
        button.colors = colors;

        button.onClick.AddListener(onClick);

        // ボタンテキスト
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform);

        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 36f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
