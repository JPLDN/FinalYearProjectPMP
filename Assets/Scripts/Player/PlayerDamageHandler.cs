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
            rb.linearVelocity = new Vector2(direction.x * knockBackForce, knockBackUpForce);
        }

        yield return new WaitForSeconds(knockBackDuration);
        isBeingKnockedBack = false;
    }

    private IEnumerator HitFlash()
    {
        if (sr != null)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
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
            if (glitchPhase != null && glitchPhase.IsInvincible())
            {
                Debug.Log("Player is invincible, no damage taken.");
                return;
            }

            if (corruptionMetre != null)
            {
                corruptionMetre.AddCorruption(corruptionPerHit);
                Debug.Log("Player took damage, corruption increased.");
            }

            StartCoroutine(HitFlicker());
            StartCoroutine(HitFlash());
            Vector2 knockDir = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockDir));
        }
    }
}
