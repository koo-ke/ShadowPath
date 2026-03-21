using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundAcceleration = 15f;
    [SerializeField] private float airAcceleration = 8f;

    [Header("ジャンプ")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("重力")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("クイックステップ")]
    [SerializeField] private float quickStepForce = 15f;
    [SerializeField] private float quickStepCooldown = 0.3f;
    [SerializeField] private float doubleTapWindow = 0.3f;
    [SerializeField] private float quickStepBrakeDuration = 0.2f;
    [SerializeField] private float quickStepBrakeMultiplier = 3f;

    [Header("接地判定")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;

    private float horizontalInput;
    private float prevHorizontalInput;
    private bool isGrounded;
    private bool wasGrounded;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float defaultGravityScale;

    // クイックステップ管理
    private float lastTapDirection;
    private float lastTapTime;
    private float quickStepCooldownTimer;
    private bool isQuickStepping;
    private float quickStepBrakeTimer;

    public event System.Action<float> OnQuickStep;  // 引数: 移動方向 (-1 or 1)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        defaultGravityScale = rb.gravityScale;
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Jump.canceled  += OnJumpCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Jump.canceled  -= OnJumpCanceled;
        inputActions.Player.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
            AudioManager.Instance?.PlaySE("land");

        prevHorizontalInput = horizontalInput;
        horizontalInput = inputActions.Player.Move.ReadValue<Vector2>().x;

        // コヨーテタイムのカウントダウン
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // ジャンプバッファのカウントダウン
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        // クイックステップクールダウンのカウントダウン
        if (quickStepCooldownTimer > 0f)
            quickStepCooldownTimer -= Time.deltaTime;

        // クイックステップブレーキのカウントダウン
        if (isQuickStepping)
        {
            quickStepBrakeTimer -= Time.deltaTime;
            if (quickStepBrakeTimer <= 0f)
                isQuickStepping = false;
        }

        // ジャンプバッファ×コヨーテタイムでジャンプ実行
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            AudioManager.Instance?.PlaySE("jump");
        }

        DetectQuickStep();
    }

    private void DetectQuickStep()
    {
        // 前フレームが0かつ今フレームが非0 → 方向キーが押された瞬間
        bool justPressed = Mathf.Approximately(prevHorizontalInput, 0f) && !Mathf.Approximately(horizontalInput, 0f);
        if (!justPressed) return;

        float direction = Mathf.Sign(horizontalInput);

        if (quickStepCooldownTimer <= 0f
            && direction == lastTapDirection
            && Time.time - lastTapTime <= doubleTapWindow)
        {
            // ダブルタップ確定 → クイックステップ発動
            rb.AddForce(new Vector2(direction * quickStepForce, 0f), ForceMode2D.Impulse);
            quickStepCooldownTimer = quickStepCooldown;
            isQuickStepping = true;
            quickStepBrakeTimer = quickStepBrakeDuration;
            lastTapDirection = 0f;  // リセットして連続発動防止
            AudioManager.Instance?.PlaySE("quickstep");
            OnQuickStep?.Invoke(direction);
        }
        else
        {
            lastTapDirection = direction;
            lastTapTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();
    }

    private void ApplyMovement()
    {
        float targetVelocityX = horizontalInput * moveSpeed;
        float accel = isGrounded ? groundAcceleration : airAcceleration;

        // クイックステップ後・入力なし・地上の場合のみブレーキ強化
        if (isQuickStepping && isGrounded && Mathf.Approximately(horizontalInput, 0f))
            accel *= quickStepBrakeMultiplier;

        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocityX, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = defaultGravityScale * fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0f && !inputActions.Player.Jump.IsPressed())
        {
            rb.gravityScale = defaultGravityScale * lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
