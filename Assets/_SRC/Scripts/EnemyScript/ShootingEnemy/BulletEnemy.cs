using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float reboundPower;
    [SerializeField] private int damage;
    public float lifeTime;

    private void Update()
    {
        ShootTranslate();
        BulletLifeTime();
    }

    private void ShootTranslate()
    {
        transform.Translate(Time.deltaTime * velocity * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out PlayerLife playerLife))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLife.TakeDamage(directionDamage, reboundPower, 1);
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
