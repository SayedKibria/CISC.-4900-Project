using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    private AttackData data;
    private Vector2 direction;
    private GameObject owner;

    private bool followOwner = false;
    private Vector3 localOffset;

    public void Initialize(AttackData attackData, Vector2 moveDirection, GameObject source)
    {
        data = attackData;
        direction = moveDirection.normalized;
        owner = source;

        // Rotate the attack to face initial direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Melee Trim Logic
        if (data.attackType == AttackType.Melee)
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box != null)
                ArenaBoundary.Instance.TrimMeleeHitbox(box, transform);

            // enable following once trimmed
            followOwner = true;
            localOffset = transform.position - owner.transform.position;
        }

        Destroy(gameObject, data.lifetime);
    }


    private void Update()
    {
        // Ranged Begavior
        if (data.attackType == AttackType.Ranged)
        {
            transform.position += (Vector3)(direction * data.speed * Time.deltaTime);
            return;
        }

        // Melee Follow
        if (followOwner)
        {
            // stay attached
            transform.position = owner.transform.position + localOffset;

            // rotate with current direction
            CreatureControl cc = owner.GetComponent<CreatureControl>();
            if (cc != null)
            {
                Vector2 face = cc.GetLastMoveDirection();
                if (face.sqrMagnitude > 0.01f)
                {
                    float angle = Mathf.Atan2(face.y, face.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaWall"))
        {
            ArenaBoundary.Instance.HandleRangedHit(gameObject);
            return;
        }

        if (collision.gameObject == owner) return;

        CreatureControl ownerControl = owner.GetComponent<CreatureControl>();
        CreatureControl targetControl = collision.GetComponent<CreatureControl>();

        if (ownerControl == null || targetControl == null)
            return;

        CreatureData ownerStats = ownerControl.data;
        CreatureData targetStats = targetControl.data;

        // 1. Physical or Special
        bool isPhysical = (data.attackType == AttackType.Melee);

        int A = isPhysical ? ownerStats.attack : ownerStats.specialAttack;
        int D = isPhysical ? targetStats.defense : targetStats.specialDefense;

        // 2. STAB
        float STAB = (ownerStats.elementType == data.elementType) ? 1.5f : 1f;

        // 3. Type effectiveness
        float typeMultiplier = TypeChart.GetEffectiveness(data.elementType, targetStats.elementType);

        // 4. Random factor
        float randomFactor = Random.Range(0.85f, 1f);

        // 5. Damage formula
        float baseDamage = ((2f * data.damage * A / D) / 5f) + 2f;

        int finalDamage = Mathf.FloorToInt(
            baseDamage * STAB * typeMultiplier * randomFactor);

        // Minimum damage of 1
        finalDamage = Mathf.Max(finalDamage, 1);

        // Apply damage
        collision.GetComponent<IDamageable>()?.TakeDamage(finalDamage);

        // Knockback
        KnockbackHandler knockback = collision.GetComponent<KnockbackHandler>();
        if (knockback != null)
        {
            Vector2 knockbackDir =
                (collision.transform.position - owner.transform.position).normalized;
            knockback.ApplyKnockback(knockbackDir);
        }

        Destroy(gameObject);
    }
}