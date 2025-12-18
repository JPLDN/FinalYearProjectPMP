using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStunned : MonoBehaviour
{
    [Header("Stun Settings")]
    public float defaultStunDuration = 1.2f;

    [Header("Visual Feedback Settings")]
    public SpriteRenderer spriteRenderer;
    public Color stunnedColor = Color.magenta;

    private bool isStunned = false; // tracks if enemy is stunned
    private Coroutine stunRoutine; // ref to running stun coroutine so it can be restarted
    private Color originalColor;

    public bool IsStunned => isStunned; // public read only access to stunned state

    void Awake()
    {
        // if no sprite renderer is assigned in the inspector, attempts to auto apply one from this object
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // cache the original colour to revert post stun color change
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

    }

    public void Stun(float duration = -1f)
    {
        // if no duration is passed in, use the default value
        if (duration <= 0f) duration = defaultStunDuration;

        // Restarts the stun timer if stunned again
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunCoroutine(duration)); // start the stun coroutine
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true; // mark the enemy as stunned

        // change colour to indicate the enemy being stunned
        if (spriteRenderer != null)
            spriteRenderer.color = stunnedColor;

        // wait for the stun duration
        yield return new WaitForSeconds(duration);

        isStunned = false; // end the stun

        // reset back to original colour
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        stunRoutine = null; // clear the coroutine ref
    }
}
