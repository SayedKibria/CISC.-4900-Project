using UnityEngine;
using System.Collections.Generic;


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
    private Dictionary<AttackData, float> cooldownTimers = new Dictionary<AttackData, float>();

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

        cooldownTimers.Clear();
        if (data != null && data.attacks != null)
        {
            foreach (var atk in data.attacks)
                cooldownTimers[atk.attackData] = 0f; // no cooldown at start
        }
    }

    void Update()
    {
        HandleMovement();
        HandleAttacks();

    }

    // Movement
    private void HandleMovement()
    {
        // Player Input
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

    public Vector2 GetMoveDirection() => new Vector2(moveDirection.x, moveDirection.y);
    public Vector2 GetLastMoveDirection() => lastMoveDirection;

    // Player Attack Input
    private void HandleAttacks()
    {
        if (controlType == ControlType.AI) return; // AI does not use player keys

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

        // Cooldown Check
        if (cooldownTimers.ContainsKey(attackData))
        {
            if (Time.time < cooldownTimers[attackData])
                return; // still cooling down

            cooldownTimers[attackData] = Time.time + attackData.cooldown; // set next ready time
        }

        Vector2 direction = lastMoveDirection.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * attackData.hitboxDistance);

        GameObject atkObj = Instantiate(attackData.prefab, spawnPos, Quaternion.identity);
        atkObj.GetComponent<AttackBehavior>().Initialize(attackData, direction, gameObject);
    }

    // Physics Movement
    void FixedUpdate()
    {
        if (knockback != null && knockback.IsBeingKnockedBack)
            return;

        if (data != null)
            rb.linearVelocity = new Vector2(moveDirection.x * data.moveSpeed, moveDirection.y * data.moveSpeed);
    }

    // Methods For AI Control
    public void ForceMove(Vector2 direction)
    {
        moveDirection = direction;

        // Only update lastMoveDirection if the AI is actually moving
        if (direction != Vector2.zero)
            lastMoveDirection = direction;

        // Animator update
        if (direction != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
        else
        {
            animator.SetBool("isMoving", false);

            // Keep animator facing last real direction
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }
    }
    public void ForceAttackWithDirection(AttackData attack, Vector2 direction)
    {
        if (cooldownTimers.ContainsKey(attack))
            if (Time.time < cooldownTimers[attack]) return;

        cooldownTimers[attack] = Time.time + attack.cooldown;

        if (direction != Vector2.zero)
            lastMoveDirection = direction; // keep orientation consistent

        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * attack.hitboxDistance);

        GameObject atkObj = Instantiate(attack.prefab, spawnPos, Quaternion.identity);

        atkObj.GetComponent<AttackBehavior>().Initialize(attack, direction.normalized, gameObject);
    }



    public void ForceAttack(AttackData attack)
    {
        UseAttack(attack);
    }
}