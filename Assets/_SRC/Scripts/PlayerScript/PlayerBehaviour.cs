using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;

    [Header("Layer Info")]
    [SerializeField] private LayerMask whatIsGround;
    private Rigidbody2D rb2d;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private PlayerLife playerLife;
    private bool isFacingRight, attacking, blocking;
    private float horizontalInput;
    

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerLife = GetComponent<PlayerLife>();
        isFacingRight = true;
    }

    void Update()
    {
        PlayerInput();
        FlipSprite();
        IsGrounded();   
        AnimationState();
    }

    private void PlayerInput()
    {
        if(!playerLife.takingDamage && !attacking && !blocking && !playerLife.isDeath)
            horizontalInput = Input.GetAxis("Horizontal");
        
        if(Input.GetButton("Jump") && IsGrounded())
        {
            Jump();
        }

        if(Input.GetMouseButtonDown(0) && !attacking && !blocking && IsGrounded())
        {
            Attack();
        }

        if(Input.GetMouseButtonDown(1) && !blocking && IsGrounded())
        {
            Block();
        }

        if(Input.GetMouseButtonUp(1) && blocking && IsGrounded())
        {
            DisableBlock();
        }
    }

    private void FixedUpdate()
    {
        if(!playerLife.takingDamage && !attacking && !blocking && !playerLife.isDeath)
        {
            rb2d.velocity = new Vector2(horizontalInput * moveSpeed, rb2d.velocity.y);
        }
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
    }
    private void FlipSprite()
    {
        if(isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D ground = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.2f, whatIsGround);
        return ground.collider != null;
    }

    private void Attack()
    {
        attacking = true;
    }

    public void DisableAttack()
    {
        attacking = false;
    }

    private void Block()
    {
        blocking = true;
    }

    public void DisableBlock()
    {
        blocking = false;
    }
    private void AnimationState()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isJumping", !IsGrounded());
        anim.SetBool("isAttacking", attacking);
        anim.SetBool("isBlocking", blocking);
    }
}
