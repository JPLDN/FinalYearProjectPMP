using UnityEngine;
using System.Collections;

public class PlayerDamageHandler : MonoBehaviour
{
    private CorruptionMetre corruptionMetre;
    private PlayerGlitchPhase glitchPhase;

    public float corruptionPerHit = 20f;

    [Header("Knockback Values")]
    public float knockBackForce = 10f;
    public float knockBackUpForce = 5f;
    public float knockBackDuration = 0.5f;

    [HideInInspector]
    public bool isBeingKnockedBack = false;

    [Header("Flicker Config")]
    public float flickerDuration = 0.5f;
    public float flickerSpeed = 0.2f;

    [Header("Contact Damage")]
    public float contactDamageCooldown = 0.6f;
    private float nextDamageTime = 0f;

    [Header("Flash Effect")]
    public float flashDuration = 0.2f;
    private SpriteRenderer sr;
    private Color originalColor;



    void Start()
    {
        corruptionMetre = GetComponent<CorruptionMetre>();
        glitchPhase = GetComponent<PlayerGlitchPhase>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalColor = sr.color;
    }
    void Update()
    {
        
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isBeingKnockedBack = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = new Vector2(direction.x * knockBackForce, knockBackUpForce); // Knockback
        }

        yield return new WaitForSeconds(knockBackDuration); // Cooldown for being knocked back
        isBeingKnockedBack = false;
    }

    private IEnumerator HitFlash()
    {
        if (sr != null)
        {
            sr.color = Color.white; // changing the colour upon being hit
            yield return new WaitForSeconds(flashDuration); // duration of the flash
            sr.color = originalColor; // return back to original colour
        }
    }

    private IEnumerator HitFlicker()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.2f); // go invisible
            yield return new WaitForSeconds(flickerSpeed);

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // go visible
            yield return new WaitForSeconds(flickerSpeed);

            elapsed += flickerSpeed * 2f;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Hazard"))
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                EnemyStunned stunned = collision.collider.GetComponent<EnemyStunned>(); // Enemy is stunned, player should not take damage whilst they are stunned
                if (stunned != null && stunned.IsStunned)
                {
                    Debug.Log("Enemy is Stunned!");
                    return;
                }
            }

            // Invincibility Check
            if (glitchPhase != null && glitchPhase.IsInvincible())
            {
                Debug.Log("Player is invincible, no damage taken.");
                return;
            }

            // Adding Corruption Check
            if (corruptionMetre != null)
            {
                corruptionMetre.AddCorruption(corruptionPerHit);
                Debug.Log("Player took damage, corruption increased.");
            }

            // hit feedback + knock back
            StartCoroutine(HitFlicker());
            StartCoroutine(HitFlash());
            Vector2 knockDir = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockDir));
        }
    }
}
