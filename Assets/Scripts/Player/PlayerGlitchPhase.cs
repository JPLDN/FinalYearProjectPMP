using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class PlayerGlitchPhase : MonoBehaviour
{
    [Header("Dash Mechanics")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.2f;
    public float invincibilityTime = 0.18f;

    [Header("Wall Collisions")]
    public LayerMask obstacleLayer;
    public float wallStopOffset = 0.08f;

    [Header("Sprite Stuff")]
    private SpriteRenderer spriteRenderer;
    public Color dashColor = Color.cyan;

    [Header("Collision Settings")]
    public string enemyLayerName = "Enemy";
    private int playerLayer;
    private int enemyLayer;

    [Header("Stun Settings")]
    public bool stunEnemiesOnDash = true;
    public float stunDuration = 1.2f;
    public LayerMask enemyLayerMask;
    public Vector2 stunBoxSize = new Vector2(0.8f, 0.8f);

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
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>(); // Cache the core components
        savedGravity = rb.gravityScale; // Store gravity to reset to this after dash
        if (trail != null) trail.emitting = false; // Disable trail at start

        playerLayer = gameObject.layer;
        enemyLayer = LayerMask.NameToLayer(enemyLayerName); // Cache layer indices for collision toggles
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && canDash && !isDashing)  // Register dash input and checks availability
        {
            StartCoroutine(DoDash());
        }
    }

    void FixedUpdate()
    {
        // While dashing force horizontal movement
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

            TryStunEnemies(); // Continuously check for enemies to stun whule phasing
        }
    }

    private void TryStunEnemies()
    {
        if (!stunEnemiesOnDash) return; // Skip stun logic if disabled

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, stunBoxSize, 0f, enemyLayerMask); // Detect enemies overlapping the player during the dash

        // Apply stun effect to any enemies found
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyStunned stunned = hits[i].GetComponent<EnemyStunned>();
            if (stunned != null)
            {
                stunned.Stun(stunDuration);
            }
        }
    }

    private IEnumerator DoDash()
    {
        // lock dash usage and enter dash state
        canDash = false;
        isDashing = true;
        if (trail != null) trail.emitting = true; // enable trail effect for visual feedback
        isInvincible = true;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true); // Temporarily ignore collision with enemies

        float inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0) dashDirection = Mathf.Sign(inputX); // determine dash direction from player input

        Vector2 startPos = rb.position;
        float maxDistance = dashSpeed * dashDuration; // calc max dash distance

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right * dashDirection, maxDistance, obstacleLayer); // Raycast to check if a wall will be hit during dash

        // Adjust dash duration if a wall is detected
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

        // disable gravity to prevent vertical movement during dash
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // change sprite colour to indicate dash state
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        if (spriteRenderer != null) spriteRenderer.color = dashColor;

        // perform mdash over time using physics 
        float timer = 0f;
        while (timer < actualDuration)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // snap player to stopping point and restore physics
        if (stopAtWall)
        {
            rb.position = wallStopPos;
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = savedGravity;

        if (trail != null) trail.emitting = false; // disable dash visuals

        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        // exit dash state and restore collisions
        isDashing = false;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false; // maintain brief invincibility window after dash

        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // enforce cooldown before next dash
    }

    // expose dash and invincibility states to other systems
    public bool IsInvincible() => isInvincible;
    public bool IsDashing() => isDashing;
}


