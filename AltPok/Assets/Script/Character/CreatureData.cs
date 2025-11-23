using UnityEngine;

[CreateAssetMenu(menuName = "Data/CreatureData")]
public class CreatureData : ScriptableObject
{
    [Header("General Info")]
    public string creatureName;
    public Sprite icon;

    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 3f;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Attacks")]
    public AttackInput[] attacks;
}