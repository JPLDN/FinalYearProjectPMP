using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Refinements")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;

    [Header("Jump Gravity")]
    public float normalGravity = 2f;
    public float fallGravity = 4.5f;
    public float jumpCutGravity = 3f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private Rigidbody2D rb;
    private float moveInput;
    private bool facingRight = true;
    private bool isGrounded;
    private bool isSprinting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;
    }

    // Update is called once per frame
    void Update()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        moveInput = Input.GetAxisRaw("Horizontal");

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        if (!GetComponent<PlayerDamageHandler>().isBeingKnockedBack)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRadius, groundLayer);
        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }

        if (!Input.GetKey(KeyCode.Space) && rb.linearVelocity.y > 0.5f)
        {
            rb.gravityScale = jumpCutGravity;
        }
        else if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
