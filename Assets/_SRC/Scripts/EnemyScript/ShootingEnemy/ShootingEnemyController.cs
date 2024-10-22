using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Transform controllShoot;
    [SerializeField] private float shootDistance;
    [SerializeField] private float reboundPower;
    public LayerMask playerLayer;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private Transform playerTransform;
    private bool playerInRange, playerIsAlive, takingDamage, isDead;
    public float timeBetweenShoots;
    public float timeLastShoot;
    public GameObject enemyBullet;
    public float waitTimeToShoot;
    private Animator anim;
    private Rigidbody2D rb2d;

    private void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }
    private void Update()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        FlipSprite(direction);
        PlayerInRange();
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
    
    private void PlayerInRange()
    {
        playerInRange = Physics2D.Raycast(controllShoot.position, transform.right, shootDistance, playerLayer);

        if(IsPlayerAlive() && playerInRange && !takingDamage && !isDead)
        {
            if(Time.time > timeBetweenShoots + timeLastShoot)
            {
                timeLastShoot = Time.time;
                anim.SetTrigger("Shoot");
                Invoke(nameof(Shoot), waitTimeToShoot);
            }
        }
    }

    private void Shoot()
    {
        Instantiate(enemyBullet, controllShoot.position, controllShoot.rotation);
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

    private bool IsPlayerAlive()
    {
        return playerIsAlive != playerLifeScript.isDeath;
    }

    public void EnemyIsDeath()
    {
        if(health <= 0)
        {
            isDead = true;
            rb2d.velocity = Vector2.zero;
            Destroy(gameObject);
        }  
        else
        {
            isDead = false;
        }    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controllShoot.position, controllShoot.position + transform.right * shootDistance);
    }
}
