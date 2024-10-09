using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("Player Life")]
    [SerializeField] private int health;
    private Rigidbody2D playerRb2d;
    private PlayerBehaviour playerScript;
    private Animator anim;
    public bool takingDamage, isDeath;

    private void Start()
    {
        playerScript = GetComponent<PlayerBehaviour>();
        playerRb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimationState();
    }
    public bool PlayerIsDeath()
    {
        if(health <= 0)
            return isDeath = true;
        else
            return isDeath = false;
    }

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            health -= damage;
            if(!PlayerIsDeath())
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
                playerRb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
                playerScript.DisableAttack();
                playerScript.DisableBlock();
            }
            else
            {
                playerRb2d.velocity = Vector2.zero;
            }
        }
    }

    public void DisableDamage()
    { 
        takingDamage = false;
    }

    private void AnimationState()
    {  
        anim.SetBool("takeDamage", takingDamage);
        anim.SetBool("isDying", isDeath);
    }
}
