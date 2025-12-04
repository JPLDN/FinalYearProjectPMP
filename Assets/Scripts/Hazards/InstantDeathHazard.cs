using UnityEngine;

public class InstantDeathHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CorruptionMetre corruption = other.GetComponent<CorruptionMetre>();
            if (corruption != null)
            {
                corruption.KillInstant();
            }
        }
    }
}
