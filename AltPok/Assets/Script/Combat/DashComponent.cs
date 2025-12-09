using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CreatureControl))]
public class DashComponent : MonoBehaviour
{
    private Rigidbody2D rb;
    private CreatureControl control;
    private SpriteRenderer spriteRenderer;
    private CreatureData data;

    private bool isDashing = false;
    private bool isInvincible = false;

    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    private Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        control = GetComponent<CreatureControl>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        data = control.data;
    }

    void Update()
    {
        if (!data.canDash) return;

        HandlePlayerDashInput();

        // Reduce cooldown even if AI calls dash
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void HandlePlayerDashInput()
    {
        if (control.controlType == CreatureControl.ControlType.AI)
            return;

        if (isDashing || cooldownTimer > 0)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            TryDash();
    }

    public void TryDash()
    {
        if (!data.canDash || isDashing || cooldownTimer > 0) return;

        Vector2 inputDir = control.GetMoveDirection();
        if (inputDir == Vector2.zero)
            inputDir = control.GetLastMoveDirection();

        if (inputDir == Vector2.zero)
            inputDir = Vector2.right;

        DashInDirection(inputDir);
    }

    public void DashInDirection(Vector2 direction)
    {
        if (!data.canDash || isDashing || cooldownTimer > 0) return;

        dashDirection = direction.normalized;
        isDashing = true;
        dashTimer = data.dashDuration;
        cooldownTimer = data.dashCooldown;
    }

    void FixedUpdate()
    {
        if (!isDashing) return;

        float dashVelocity = data.dashDistance / data.dashDuration;
        rb.linearVelocity = dashDirection * dashVelocity;

        dashTimer -= Time.fixedDeltaTime;

        float elapsed = data.dashDuration - dashTimer;
        isInvincible = (elapsed >= data.dashInvincStart && elapsed <= data.dashInvincEnd);

        if (data.dashFlash && spriteRenderer != null)
            spriteRenderer.color = isInvincible ? data.dashFlashColor : Color.white;

        if (dashTimer <= 0f)
        {
            isDashing = false;
            isInvincible = false;

            if (data.dashFlash && spriteRenderer != null)
                spriteRenderer.color = Color.white;
        }
    }

    public bool IsInvincible() => isInvincible;
    public bool IsDashing() => isDashing;
}