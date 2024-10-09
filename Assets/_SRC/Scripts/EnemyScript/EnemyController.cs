using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private int health = 3;

    [Header("Enemy Info")]
    [SerializeField] private float reboundPower;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float speed;
    private Transform player;
    private PlayerBehaviour playerScript;
    private Rigidbody2D rb2d;
    private Vector2 movement;
    private Animator anim;
    private bool chasingPlayer, takingDamage, playerIsAlive, isDead;

    void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        Chasing();
        AnimationState();
        EnemyIsDeath();     
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
        if(playerIsAlive && !isDead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if(distanceToPlayer < detectionRadius)
            {
                chasingPlayer = true;

                Vector2 direction = (player.position - transform.position).normalized;

                FlipSprite(direction);

                movement = new Vector2(direction.x, rb2d.velocity.y);
            }
            else
            {
                chasingPlayer = false;

                movement = Vector2.zero;
            }

            if(!takingDamage)
                rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerScript.TakeDamage(directionDamage, reboundPower, 1);
            playerIsAlive = !playerScript.PlayerIsDeath();
            if(!playerIsAlive)
            {
                chasingPlayer = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Sword"))
        {
            Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
            TakeDamage(directionDamage, reboundPower, 1);
        }
    
        if(collision.CompareTag("Shield"))
        {
            Vector2 direction = new Vector2(collision.gameObject.transform.position.x, 0);
            TakeDamage(direction, reboundPower, 0);
            playerScript.DisableBlock();
        }
    }

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            health -= damage;
            Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
            rb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
            StartCoroutine(DisableDamage());
        }
    }

    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.4f);
        takingDamage = false;
        rb2d.velocity = Vector2.zero;
    }

    private void AnimationState()
    {
        anim.SetBool("chasing", chasingPlayer);
        anim.SetBool("isDead", isDead);
    }

    public void EnemyIsDeath()
    {
        if(health <= 0)
        {
            isDead = true;
            rb2d.velocity = Vector2.zero;
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), gameObject.GetComponent<BoxCollider2D>(), true);
        }  
        else
        {
            isDead = false;
        }    
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
