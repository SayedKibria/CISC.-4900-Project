using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public UnityEvent<float> OnHealthChanged; // Sends normalized value (0–1)

    public float CurrentHealthNormalized{
        get {
            return currentHealth / (float)maxHealth; 
        }
    }

    void Awake(){
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    public void TakeDamage(int amount){
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealthNormalized);

        if (currentHealth <= 0){
            Die();
        }
    }

    private void Die(){
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject); // Or add fainting logic later
    }
}