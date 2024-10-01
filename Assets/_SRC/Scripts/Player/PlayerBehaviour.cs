using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;

    [Header("Layer Info")]
    [SerializeField] private LayerMask layerGround;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private bool isFacingRight = true;
    private float horizontalInput;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        PlayerInput();
        FlipSprite();
        IsGrounded();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        if(Input.GetButton("Jump") && IsGrounded())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        anim.SetFloat("xVelocity", math.abs(rb.velocity.x));
        anim.SetFloat("yVelocity", rb.velocity.y);

    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
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
        RaycastHit2D ground = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.2f, layerGround);
        if(ground.collider != null)
            anim.SetBool("isJumping", false);
        else
            anim.SetBool("isJumping", true);
        
        return ground.collider != null;
    }
}
