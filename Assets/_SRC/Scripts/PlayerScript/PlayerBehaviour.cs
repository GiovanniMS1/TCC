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
    private bool isFacingRight, isGrounded, attacking, blocking;
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
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
        
        if(Input.GetButtonDown("Jump") && !playerLife.takingDamage && !attacking && !blocking && isGrounded && !playerLife.isDeath)
        {
            Jump();
        }

        if(Input.GetKey(KeyCode.E) && !attacking && !blocking && isGrounded)
        {
            Attack();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !blocking && isGrounded)
        {
            Block();
        }

        if(Input.GetKeyUp(KeyCode.LeftShift) && blocking && isGrounded)
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
        if(ground.collider != null)
            return isGrounded = true;
        else
            return isGrounded = false;
    }

    private void Attack()
    {
        attacking = true;
        rb2d.velocity = Vector2.zero;
    }

    public void DisableAttack()
    {
        attacking = false;
    }

    private void Block()
    {
        blocking = true;
        rb2d.velocity = Vector2.zero;
    }

    public void DisableBlock()
    {
        blocking = false;
    }
    private void AnimationState()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isJumping", !isGrounded);
        anim.SetBool("isAttacking", attacking);
        anim.SetBool("isBlocking", blocking);
    }
}
