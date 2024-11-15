using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Enemy Life Status")]
    [SerializeField] private int life;
    [SerializeField] private GameObject blood;
    private Rigidbody2D rb2d;
    public bool takingDamage, isDead, isRebounding;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        EnemyIsDeath();
    }

    public void TakeDamage(int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            life -= damage;
            if(life >= 1) SoundManager.Instance.PlaySound2D("EnemyHit");
            rb2d.velocity = Vector2.zero;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            StartCoroutine(DisableDamage());
        }
    }

    public void Rebound(Vector2 direction, float reboundPower)
    {
        if(!isRebounding)
        {
            isRebounding = true;
            Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
            rb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
            StartCoroutine(DisableRebound());
        }
    }

    IEnumerator DisableRebound()
    {
        yield return new WaitForSeconds(0.5f);
        isRebounding = false;
        rb2d.velocity = Vector2.zero;
    }
    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.5f);
        takingDamage = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        if(other.GetContact(0).normal.y <= -0.9f)
        {
            TakeDamage(1);
            other.gameObject.GetComponent<PlayerBehaviour>().Rebound();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("DeathZone"))
        {
            life = 0;
            takingDamage = true;
            isDead = true;
            DestroyEnemy();
            SoundManager.Instance.PlaySound2D("Explosion");
        }
    }
    private void EnemyIsDeath()
    {
        if(life <= 0 && !isDead)
        {
            isDead = true;
            rb2d.velocity = Vector2.zero;
            DestroyEnemy();
        }
    }

    private void OnDestroy()
    {
        if (!isDead) return;

        Instantiate(blood, transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySound2D("Explosion");
    }
    private void DestroyEnemy()
    {
        if(isDead)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            Destroy(gameObject);
        }
    }
}