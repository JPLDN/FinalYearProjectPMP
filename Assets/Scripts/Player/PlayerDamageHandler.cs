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

            StartCoroutine(HitFlash());
            Vector2 knockDir = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockDir));
        }
    }
}
