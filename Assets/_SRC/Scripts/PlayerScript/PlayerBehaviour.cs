using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private CapsuleCollider2D capsuleCollider2D;
    private PlayerLife playerLife;
    private PauseScript pauseGame;

    public bool isFacingRight, canMove;
    private bool isGrounded, attacking, blocking;
    private float horizontalInput;
    private float footstepTimer;

    private float attackCooldown = 0.5f;
    private float lastAttackTime;
    private float attackBufferTime = 0.3f;
    private float attackBufferCounter;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Stamina Settings")]
    public Slider staminaSlider; // Referência ao Slider da estamina
    public float maxStamina; // Valor máximo de estamina
    public float staminaDepletionRate; // Taxa de redução da estamina por segundo
    public float staminaRecoveryRate; // Taxa de recuperação da estamina por segundo
    public float recoveryDelay = 1f; // Tempo de espera para começar a recuperar após soltar o botão

    public bool isDefending = false; // Verifica se o jogador está defendendo
    public bool canDefend = true; // Verifica se o jogador pode defender
    private float currentStamina;
    private float recoveryTimer;

    void Start()
    {
        isFacingRight = true;
        canMove = false;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        playerLife = GetComponent<PlayerLife>();
        pauseGame = GameObject.FindAnyObjectByType<PauseScript>();
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }

    void Update()
    {
        if(playerLife.isDeath || !canMove || PauseScript.paused) return;

        PlayerInput();
        FlipSprite();
        IsGrounded();   
        AnimationState();
        CheckPlayerSteps();
        HandleStamina();
    }

    private void PlayerInput()
    {
        if(!playerLife.takingDamage)
        {
            // Movimento horizontal
            horizontalInput = Input.GetAxis("Horizontal");
            
            // Pular
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

            if(coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !attacking && !blocking)
            {
                Jump();
                jumpBufferCounter = 0f;
            }
            
            if(Input.GetButtonUp("Jump") && rb2d.velocity.y > 0f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }

            //Atacar
            if(Input.GetButtonDown("Fire1"))
            {
                attackBufferCounter = attackBufferTime;
            }
            else
            {
                attackBufferCounter -= Time.deltaTime;
            }

            if(attackBufferCounter > 0f && !attacking)
            {
                Attack(); 
                attackBufferCounter = 0;
                horizontalInput = 0;
            }

            // Bloquear
            if (Input.GetButtonDown("Fire2") && IsGrounded() && !blocking && canDefend)
            {
                Block();
                isDefending = true;
                horizontalInput = 0;
            }

            if (Input.GetButtonUp("Fire2") && blocking)
            {
                DisableBlock();
                isDefending = false;
            }

            // Pausar
            if (Input.GetButtonDown("Cancel"))
            {
                pauseGame.SetPauseMenu(!PauseScript.paused);
            }
        }
    }

    private void FixedUpdate()
    {
        if(playerLife.takingDamage || playerLife.isDeath) return;

        
        if (attacking && IsGrounded() || blocking && IsGrounded())
        {
            rb2d.velocity = Vector2.zero;
        }

        else
        {
            // Movimentação normal
            rb2d.velocity = new Vector2(horizontalInput * moveSpeed, rb2d.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D ground = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, 0, Vector2.down, 0.1f, whatIsGround);
        if (ground.collider != null && !isGrounded)
        {
            anim.ResetTrigger("AirAttack");
            attacking = false;
        }
        return isGrounded = ground.collider != null;
    }

    private void Jump()
    {
        CreateDust();
        SoundManager.Instance.PlaySound2D("Jumping");
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown || attacking) return;

        attacking = true;
        lastAttackTime = Time.time;

        if (IsGrounded())
        {
            // Ataque no chão
            anim.SetTrigger("Attack");
        }
        else
        {
            // Ataque no ar
            anim.SetTrigger("AirAttack");
        }

        StartCoroutine(AttackSlashSound());
        StartCoroutine(DisableAttack());
    }

    private IEnumerator AttackSlashSound()
    {
        yield return new WaitForSeconds(0.30f);
        if (!playerLife.takingDamage)
        {
            SoundManager.Instance.PlaySound2D("SwordSlash");
        }
    }

    public IEnumerator DisableAttack()
    {
        yield return new WaitForSeconds(0.5f);
        attacking = false;
    }

    private void Block()
    {
        blocking = true;
        SoundManager.Instance.PlaySound2D("PullShield");
    }

    public void DisableBlock()
    {
        StartCoroutine(DisableBlockDelay());
    }

    private IEnumerator DisableBlockDelay()
    {
        yield return new WaitForSeconds(0.1f);
        blocking = false;
        isDefending = false;
    }

    private void HandleStamina()
    {
        if (isDefending && canDefend)
        {
            // Reduz a estamina enquanto o jogador estiver defendendo
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaSlider.value = currentStamina;

            // Impede que o jogador continue defendendo se a estamina zerar
            if (currentStamina <= 0)
            {
                canDefend = false;
                isDefending = false;
                DisableBlock();
            }

            recoveryTimer = 0; // Reseta o tempo de recuperação
        }
        else
        {
            // Inicia a recuperação após o tempo de espera
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                staminaSlider.value = currentStamina;

                // Permite defender novamente quando a estamina estiver cheia
                if (currentStamina >= maxStamina)
                {
                    canDefend = true;
                }
            }
        }
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

    private void AnimationState()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isJumping", !isGrounded);
        anim.SetBool("isBlocking", blocking);
    }

    public void Rebound()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, reboundVelocity);
    }

    public void EnemyRebound()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
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