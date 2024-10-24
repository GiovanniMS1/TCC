using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyController : MonoBehaviour
{
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
    private EnemyLife enemyLife;
    private bool chasingPlayer, playerIsAlive;

    void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        AnimationState();
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
        if(playerIsAlive && !enemyLife.isDead)
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

            if(!enemyLife.takingDamage)
            {
                rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
            }
        }
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
            Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
            enemyLife.TakeDamage(directionDamage, reboundPower, 1);
        }
    
        if(collision.CompareTag("Shield") && !enemyLife.isDead)
        {
            Vector2 direction = new Vector2(collision.gameObject.transform.position.x, 0);
            enemyLife.TakeDamage(direction, reboundPower, 0);
            playerMovementScript.DisableBlock();
        }
    }

    private void AnimationState()
    {
        anim.SetBool("Chasing", chasingPlayer);
        anim.SetBool("isDead", enemyLife.isDead);
        anim.SetBool("Hit", enemyLife.takingDamage);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
