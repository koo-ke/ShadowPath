using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPoint = new Vector2(0f, 1f);
    [SerializeField] private float deathY = -10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y < deathY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
    }
}
