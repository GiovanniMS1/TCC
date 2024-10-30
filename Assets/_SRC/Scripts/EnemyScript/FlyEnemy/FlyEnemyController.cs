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
    private EnemyLife enemyLife;
    private Rigidbody2D rb;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        rb = GetComponent<Rigidbody2D>();
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !enemyLife.takingDamage && !enemyLife.isDead && !playerLifeScript.takingDamage)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
            anim.SetTrigger("Return");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Sword") && !enemyLife.isDead)
        {
            enemyLife.TakeDamage(1);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(distanceBetweenPlayer <= 5f && playerTransform != null)
        {
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}
