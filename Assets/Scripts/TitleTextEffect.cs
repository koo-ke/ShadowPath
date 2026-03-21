using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TitleTextEffect : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minAlpha = 0.2f;

    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(Time.time * speed) + 1f) * 0.5f);
        Color c = tmp.color;
        tmp.color = new Color(c.r, c.g, c.b, alpha);
    }
}
