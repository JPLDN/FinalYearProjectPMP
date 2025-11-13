using UnityEditor.Tilemaps;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public Transform frontCheck;
    public float wallCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private bool movingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, 0f);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D frontInfo = Physics2D.Raycast(frontCheck.position, Vector2.right * (movingRight ? 1 : -1), wallCheckDistance, groundLayer);

        if (groundInfo.collider == null || frontInfo.collider != null)
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
