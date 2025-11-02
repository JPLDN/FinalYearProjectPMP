using UnityEngine;
using System.Collections;

public class PlayerGlitchPhase : MonoBehaviour
{
    public float dashSpeed = 30f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.2f;
    public float invincibilityTime = 0.18f;

    public LayerMask obstacleLayer;
    public float wallStopOffset = 0.08f;

    public SpriteRenderer spriteRenderer;
    public Color dashColor = Color.cyan;

    private Rigidbody2D rb;
    private bool canDash = true;
    private bool isDashing = false;
    private bool isInvincible = false;
    private float dashDirection = 1f;
    private float savedGravity;

    public TrailRenderer trail;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        savedGravity = rb.gravityScale;
        if (trail != null) trail.emitting = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(DoDash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        }
    }

    private IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;
        if (trail != null) trail.emitting = true;
        isInvincible = true;

        float inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0) dashDirection = Mathf.Sign(inputX);

        Vector2 startPos = rb.position;
        float maxDistance = dashSpeed * dashDuration;

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right * dashDirection, maxDistance, obstacleLayer);

        float actualDuration = dashDuration;
        bool stopAtWall = false;
        Vector2 wallStopPos = startPos;

        if (hit.collider != null)
        {
            stopAtWall = true;
            float hitDist = Mathf.Max(0.01f, hit.distance - wallStopOffset);
            actualDuration = hitDist / dashSpeed;
            wallStopPos = startPos + Vector2.right * dashDirection * hitDist;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        if (spriteRenderer != null) spriteRenderer.color = dashColor;

        float timer = 0f;
        while (timer < actualDuration)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (stopAtWall)
        {
            rb.position = wallStopPos;
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = savedGravity;

        if (trail != null) trail.emitting = false;

        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        isDashing = false;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool IsInvincible() => isInvincible;
    public bool IsDashing() => isDashing;
}


