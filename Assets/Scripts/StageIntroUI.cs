using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageIntroUI : MonoBehaviour
{
    [SerializeField] private string stageTitle = "Stage 1";

    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        TextMeshProUGUI label = BuildLabel();
        SetAlpha(label, 0f);

        // ScreenFaderによるフェードイン（失敗しても続行）
        if (ScreenFader.Instance != null)
        {
            bool faderDone = false;
            StartCoroutine(SafeFadeIn(0.5f, () => faderDone = true));
            float timeout = 2f;
            while (!faderDone && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }
        }

        yield return StartCoroutine(FadeText(label, 0f, 1f, 0.5f));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeText(label, 1f, 0f, 0.5f));

        if (label != null && label.transform.parent != null && label.transform.parent.parent != null)
            Destroy(label.transform.parent.parent.gameObject);
    }

    private IEnumerator SafeFadeIn(float duration, System.Action onComplete)
    {
        IEnumerator fader = null;
        try { fader = ScreenFader.FadeIn(duration); } catch { }
        if (fader != null)
        {
            while (true)
            {
                try { if (!fader.MoveNext()) break; }
                catch { break; }
                yield return fader.Current;
            }
        }
        onComplete?.Invoke();
    }

    private IEnumerator FadeText(TextMeshProUGUI tmp, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(tmp, Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration)));
            yield return null;
        }
        SetAlpha(tmp, to);
    }

    private void SetAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    private TextMeshProUGUI BuildLabel()
    {
        GameObject canvasGO = new GameObject("IntroCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 150;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textGO = new GameObject("IntroText");
        textGO.transform.SetParent(canvasGO.transform);

        var rt = textGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(800f, 100f);
        rt.anchoredPosition = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = stageTitle;
        tmp.fontSize = 48f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        return tmp;
    }
}
