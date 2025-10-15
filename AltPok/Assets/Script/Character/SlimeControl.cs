using UnityEngine;

public class SlimeControl : MonoBehaviour, IControllable{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;

    [Header("Attacks")]
    [SerializeField]
    private AttackInput[] attackInputs;  // Array of attacks assigned via Inspector

    private Vector3 playerMoveDirection;
    private Vector2 lastMoveDirection;

    public void EnableControl() => this.enabled = true;
    public void DisableControl() => this.enabled = false;

    void Update(){
        HandleMovement();
        HandleAttacks();

        //test knockback + health

        if (Input.GetKeyDown(KeyCode.K)){
            // Apply damage
            var health = GetComponent<Health>();
            if (health != null){
                health.TakeDamage(10);
                Debug.Log("Slime took 10 damage");
            }
            else{
                Debug.LogWarning("No Health component found on slime!");
            }

            // Apply knockback
            var knockback = GetComponent<KnockbackHandler>();
            if (knockback != null){
                knockback.ApplyKnockback(Vector2.right); // Push left
                Debug.Log("Knockback applied to slime!");
            }
            else{
                Debug.LogWarning("No KnockbackHandler found on slime!");
            }
        }
        //testing testing
    }

    private void HandleMovement(){
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;

        if (playerMoveDirection != Vector3.zero){
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", inputX);
            animator.SetFloat("moveY", inputY);
            lastMoveDirection = new Vector2(inputX, inputY);
        }
        else{
            animator.SetBool("isMoving", false);
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }
    }

    private void HandleAttacks(){
        foreach (var attackInput in attackInputs){
            if (Input.GetKeyDown(attackInput.key)){
                UseAttack(attackInput.attackData);
            }
        }
    }

    private void UseAttack(AttackData attackData){
        if (attackData == null || attackData.prefab == null) return;

        Vector2 direction = lastMoveDirection.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * attackData.hitboxDistance);

        GameObject atkObj = Instantiate(attackData.prefab, spawnPos, Quaternion.identity);
        atkObj.GetComponent<AttackBehavior>().Initialize(attackData, direction, gameObject);
    }

    void FixedUpdate(){
    // Prevent movement during knockback
    var knockback = GetComponent<KnockbackHandler>();
    if (knockback != null && knockback.IsBeingKnockedBack)
        return;

    rb.linearVelocity = new Vector2(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }

}
