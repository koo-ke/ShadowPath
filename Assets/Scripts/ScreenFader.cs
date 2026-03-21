using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    private Image panel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene" && panel.color.a > 0f)
            StartCoroutine(Fade(panel.color.a, 0f, 0.5f));
    }

    private void BuildUI()
    {
        GameObject canvasGO = new GameObject("FaderCanvas");
        canvasGO.transform.SetParent(transform);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform);

        var rt = panelGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        panel = panelGO.AddComponent<Image>();
        panel.color = new Color(0f, 0f, 0f, 0f); // 初期状態は透明
        panel.raycastTarget = false;
    }

    public static IEnumerator FadeIn(float duration)
    {
        yield return Instance.StartCoroutine(Instance.Fade(1f, 0f, duration));
    }

    public static IEnumerator FadeOut(float duration)
    {
        yield return Instance.StartCoroutine(Instance.Fade(0f, 1f, duration));
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
    {
        panel.raycastTarget = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            panel.color = new Color(0f, 0f, 0f, Mathf.Lerp(fromAlpha, toAlpha, t));
            yield return null;
        }
        panel.color = new Color(0f, 0f, 0f, toAlpha);
        // フェードイン完了後はレイキャストを無効化（クリック遮断を解除）
        panel.raycastTarget = toAlpha > 0f;
    }
}
