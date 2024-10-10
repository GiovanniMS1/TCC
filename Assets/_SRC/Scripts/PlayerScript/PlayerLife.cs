using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [Header("Player Life")]
    [SerializeField] private int actualLife;
    [SerializeField] private int maxLife;
    public UnityEvent<int> changeLife;
    private Rigidbody2D playerRb2d;
    private PlayerBehaviour playerScript;
    private Animator anim;
    public bool takingDamage, isDeath;

    private void Start()
    {
        actualLife = maxLife;
        changeLife.Invoke(actualLife);
        playerScript = GetComponent<PlayerBehaviour>();
        playerRb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimationState();
    }

    private bool PlayerIsDeath()
    {
        if(actualLife <= 0)
        {
            Physics2D.IgnoreLayerCollision(6, 7, true);
            return isDeath = true;
        }
        else
        {
            Physics2D.IgnoreLayerCollision(6, 7, false);
            return isDeath = false;
        }
    }

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;

            actualLife -= damage;

            changeLife.Invoke(actualLife);

            if(!PlayerIsDeath())
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
                playerRb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
                playerScript.DisableAttack();
                playerScript.DisableBlock();
                StartCoroutine(DisableDamage());
            }
            else
            {
                playerRb2d.velocity = Vector2.zero;
            }
        }
    }

    private IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(1);
        takingDamage = false;
    }

    public void HealLife(int heal)
    {
        int tempLife = actualLife + heal;

        if(actualLife > maxLife)
        {
            actualLife = maxLife;
        }
        else
        {
            actualLife = tempLife;
        }

        changeLife.Invoke(actualLife);
    }

    private void AnimationState()
    {  
        anim.SetBool("takeDamage", takingDamage);
        anim.SetBool("isDying", isDeath);
    }
}
