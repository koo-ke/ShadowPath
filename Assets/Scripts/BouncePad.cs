using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;
    [SerializeField] private Vector2 size = new Vector2(2f, 0.3f);

    public void Configure(Vector2 platformSize)
    {
        size = platformSize;
    }

    private Vector3 originScale;

    private void Start()
    {
        SetupVisual();
        originScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // 上から踏んだ場合のみ弾く
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);

                StopAllCoroutines();
                StartCoroutine(SquishAnimation());
                return;
            }
        }
    }

    private IEnumerator SquishAnimation()
    {
        float duration = 0.08f;
        float elapsed = 0f;

        // 縮む
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = new Vector3(
                originScale.x * Mathf.Lerp(1f, 1.3f, t),
                originScale.y * Mathf.Lerp(1f, 0.4f, t),
                1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // 戻る
        while (elapsed < duration * 2f)
        {
            float t = elapsed / (duration * 2f);
            transform.localScale = new Vector3(
                originScale.x * Mathf.Lerp(1.3f, 1f, t),
                originScale.y * Mathf.Lerp(0.4f, 1f, t),
                1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originScale;
    }

    private void SetupVisual()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1) gameObject.layer = groundLayer;

        transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 4f);
        sr.color = new Color(0x44 / 255f, 0x44 / 255f, 0x44 / 255f);  // #444444
        sr.sortingOrder = 0;

        var col = GetComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }
}
