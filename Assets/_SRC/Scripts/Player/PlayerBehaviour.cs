using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;

    [Header("Player Status")]
    [SerializeField] private int health;

    [Header("Layer Info")]
    [SerializeField] private LayerMask whatIsGround;
    private Rigidbody2D rb2d;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private bool isFacingRight, takingDamage, attacking, isDeath;
    private float horizontalInput;
    

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        isFacingRight = true;
    }

    void Update()
    {
        PlayerIsDeath();
        PlayerInput();
        FlipSprite();
        IsGrounded();   
        AnimationState();
    }

    private void PlayerInput()
    {
        if(!takingDamage && !attacking && !isDeath)
            horizontalInput = Input.GetAxis("Horizontal");
        
        if(Input.GetButton("Jump") && IsGrounded())
        {
            Jump();
        }

        if(Input.GetMouseButtonDown(0) && !attacking && IsGrounded())
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if(!takingDamage && !isDeath)
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

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            health -= damage;
            if(!PlayerIsDeath())
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
                rb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
                DisableAttack();
            }
            else
            {
                rb2d.velocity = Vector2.zero;
            }
        }
    }

    public void DisableDamage()
    { 
        takingDamage = false;
        rb2d.velocity = Vector2.zero;
    }

    private void Attack()
    {
        attacking = true;
    }

    private void DisableAttack()
    {
        attacking = false;
    }

    private void AnimationState()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isJumping", !IsGrounded());
        anim.SetBool("takeDamage", takingDamage);
        anim.SetBool("isAttacking", attacking);
        anim.SetBool("isDying", isDeath);
    }

    public bool PlayerIsDeath()
    {
        if(health <= 0)
            return isDeath = true;
        else
            return isDeath = false;
    }
}
