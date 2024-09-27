using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    private float horizontalInput;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isFacingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        FlipSprite();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        if(Input.GetButton("Jump") && isGrounded)
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
        //anim.SetBool("isJumping", !isGrounded);
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

    private void OnCollisionExit2D(Collision2D other)
    {   
            isGrounded = false;
            anim.SetBool("isJumping", !isGrounded);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            anim.SetBool("isJumping", !isGrounded);
        }
    }
}
