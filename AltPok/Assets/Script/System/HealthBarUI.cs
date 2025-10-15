using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour{
    [SerializeField] private Image fillImage;

    private Health targetHealth;

    public void SetTarget(Health health){
        if (targetHealth != null)
            targetHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);

        targetHealth = health;

        // If there's no health target, hide the UI
        if (targetHealth == null){
            gameObject.SetActive(false);
            return;
        }

        // Show UI and subscribe
        gameObject.SetActive(true);
        targetHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        UpdateHealthBar(targetHealth.CurrentHealthNormalized); // Optional: initialize fill
    }

    private void UpdateHealthBar(float normalizedValue){
        Debug.Log($"HealthBarUI updating fill: {normalizedValue}");
        if (fillImage != null)
            fillImage.fillAmount = normalizedValue;
    }

    private void OnDisable(){
        if (targetHealth != null)
            targetHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }
}
