using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyController : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private int health = 3;

    [Header("Enemy Info")]
    [SerializeField] private float speed;
    [SerializeField] private float reboundPower;
    [SerializeField] private float detectionRadius;
    private Transform playerTransform;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private Rigidbody2D rb2d;
    private Vector2 movement;
    private Animator anim;
    private bool chasingPlayer, takingDamage, playerIsAlive, isDead;

    void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        AnimationState();
        EnemyIsDeath();     
    }

    private void FixedUpdate()
    {
        Chasing();
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
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if(distanceToPlayer < detectionRadius)
            {
                chasingPlayer = true;

                Vector2 direction = (playerTransform.position - transform.position).normalized;

                FlipSprite(direction);

                movement = new Vector2(direction.x, -1);
            }
            else
            {
                chasingPlayer = false;

                movement = Vector2.down;
            }

            if(!takingDamage)
            {
                rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !takingDamage && !isDead)
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
        if(collision.CompareTag("Sword") && !isDead)
        {
            Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
            TakeDamage(directionDamage, reboundPower, 1);
        }
    
        if(collision.CompareTag("Shield") && !isDead)
        {
            Vector2 direction = new Vector2(collision.gameObject.transform.position.x, 0);
            TakeDamage(direction, reboundPower, 0);
            playerMovementScript.DisableBlock();
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
        yield return new WaitForSeconds(0.5f);
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
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>(), gameObject.GetComponent<BoxCollider2D>(), true);
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
