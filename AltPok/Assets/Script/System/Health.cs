using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    private int maxHealth;
    private int currentHealth;

    public UnityEvent<float> OnHealthChanged; // Sends normalized value (0–1)

    public float CurrentHealthNormalized =>
        maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

    // Called from CreatureControl when the creature spawns
    public void Initialize(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealthNormalized);
    }

    public void TakeDamage(int amount)
    {
        if (maxHealth <= 0) return; // safety check

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealthNormalized);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject); // or trigger faint animation later
    }
}