using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private float heartSize = 36f;
    [SerializeField] private float heartSpacing = 10f;
    [SerializeField] private Vector2 topLeftOffset = new Vector2(20f, -20f);

    private TextMeshProUGUI[] heartTexts;

    private void Start()
    {
        if (LivesManager.Instance == null)
        {
            Debug.LogWarning("[LivesUI] LivesManager が見つかりません。");
            return;
        }

        BuildUI(LivesManager.Instance.CurrentLives);
        LivesManager.Instance.OnLivesChanged += UpdateHearts;
    }

    private void OnDestroy()
    {
        if (LivesManager.Instance != null)
            LivesManager.Instance.OnLivesChanged -= UpdateHearts;
    }

    private void BuildUI(int maxLives)
    {
        GameObject canvasGO = new GameObject("LivesCanvas");
        canvasGO.transform.SetParent(transform);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        heartTexts = new TextMeshProUGUI[maxLives];

        for (int i = 0; i < maxLives; i++)
        {
            GameObject heartGO = new GameObject($"Heart_{i}");
            heartGO.transform.SetParent(canvasGO.transform);

            var rt = heartGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot     = new Vector2(0f, 1f);
            rt.sizeDelta = new Vector2(heartSize, heartSize);
            rt.anchoredPosition = new Vector2(
                topLeftOffset.x + i * (heartSize + heartSpacing),
                topLeftOffset.y
            );

            var tmp = heartGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "♥";
            tmp.fontSize = heartSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            heartTexts[i] = tmp;
        }
    }

    private void UpdateHearts(int currentLives)
    {
        if (heartTexts == null) return;

        for (int i = 0; i < heartTexts.Length; i++)
            heartTexts[i].gameObject.SetActive(i < currentLives);
    }
}
