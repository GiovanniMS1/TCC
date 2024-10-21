using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyController : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private int health = 3;
    [SerializeField] private float reboundPower;
    private Transform playerTransform;
    private Rigidbody2D rb2d;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private float distanceBetweenPlayer;
    private Vector3 initialPoint;
    private Animator anim;
    private bool takingDamage, isDead;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initialPoint = transform.position;
    }

    private void Update()
    {
        CalculateDistance();
        AnimationState();
        EnemyIsDeath();
    }

    public float CalculateDistance()
    {
        return distanceBetweenPlayer = Vector2.Distance(transform.position, playerTransform.position);
    }

    public Vector3 GetInitialPoint()
    {
        return initialPoint;
    }

    public void FlipSprite(Vector3 target)
    {
        if(transform.position.x < target.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !takingDamage && !isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
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
            anim.SetTrigger("Return");
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
        yield return new WaitForSeconds(0.1f);
        takingDamage = false;
    }

    public bool GetTakeDamage()
    {
        return takingDamage;
    }
    private void AnimationState()
    {
        anim.SetFloat("Distance", distanceBetweenPlayer);
        anim.SetBool("Hit", takingDamage);
    }

    public void EnemyIsDeath()
    {
        if(health <= 0)
        {
            isDead = true;
        }  
        else
        {
            isDead = false;
        }    
    }

    private void DestroyEnemy()
    {
        if(isDead)
            Destroy(gameObject);
    }
}
