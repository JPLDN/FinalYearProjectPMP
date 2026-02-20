using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public float chaseSpeed = 2.5f;
    public float detectionRange = 5f; // distance to detect the player
    public float stopDistance = 0.8f; // how close the enemy gets to the player before stopping

    [Header("Wall/Edge Detection")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.8f; // distance to check for ground ahead
    public LayerMask groundLayer; // layer for ground/platforms

    public Transform wallCheck; // position to check for walls ahead
    public float wallCheckDistance = 0.2f;
    public LayerMask obstacleLayer; // layer for walls and obstacles

    [Header("Target")]
    public string playerTag = "Player";

    private Rigidbody2D rb;
    private Transform player;
    private EnemyStunned stunned;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stunned = GetComponent<EnemyStunned>();
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = player.transform;
    }

    private void FixedUpdate()
    {
        if (stunned != null && stunned.IsStunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // stop horizontal movement when stunned
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // if there's no player, stop movement
        }

        float dist = Vector2.Distance(transform.position, player.position);


        // chases the player when in detection range
        if (dist > detectionRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dx = player.position.x - transform.position.x;
        if (Mathf.Abs(dx) < stopDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // stop moving if close enough to the player
            return;
        }

        float dir = Mathf.Sign(dx);

        bool groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer); // ground edge check

        bool wallAhead = Physics2D.Raycast(wallCheck.position, Vector2.right * dir, wallCheckDistance, obstacleLayer); // wall in front check

        if (!groundAhead || wallAhead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        transform.localScale = s;
    }
}
