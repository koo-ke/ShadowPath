using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2 pointBOffset = new Vector2(4f, 0f);
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector2 size = new Vector2(4f, 0.5f);

    public void Configure(Vector2 bOffset, Vector2 platformSize)
    {
        pointBOffset = bOffset;
        size = platformSize;
    }

    private Rigidbody2D rb;
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 target;

    private bool isPlayerOn;
    private Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        pointA = rb.position;
        pointB = pointA + pointBOffset;
        target = pointB;

        SetupVisual();
    }

    private void FixedUpdate()
    {
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        Vector2 delta = next - rb.position;

        rb.MovePosition(next);

        if (Vector2.Distance(next, target) < 0.01f)
            target = (target == pointA) ? pointB : pointA;

        if (isPlayerOn && playerTransform != null)
        {
            playerTransform.position += new Vector3(delta.x, delta.y, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"[MovingPlatform] 衝突: {col.gameObject.name}, Tag: {col.gameObject.tag}");

        if (col.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;
            playerTransform = col.transform;
            Debug.Log("[MovingPlatform] Player乗った");
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
            playerTransform = null;
            Debug.Log("[MovingPlatform] Player降りた");
        }
    }

    private void SetupVisual()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1) gameObject.layer = groundLayer;

        transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 4f);
        sr.color = Color.black;
        sr.sortingOrder = 0;

        var col = GetComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying
            ? pointA
            : (Vector2)transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + pointBOffset);
        Gizmos.DrawWireSphere(origin, 0.15f);
        Gizmos.DrawWireSphere(origin + pointBOffset, 0.15f);
    }
}
