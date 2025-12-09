using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHandler : MonoBehaviour
{
    private float knockbackForce = 30f;
    private float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isBeingKnockedBack = false;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Initialize(float force, float duration)
    {
        knockbackForce = force;
        knockbackDuration = duration;
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (!isBeingKnockedBack)
            StartCoroutine(DoKnockback(direction));
    }

    private System.Collections.IEnumerator DoKnockback(Vector2 direction)
    {
        isBeingKnockedBack = true;
        rb.linearVelocity = direction.normalized * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        isBeingKnockedBack = false;
    }

    public bool IsBeingKnockedBack => isBeingKnockedBack;
}