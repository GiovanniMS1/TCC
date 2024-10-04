using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;
    [Header("Enemy Info")]
    public float reboundPower = 10f;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    private Rigidbody2D rb2d;
    private Vector2 movement;
    private Animator anim;
    private bool chasingPlayer;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Chasing();
        AnimationState();
    }

    private void FlipSprite(Vector2 direction)
    {
        if(direction.x < 0)
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
        if(direction.x > 0)
            transform.localScale = new Vector3(-2, 2, 2);
    }

    private void Chasing()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer < detectionRadius)
        {
            chasingPlayer = true;

            Vector2 direction = (player.position - transform.position).normalized;

            FlipSprite(direction);

            movement = new Vector2(direction.x, 0);
        }
        else
        {
            chasingPlayer = false;

            movement = Vector2.zero;
        }

        rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
    }
    private void AnimationState()
    {
        anim.SetBool("chasing", chasingPlayer);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);

            collision.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(directionDamage, reboundPower, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
