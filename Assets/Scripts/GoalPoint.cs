using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    public event System.Action OnCleared;

    private bool cleared = false;
    private SpriteRenderer pillarRenderer;
    private SpriteRenderer[] orbRenderers = new SpriteRenderer[4];

    // 上下左右の配置オフセット
    private static readonly Vector2[] OrbOffsets = new Vector2[]
    {
        new Vector2( 0f,  0.5f),  // 上
        new Vector2( 0f, -0.5f),  // 下
        new Vector2(-0.5f, 0f),   // 左
        new Vector2( 0.5f, 0f),   // 右
    };

    // 各orbの点滅位相オフセット（ずらして明滅感を出す）
    private static readonly float[] OrbPhaseOffsets = new float[]
    {
        0f, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f,
    };

    private void Start()
    {
        SetupVisual();
    }

    private void Update()
    {
        float t = Time.time * 3f;

        // 柱の点滅：Alpha 0.4〜1.0
        float pillarAlpha = Mathf.Lerp(0.4f, 1.0f, (Mathf.Sin(t) + 1f) * 0.5f);
        pillarRenderer.color = new Color(1f, 1f, 1f, pillarAlpha);

        // 各orbをタイミングをずらして点滅
        for (int i = 0; i < orbRenderers.Length; i++)
        {
            float orbAlpha = Mathf.Lerp(0.4f, 1.0f, (Mathf.Sin(t + OrbPhaseOffsets[i]) + 1f) * 0.5f);
            orbRenderers[i].color = new Color(1f, 1f, 1f, orbAlpha);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;

        if (other.CompareTag("Player"))
        {
            cleared = true;
            AudioManager.Instance?.PlaySE("goal");
            Debug.Log("Stage Clear!");
            OnCleared?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void Reset()
    {
        SetupVisual();
    }
#endif

    private void SetupVisual()
    {
        Sprite square = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 4, 4),
            Vector2.one * 0.5f,
            4f
        );

        // 柱
        transform.localScale = new Vector3(0.5f, 4f, 1f);

        pillarRenderer = GetComponent<SpriteRenderer>();
        if (pillarRenderer == null) pillarRenderer = gameObject.AddComponent<SpriteRenderer>();
        pillarRenderer.sprite = square;
        pillarRenderer.color = Color.white;
        pillarRenderer.sortingOrder = 20;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = Vector2.one;

        // 周囲の4つのorb
        for (int i = 0; i < OrbOffsets.Length; i++)
        {
            // 既存の子を再利用（Reset連打時の重複防止）
            Transform existing = transform.Find($"Orb_{i}");
            GameObject orb = existing != null ? existing.gameObject : new GameObject($"Orb_{i}");
            orb.transform.SetParent(transform);
            // localPositionはpillarのscaleの逆数でワールド0.5オフセットに換算
            orb.transform.localPosition = new Vector3(
                OrbOffsets[i].x / 0.5f,
                OrbOffsets[i].y / 4f,
                0f
            );
            orb.transform.localScale = new Vector3(0.3f / 0.5f, 0.3f / 4f, 1f);

            SpriteRenderer sr = orb.GetComponent<SpriteRenderer>();
            if (sr == null) sr = orb.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = Color.white;
            sr.sortingOrder = 20;
            orbRenderers[i] = sr;
        }
    }
}
