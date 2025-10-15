using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHandler : MonoBehaviour{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isBeingKnockedBack = false;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction){
        if (!isBeingKnockedBack){
            StartCoroutine(DoKnockback(direction));
        }
    }

    private System.Collections.IEnumerator DoKnockback(Vector2 direction){
        isBeingKnockedBack = true;
        rb.linearVelocity = direction.normalized * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isBeingKnockedBack = false;
    }
    public bool IsBeingKnockedBack => isBeingKnockedBack;

}