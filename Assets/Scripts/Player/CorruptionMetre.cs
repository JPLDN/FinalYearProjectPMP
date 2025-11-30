using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionMetre : MonoBehaviour
{
    [Header("Corruption Value")]
    public float maxCorruption = 100f;
    public float currentCorruption = 0f;

    [Header("UI Elements")]
    public Slider corruptionSlider;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1f;

    private bool isDead = false;
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (corruptionSlider != null) 
            corruptionSlider.maxValue = maxCorruption;

        UpdateUI();

        initialPosition = respawnPoint != null ? respawnPoint.position : transform.position;
    }

    void Update()
    {
        
    }
    
    public void AddCorruption(float amount)
    {
        if (isDead) return;

        currentCorruption += amount;
        currentCorruption = Mathf.Clamp(currentCorruption, 0, maxCorruption);

        UpdateUI();

        if (currentCorruption >= maxCorruption)
        {
            Die();
        }
    }

    // Update is called once per frame
    private void UpdateUI()
    {
        if (corruptionSlider != null)
            corruptionSlider.value = currentCorruption;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("You have been corrupted! Respawning...");
        StartCoroutine(HandleDeathAndRespawn());
    }

    private IEnumerator HandleDeathAndRespawn()
    {
        // disables player control when dead
        PlayerMovement move = GetComponent<PlayerMovement>();
        PlayerDamageHandler damage = GetComponent<PlayerDamageHandler>();
        PlayerGlitchPhase glitch = GetComponent<PlayerGlitchPhase>();

        if (move != null) move.enabled = false;
        if (damage != null) damage.enabled = false;
        if (glitch != null) glitch.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Apply death animation or Screen effect here
        yield return new WaitForSeconds(respawnDelay);

        transform.position = initialPosition;

        ResetCorruption();

        if (move != null) move.enabled = true;
        if (damage != null) damage.enabled = true;
        if (glitch != null) glitch.enabled = true;

        isDead = false;
    }
    public void ResetCorruption()
    {
        currentCorruption = 0f;
        UpdateUI();
    }
    public void KillInstant()
    {
        if (isDead) return;

        currentCorruption = maxCorruption;
        UpdateUI();
        Die();
    }
}
