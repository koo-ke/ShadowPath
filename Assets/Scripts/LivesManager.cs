using System;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }

    [SerializeField] private int maxLives = 3;

    public int CurrentLives { get; private set; }

    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private Vector3 playerStartPosition;

    private void Awake()
    {
        Instance = this;
        CurrentLives = maxLives;
    }

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
            playerStartPosition = player.transform.position;
        }
        else
        {
            Debug.LogWarning("[LivesManager] 'Player' タグのオブジェクトが見つかりません。");
        }
    }

    public void LoseLife()
    {
        if (CurrentLives <= 0) return;

        CurrentLives--;
        OnLivesChanged?.Invoke(CurrentLives);
        AudioManager.Instance?.PlaySE("death");

        if (CurrentLives <= 0)
            OnGameOver?.Invoke();
        else
            RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
        if (playerTransform != null)
            playerTransform.position = playerStartPosition;
    }
}
