using UnityEngine;

public class RegisterEnemyHealthBar : MonoBehaviour{
    void Start(){
        var health = GetComponent<Health>();
        if (health != null && BattleUIManager.Instance != null){
            BattleUIManager.Instance.RegisterCreature(health, true);
            Debug.Log("Enemy registered to health bar");
        }
        else{
            Debug.LogWarning("Could not register enemy health bar");
        }
    }
}
