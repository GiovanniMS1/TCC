using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float reboundPower;
    [SerializeField] private int damage;
    private PlayerLife playerLife;
    public float lifeTime;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
    }
    private void Update()
    {
        ShootTranslate();
        BulletLifeTime();
    }

    private void ShootTranslate()
    {
        transform.Translate(Time.deltaTime * velocity * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLife.TakeDamage(directionDamage, reboundPower, 1);
            DestroyBullet();
        }
        else if(collision.TryGetComponent(out EnemyLife enemyLife))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            enemyLife.TakeDamage(directionDamage, reboundPower, 1);
            DestroyBullet();
        }
        else
        {
            DestroyBullet();
        }
    }

    private void BulletLifeTime()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
            DestroyBullet();
    }
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
