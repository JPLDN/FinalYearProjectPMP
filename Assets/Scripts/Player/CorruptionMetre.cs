using UnityEngine;
using UnityEngine.UI;

public class CorruptionMetre : MonoBehaviour
{
    public float maxCorruption = 100f;
    public float currentCorruption = 0f;

    public Slider corruptionSlider;

    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (corruptionSlider != null) 
            corruptionSlider.maxValue = maxCorruption;

        UpdateUI();
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
        isDead = true;
        Debug.Log("You have been corrupted!");
        // Implement death logic here (e.g., trigger game over screen) + Trigger respawn, animations etc
    }

    public void ResetCorruption()
    {
        currentCorruption = 0f;
        isDead = false;
        UpdateUI();
    }
}
