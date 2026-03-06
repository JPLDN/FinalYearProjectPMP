using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private bool isGameOver = false;     // corruption max -> game over
    private bool isRespawning = false;   // hazards -> respawn coroutine running
    private Vector3 initialPosition;

    void Start()
    {
        if (corruptionSlider != null)
            corruptionSlider.maxValue = maxCorruption;

        UpdateUI();

        initialPosition = respawnPoint != null ? respawnPoint.position : transform.position;
    }

    public void AddCorruption(float amount)
    {
        if (isGameOver) return; // stop taking damage once game over is triggered

        currentCorruption += amount;
        currentCorruption = Mathf.Clamp(currentCorruption, 0, maxCorruption);

        UpdateUI();

        if (currentCorruption >= maxCorruption)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        if (corruptionSlider != null)
            corruptionSlider.value = currentCorruption;
    }

    private void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("You have been corrupted! Game Over.");

        // Disable player controls when dead
        PlayerMovement move = GetComponent<PlayerMovement>();
        PlayerDamageHandler damage = GetComponent<PlayerDamageHandler>();
        PlayerGlitchPhase glitch = GetComponent<PlayerGlitchPhase>();

        if (move != null) move.enabled = false;
        if (damage != null) damage.enabled = false;
        if (glitch != null) glitch.enabled = false;

        // Freeze physics so the body doesn't keep falling
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Show Game Over UI
        GameOverUI gameOver = FindObjectOfType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowGameOver();
        else
            Debug.LogWarning("GameOverUI not found in scene.");
    }

    public void Respawn()
    {
        if (isGameOver) return;      // don't respawn if game over screen is active
        if (isRespawning) return;    // prevent multiple respawn coroutines

        StartCoroutine(HandleDeathAndRespawn());
    }

    private IEnumerator HandleDeathAndRespawn()
    {
        isRespawning = true;

        PlayerMovement move = GetComponent<PlayerMovement>();
        PlayerDamageHandler damage = GetComponent<PlayerDamageHandler>();
        PlayerGlitchPhase glitch = GetComponent<PlayerGlitchPhase>();

        if (move != null) move.enabled = false;
        if (damage != null) damage.enabled = false;
        if (glitch != null) glitch.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float originalGravity = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.simulated = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        transform.position = initialPosition;

        ResetCorruption(); 

        if (rb != null)
        {
            rb.simulated = true;
            rb.gravityScale = originalGravity;
        }

        if (move != null) move.enabled = true;
        if (damage != null) damage.enabled = true;
        if (glitch != null) glitch.enabled = true;

        isRespawning = false;
    }

    public void ResetCorruption()
    {
        currentCorruption = 0f;
        UpdateUI();
    }
    public void KillInstant()
    {
        if (isGameOver) return; // prevent killing if already game over or respawning

        GameOverUI gameOver = FindObjectOfType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowGameOver();
        else
            Debug.LogWarning("GameOverUI not found in scene.");
    }
}
