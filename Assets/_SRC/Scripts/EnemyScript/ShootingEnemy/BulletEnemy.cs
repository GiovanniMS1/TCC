using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float reboundPower;
    [SerializeField] private int damage;
    private Vector3 directionBullet;
    private PlayerBehaviour playerMovement;
    public float lifeTime;
    private Rigidbody2D rb2d;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        BulletLifeTime();
    }

    private void FixedUpdate()
    {
        rb2d.velocity = directionBullet * velocity;
    }

    public void SetInitialDirection(Vector3 initialDirection)
    {
        directionBullet = initialDirection.x < 0 ? Vector3.left : Vector3.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Shield"))
        {
            SoundManager.Instance.PlaySound2D("Blocked");
            Vector3 shieldNormal = collision.transform.right;
            directionBullet = Vector2.Reflect(directionBullet, shieldNormal);
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            playerMovement.DisableBlock();
        }

        else if(collision.TryGetComponent(out PlayerLife playerLife))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLife.TakeDamage(directionDamage, reboundPower, damage);
            DestroyBullet();
        }
        
        else if(collision.TryGetComponent(out EnemyLife enemyLife))
        {
            enemyLife.TakeDamage(damage);
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