using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("スプライト")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] runSprites;
    [SerializeField] private Sprite[] jumpSprites;
    [SerializeField] private Sprite[] fallSprites;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private Sprite[] dashSprites;

    [Header("フレームレート")]
    [SerializeField] private float frameRate = 10f;

    private SpriteRenderer sr;
    private PlayerController player;

    private enum AnimState { Idle, Run, Jump, Fall, Dash, Death }
    private AnimState currentState;
    private AnimState prevState;

    private float frameTimer;
    private int frameIndex;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player == null) return;

        prevState = currentState;
        currentState = ResolveState();

        // ステート切り替わり時にフレームをリセット
        if (currentState != prevState)
        {
            frameIndex = 0;
            frameTimer = 0f;
        }

        // 向き（水平入力がある時だけ更新）
        if (!Mathf.Approximately(player.HorizontalInput, 0f))
            sr.flipX = player.HorizontalInput < 0f;

        AdvanceFrame();
    }

    private AnimState ResolveState()
    {
        if (player.IsDead) return AnimState.Death;
        if (player.IsQuickStepping) return AnimState.Dash;
        if (!player.IsGrounded)
            return player.VelocityY > 0f ? AnimState.Jump : AnimState.Fall;
        return !Mathf.Approximately(player.HorizontalInput, 0f) ? AnimState.Run : AnimState.Idle;
    }

    private void AdvanceFrame()
    {
        Sprite[] sprites = GetSprites(currentState);
        if (sprites == null || sprites.Length == 0) return;

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(frameRate, 0.01f);

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            bool loops = currentState == AnimState.Idle || currentState == AnimState.Run;

            if (loops)
            {
                frameIndex = (frameIndex + 1) % sprites.Length;
            }
            else
            {
                // ループなし：最後のフレームで停止
                frameIndex = Mathf.Min(frameIndex + 1, sprites.Length - 1);
            }
        }

        if (sprites[frameIndex] != null)
            sr.sprite = sprites[frameIndex];
    }

    private Sprite[] GetSprites(AnimState state) => state switch
    {
        AnimState.Idle  => idleSprites,
        AnimState.Run   => runSprites,
        AnimState.Jump  => jumpSprites,
        AnimState.Fall  => fallSprites,
        AnimState.Dash  => dashSprites,
        AnimState.Death => deathSprites,
        _               => null,
    };
}
