using UnityEngine;

[CreateAssetMenu(menuName = "Data/CreatureData")]
public class CreatureData : ScriptableObject
{
    [Header("General Info")]
    public string creatureName;
    public Sprite icon;

    [Header("Typing")]
    public ElementType elementType;

    [Header("Stats")]
    public int maxHealth = 100;
    public int attack = 10;            // affects melee
    public int specialAttack = 10;     // affects ranged
    public int defense = 10;           // reduces melee damage taken
    public int specialDefense = 10;    // reduces ranged damage taken
    public float moveSpeed = 3f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Dash Settings")]
    public bool canDash = true;

    public float dashDistance = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Dash Invincibility (Hybrid i-frames)")]
    public float dashInvincStart = 0.05f;
    public float dashInvincEnd = 0.15f;

    [Header("Dash Visuals")]
    public bool dashFlash = true;
    public Color dashFlashColor = new Color(1f, 1f, 1f, 0.6f);


    [Header("Attacks")]
    public AttackInput[] attacks;
}