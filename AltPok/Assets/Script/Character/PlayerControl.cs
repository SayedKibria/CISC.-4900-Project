using UnityEngine;

public class PlayerControl : MonoBehaviour, IControllable
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private float moveSpeed;
    public Vector3 playerMoveDirection;
    private Vector2 lastMoveDirection;

    public void EnableControl()
    {
        this.enabled = true;
    }

    public void DisableControl()
    {
        this.enabled = false;
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;

        if (playerMoveDirection != Vector3.zero)
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

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerMoveDirection.x *
        moveSpeed, playerMoveDirection.y * moveSpeed);
    }

    public void ForceFaceDirection(Vector2 dir)
    {
        lastMoveDirection = dir;

        animator.SetBool("isMoving", false);
        animator.SetFloat("moveX", dir.x);
        animator.SetFloat("moveY", dir.y);
    }
}