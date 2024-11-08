using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float footstepInterval;
    [SerializeField] private float reboundVelocity;

    [Header("Layer Info")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private ParticleSystem dust;
    
    private Rigidbody2D rb2d;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private PlayerLife playerLife;
    private PauseScript pauseGame;

    public bool isFacingRight, canMove;
    private bool isGrounded, attacking, blocking;
    private float horizontalInput;
    private float footstepTimer;

    private float attackCooldown = 0.5f;
    private float lastAttackTime;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    void Start()
    {
        isFacingRight = true;
        canMove = false;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GameObject.FindGameObjectWithTag("FloorDetection").GetComponent<BoxCollider2D>();
        playerLife = GetComponent<PlayerLife>();
        pauseGame = GameObject.FindAnyObjectByType<PauseScript>();
    }

    void Update()
    {
        if(playerLife.isDeath || !canMove || PauseScript.paused) return;

        PlayerInput();
        FlipSprite();
        IsGrounded();   
        AnimationState();
        CheckPlayerSteps();
    }

    private void PlayerInput()
    {
        if(!playerLife.takingDamage && !playerLife.isDeath && !PauseScript.paused)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            if(IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
            if(Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }
            if(coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
            {
                Jump();
                jumpBufferCounter = 0f;
            }
            if(Input.GetButtonUp("Jump") && rb2d.velocity.y > 0f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }
            if(Input.GetButtonDown("Fire1") && IsGrounded() && !attacking)
            {
                Attack(); 
                horizontalInput = 0;
            }
            if (Input.GetButtonDown("Fire2") && IsGrounded() && !blocking)
            {
                Block();
                horizontalInput = 0;
            }
            if (Input.GetButtonUp("Fire2") && blocking)
            {
                DisableBlock();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                pauseGame.SetPauseMenu(!PauseScript.paused);
            }
        }
    }

    private void FixedUpdate()
    {
        if(!playerLife.takingDamage && !attacking && !blocking && !playerLife.isDeath)
        {
            rb2d.velocity = new Vector2(horizontalInput * moveSpeed, rb2d.velocity.y);
        }
    }

    public void Rebound()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, reboundVelocity);
    }

    public void EnemyRebound()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
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
        return isGrounded = ground.collider != null;
    }

    private void CheckPlayerSteps()
    {
        if (!isGrounded || Mathf.Abs(horizontalInput) == 0 || attacking || blocking) return;

        if (isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                SoundManager.Instance.PlaySound2D("Footstep");
                CreateDust();
                footstepTimer = footstepInterval;
            }
        }
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown || attacking) return;
        attacking = true;
        lastAttackTime = Time.time;
        rb2d.velocity = Vector2.zero;
        anim.SetTrigger("Attack");
        StartCoroutine(AttackSlashSound());
        StartCoroutine(DisableAttack());
    }

    private IEnumerator AttackSlashSound()
    {
        yield return new WaitForSeconds(0.16f);
        SoundManager.Instance.PlaySound2D("SwordSlash");
    }

    public IEnumerator DisableAttack()
    {
        yield return new WaitForSeconds(0.5f);
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