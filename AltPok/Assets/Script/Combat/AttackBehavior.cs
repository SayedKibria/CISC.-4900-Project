using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    private AttackData data;
    private Vector2 direction;
    private GameObject owner;

    public void Initialize(AttackData attackData, Vector2 moveDirection, GameObject source)
    {
        data = attackData;
        direction = moveDirection.normalized;
        owner = source;

        // Rotate the attack to face the correct direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Auto destroy after time
        Destroy(gameObject, data.lifetime);
    }

    private void Update()
    {
        if (data.attackType == AttackType.Ranged)
        {
            transform.position += (Vector3)(direction * data.speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner) return;

        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(data.damage);

            // Apply knockback if possible
            KnockbackHandler knockback = collision.GetComponent<KnockbackHandler>();
            if (knockback != null)
            {
                // Knock the target away from the attacker
                Vector2 knockbackDir = (collision.transform.position - owner.transform.position).normalized;
                knockback.ApplyKnockback(knockbackDir);
            }

            Destroy(gameObject); // Destroy the attack after hitting
        }
    }
}