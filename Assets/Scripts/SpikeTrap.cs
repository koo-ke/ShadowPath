using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Vector2 respawnPosition = new Vector2(0f, 1f);
    [SerializeField] private int spikeCount = 5;
    [SerializeField] private float spikeWidth = 0.3f;
    [SerializeField] private float spikeHeight = 0.6f;

    // totalWidth から spikeCount を自動計算して設定
    public void Configure(float totalWidth, float height)
    {
        spikeHeight = height;
        spikeCount = Mathf.Max(1, Mathf.RoundToInt(totalWidth / spikeWidth));
    }

    private void Start()
    {
        SetupVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        other.transform.position = new Vector3(respawnPosition.x, respawnPosition.y, 0f);
    }

    private void SetupVisual()
    {
        // トゲを spikeCount 本並べる
        Sprite square = Sprite.Create(
            Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 4f);
        Color spikeColor = new Color(0x2a / 255f, 0f, 0f);  // #2a0000

        float totalWidth = spikeCount * spikeWidth;
        float startX = -(totalWidth - spikeWidth) * 0.5f;

        for (int i = 0; i < spikeCount; i++)
        {
            GameObject spike = new GameObject($"Spike_{i}");
            spike.transform.SetParent(transform);
            // ローカル座標で並べる（親のscaleは1,1,1）
            spike.transform.localPosition = new Vector3(
                startX + i * spikeWidth, spikeHeight * 0.5f, 0f);
            spike.transform.localScale = new Vector3(spikeWidth * 0.7f, spikeHeight, 1f);

            SpriteRenderer sr = spike.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = spikeColor;
            sr.sortingOrder = 1;
        }

        // 全体のトリガーコライダー
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(totalWidth, spikeHeight);
        col.offset = new Vector2(0f, spikeHeight * 0.5f);
    }
}
