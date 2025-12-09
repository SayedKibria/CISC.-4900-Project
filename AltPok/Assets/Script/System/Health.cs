using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    private int maxHealth;
    private int currentHealth;

    public UnityEvent<float> OnHealthChanged; // Sends normalized HP
    public UnityEvent OnDeath;                // Called when HP reaches 0

    public float CurrentHealthNormalized =>
        maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

    // Initialize max HP
    public void Initialize(int newMaxHealth)
    {
        maxHealth = newMaxHealth;

        // If currentHealth has not been set yet, set to max
        if (currentHealth <= 0)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(CurrentHealthNormalized);
    }

    // Set current HP explicitly used when resummoning
    public void SetCurrentHealth(int newHP)
    {
        currentHealth = Mathf.Clamp(newHP, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealthNormalized);
    }

    public int GetMaxHealth() => maxHealth;

    public int GetCurrentHealth() => currentHealth;

    public void TakeDamage(int amount)
    {
        if (maxHealth <= 0) return;

        DashComponent dash = GetComponent<DashComponent>();
        if (dash != null && dash.IsInvincible())
            return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealthNormalized);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        OnDeath?.Invoke(); // notify system

        Destroy(gameObject);
    }
}