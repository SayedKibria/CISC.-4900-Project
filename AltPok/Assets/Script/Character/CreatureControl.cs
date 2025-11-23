using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class CreatureControl : MonoBehaviour, IControllable
{
    public enum ControlType
    {
        Player,
        AI
    }

    [Header("Control Settings")]
    public ControlType controlType = ControlType.Player;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] public CreatureData data;

    private Vector3 moveDirection;
    private Vector2 lastMoveDirection;
    private Health health;
    private KnockbackHandler knockback;

    public void EnableControl() => this.enabled = true;
    public void DisableControl() => this.enabled = false;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        knockback = GetComponent<KnockbackHandler>();

        // Initialize health from CreatureData
        if (health != null && data != null)
            health.Initialize(data.maxHealth);
    }

    void Update()
    {
        HandleMovement();
        HandleAttacks();

    }

    // MOVEMENT
    private void HandleMovement()
    {
        // PLAYER INPUT
        if (controlType == ControlType.Player)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(inputX, inputY).normalized;

            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("isMoving", true);
                animator.SetFloat("moveX", inputX);
                animator.SetFloat("moveY", inputY);
                lastMoveDirection = new Vector2(inputX, inputY);
            }
            else
            {
                animator.SetBool("isMoving", false);
                animator.SetFloat("moveX", lastMoveDirection.x);
                animator.SetFloat("moveY", lastMoveDirection.y);
            }
        }
    }

    // PLAYER ATTACK INPUT
    private void HandleAttacks()
    {
        if (controlType == ControlType.AI) return; // AI does NOT use player keys

        if (data == null || data.attacks == null) return;

        foreach (var atkInput in data.attacks)
        {
            if (Input.GetKeyDown(atkInput.key))
                UseAttack(atkInput.attackData);
        }
    }

    // Attack execution (shared by AI + Player)
    private void UseAttack(AttackData attackData)
    {
        if (attackData == null || attackData.prefab == null) return;

        Vector2 direction = lastMoveDirection.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * attackData.hitboxDistance);

        GameObject atkObj = Instantiate(attackData.prefab, spawnPos, Quaternion.identity);
        atkObj.GetComponent<AttackBehavior>().Initialize(attackData, direction, gameObject);
    }

    // PHYSICS MOVEMENT
    void FixedUpdate()
    {
        if (knockback != null && knockback.IsBeingKnockedBack)
            return;

        if (data != null)
            rb.linearVelocity = new Vector2(moveDirection.x * data.moveSpeed, moveDirection.y * data.moveSpeed);
    }

    // METHODS FOR AI CONTROL
    public void ForceMove(Vector2 direction)
    {
        moveDirection = direction;
        lastMoveDirection = direction;

        // Animator update for AI
        if (direction != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void ForceAttack(AttackData attack)
    {
        UseAttack(attack);
    }
}