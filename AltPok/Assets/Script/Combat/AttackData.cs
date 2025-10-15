using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject{
    public string moveName;
    public AttackType attackType;
    public ElementType elementType;

    public int damage;
    public float speed;
    public float lifetime = 1f;
    public float hitboxDistance = 0.5f;

    public GameObject prefab;
}