using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Enemy Life Status")]
    [SerializeField] private int life;
    private Rigidbody2D rb2d;
    public bool takingDamage, isDead;

    private void Start()
    {

        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        EnemyIsDeath();
    }

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            life -= damage;
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

    private void EnemyIsDeath()
    {
        if(life <= 0)
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

    private void DestroyEnemy()
    {
        if(isDead)
            Destroy(gameObject);
    }
}
