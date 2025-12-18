using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;

    private EnemyStunned stunnable; // ref to stun component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        stunnable = GetComponent<EnemyStunned>(); // attempt to get EnemyStunnable component on this enemy.
    }

    private void FixedUpdate()
    {
        // if enemy is stunned, do not move
        if (stunnable != null && stunnable.IsStunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * speed, rb.linearVelocity.y); // normal left & right movement

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer); // check if there is ground ahead

        // if no ground ahead, flip direction
        if (groundInfo.collider == false)
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // flip when hitting a wall
        if (collision.gameObject.CompareTag("Ground"))
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
