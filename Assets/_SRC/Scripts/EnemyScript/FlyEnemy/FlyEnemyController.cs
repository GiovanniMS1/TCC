using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyController : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private float reboundPower;
    private Transform playerTransform;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private float distanceBetweenPlayer;
    private Vector3 initialPoint;
    private Animator anim;
    private Rigidbody2D rb2d;
    private EnemyLife enemyLife;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        initialPoint = transform.position;
    }

    private void Update()
    {
        CalculateDistance();
        AnimationState();
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
        if(collision.collider.CompareTag("Player") && !enemyLife.takingDamage && !enemyLife.isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
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
            anim.SetTrigger("Return");
            playerMovementScript.DisableBlock();
        }
    }
    
    private void AnimationState()
    {
        anim.SetFloat("Distance", distanceBetweenPlayer);
        anim.SetBool("Hit", enemyLife.takingDamage);
    }
}
