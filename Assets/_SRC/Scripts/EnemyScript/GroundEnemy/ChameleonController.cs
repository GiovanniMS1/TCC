using System.Collections;
using UnityEngine;

public class ChameleonController : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private float speed;
    [SerializeField] private float reboundPower;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    private Transform playerTransform;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private Rigidbody2D rb2d;
    private Vector2 movement;
    private Vector2 attackLockedDirection;
    private Animator anim;
    private EnemyLife enemyLife;
    public bool chasingPlayer, playerIsAlive, isAttacking;
    private float lastAttackTime;

    void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        AnimationState();
    }

    private void FixedUpdate()
    {
        if(playerIsAlive && !enemyLife.isDead)
        {
            Chasing();
        }
    }

    private void FlipSprite(Vector2 direction)
    {
        if(direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Chasing()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRadius)
        {
            chasingPlayer = true;

            // Só atualiza direção e movimento se não estiver atacando
            if (!isAttacking)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                FlipSprite(direction);

                if (distanceToPlayer <= attackRange)
                {
                    if (!enemyLife.takingDamage && Time.time >= lastAttackTime + attackCooldown)
                    {
                        // Travar a direção de ataque e iniciar a corotina
                        attackLockedDirection = direction;
                        StartCoroutine(AttackCoroutine());
                        lastAttackTime = Time.time;
                        movement = Vector2.zero; // Para de mover ao atacar
                    }
                }
                else
                {
                    movement = new Vector2(direction.x, -1);
                }
            }
            else
            {
                // Manter a direção travada enquanto ataca
                movement = Vector2.zero;
            }
        }
        else
        {
            chasingPlayer = false;
            isAttacking = false;
            movement = Vector2.down;
        }

        if (!enemyLife.takingDamage && !enemyLife.isRebounding)
        {
            rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        FlipSprite(attackLockedDirection);
        yield return new WaitForSeconds(0.75f);  // Espera 1 segundo, sincronizado com os 12 frames da animação
        isAttacking = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !enemyLife.takingDamage && !enemyLife.isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
            playerIsAlive = !playerLifeScript.isDeath;
            if(!playerIsAlive)
            {
                chasingPlayer = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Sword") && !enemyLife.isDead)
        {
            enemyLife.TakeDamage(1);
            Vector2 direction = new Vector2(collision.gameObject.transform.position.x, 0);
            enemyLife.Rebound(direction, 3);
        }
    
        if(collision.CompareTag("Shield") && !enemyLife.isDead)
        {
            playerMovementScript.DisableBlock();
        }

        // Se eu quiser que a lingua também dê dano só tirar o comentário!
        if(collision.CompareTag("Player") && !enemyLife.takingDamage && !enemyLife.isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
            playerIsAlive = !playerLifeScript.isDeath;
            if(!playerIsAlive)
            {
                chasingPlayer = false;
            }
        }
    }

    private void AnimationState()
    {
        anim.SetBool("Chasing", chasingPlayer);
        anim.SetBool("Attacking", isAttacking);
        anim.SetBool("Hit", enemyLife.takingDamage);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
