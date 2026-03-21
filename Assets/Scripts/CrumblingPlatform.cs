using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private float crumbleDelay = 1.0f;
    [SerializeField] private float respawnDelay = 3.0f;
    [SerializeField] private Vector2 size = new Vector2(3f, 0.5f);

    public void Configure(Vector2 platformSize)
    {
        size = platformSize;
    }

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sr;

    private Vector3 originPosition;
    private bool isCrumbling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        originPosition = transform.position;

        SetupVisual();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCrumbling) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // 上から乗った場合のみ崩れる
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                StartCoroutine(CrumbleRoutine());
                return;
            }
        }
    }

    private IEnumerator CrumbleRoutine()
    {
        isCrumbling = true;

        // 点滅で予兆を演出
        float elapsed = 0f;
        float blinkInterval = 0.1f;
        while (elapsed < crumbleDelay)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
            blinkInterval = Mathf.Max(0.05f, blinkInterval - 0.01f);  // 徐々に点滅が速くなる
        }
        sr.enabled = true;

        // 崩落: コライダー無効 → Dynamic に変えて落下
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(respawnDelay);

        // 復帰
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        transform.position = originPosition;
        col.enabled = true;
        isCrumbling = false;
    }

    private void SetupVisual()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1) gameObject.layer = groundLayer;

        transform.localScale = new Vector3(size.x, size.y, 1f);

        sr.sprite = Sprite.Create(
            Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 4f);
        sr.color = new Color(0.15f, 0.15f, 0.2f);  // 少し青みがかった暗いグレー
        sr.sortingOrder = 0;

        col.size = Vector2.one;
    }
}
