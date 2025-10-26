using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Movement
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    // Ground check
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    // Jump Refinements
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private Rigidbody2D rb;
    private float moveInput;
    private bool facingRight = true;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && !facingRight)
        {
            Flip();
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

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
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
