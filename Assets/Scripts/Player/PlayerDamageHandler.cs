using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    private CorruptionMetre corruptionMetre;
    private PlayerGlitchPhase glitchPhase;

    public float corruptionPerHit = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        corruptionMetre = GetComponent<CorruptionMetre>();
        glitchPhase = GetComponent<PlayerGlitchPhase>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
    }
}
