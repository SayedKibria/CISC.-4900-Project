using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public HealthBarUI playerHealthBarUI;
    public HealthBarUI enemyHealthBarUI;

    public static BattleUIManager Instance
    {
        get; private set;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterCreature(Health health, bool isEnemy)
    {
        if (isEnemy)
            SetEnemyTarget(health);
        else
            SetPlayerTarget(health);
    }


    public void SetPlayerTarget(Health health)
    {
        playerHealthBarUI.SetTarget(health);
    }

    public void SetEnemyTarget(Health health)
    {
        enemyHealthBarUI.SetTarget(health);
    }
}