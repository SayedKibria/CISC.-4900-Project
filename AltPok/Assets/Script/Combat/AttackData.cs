using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject
{
    public string moveName;
    public AttackType attackType;   // Melee = physical, Ranged = special
    public ElementType elementType;

    public int damage;
    public float speed;
    public float lifetime = 1f;
    public float hitboxDistance = 0.5f;
    public float cooldown = 1f;
    public GameObject prefab;
}