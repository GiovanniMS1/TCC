using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float footstepInterval;

    [Header("Layer Info")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private ParticleSystem dust;
    private Rigidbody2D rb2d;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private PlayerLife playerLife;
    public bool isFacingRight, isGrounded, attacking, blocking;
    private bool canMove = false;
    private float horizontalInput;
    private float footstepTimer;
    private PauseScript pauseGame;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerLife = GetComponent<PlayerLife>();
        isFacingRight = true;
        pauseGame = GameObject.FindAnyObjectByType<PauseScript>();
    }

    void Update()
    {
        if(playerLife.isDeath || !canMove) return;
        PlayerInput();
        FlipSprite();
        IsGrounded();   
        AnimationState();
        CheckPlayerSteps();
    }

    private void PlayerInput()
    {
        if(!playerLife.takingDamage && !attacking && !blocking && !playerLife.isDeath && !PauseScript.paused)
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
        
        if(Input.GetButtonDown("Jump") && !playerLife.takingDamage && !attacking && !blocking && isGrounded && !playerLife.isDeath && !PauseScript.paused)
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.E) && !attacking && !blocking && isGrounded && !PauseScript.paused)
        {
            Attack();
            horizontalInput = 0;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !blocking && isGrounded && !PauseScript.paused)
        {
            Block();
            horizontalInput = 0;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift) && blocking && isGrounded && !PauseScript.paused)
        {
            DisableBlock();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame.SetPauseMenu(!PauseScript.paused);
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
        CreateDust();
        SoundManager.Instance.PlaySound2D("Jumping");
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

    private void CheckPlayerSteps()
    {
        if (!isGrounded || Mathf.Abs(horizontalInput) == 0) return;
        if (isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                SoundManager.Instance.PlaySound2D("Footstep");
                footstepTimer = footstepInterval;
            }
        }
    }

    private void Attack()
    {
        attacking = true;
        rb2d.velocity = Vector2.zero;
        SoundManager.Instance.PlaySound2D("SwordSlash");
        StartCoroutine(DisableAttack());
    }

    public IEnumerator DisableAttack()
    {
        yield return new WaitForSeconds(0.45f);
        attacking = false;
    }

    private void Block()
    {
        SoundManager.Instance.PlaySound2D("PullShield");
        blocking = true;
        rb2d.velocity = Vector2.zero;
    }

    public void DisableBlock()
    {
        StartCoroutine(DisableBlockDelay());
    }

    private IEnumerator DisableBlockDelay()
    {
        yield return new WaitForSeconds(0.1f);
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

    private void CreateDust()
    {
        dust.Play();
    }

    public void EnablePlayerControl()
    {
        canMove = true;
    }

    public void DisablePlayerControl()
    {
        canMove = false;
    }

}
