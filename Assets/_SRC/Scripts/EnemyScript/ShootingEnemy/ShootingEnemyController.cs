using System.Collections;
using UnityEngine;

public class ShootingEnemyController : MonoBehaviour
{
    [SerializeField] private Transform controllShoot;
    [SerializeField] private float shootDistance;
    [SerializeField] private float reboundPower;
    public LayerMask playerLayer;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private Transform playerTransform;
    private bool playerInRange, playerIsAlive, isShooting;
    public float timeBetweenShoots;
    public float timeLastShoot;
    public GameObject enemyBullet;
    public float waitTimeToShoot;
    private Animator anim;
    private Rigidbody2D rb2d;
    private EnemyLife enemyLife;
    private Vector3 direction;
    private Vector3 lockedDirection;

    private void Start()
    {
        playerIsAlive = true;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }
    private void Update()
    {
        if(!enemyLife.takingDamage && !isShooting && playerIsAlive)
        {
            PlayerInRange();
            FlipSprite(Direction());
        }

        AnimationState();
    }

    private Vector2 Direction()
    {
        return direction = new Vector3(playerTransform.position.x - transform.position.x, 0, 0).normalized;
    }
    private void FlipSprite(Vector2 direction)
    {
        if(direction.x < 0)
            transform.eulerAngles = new Vector3(0,0,0);
        if(direction.x > 0)
            transform.eulerAngles = new Vector3(0,180,0);
    }
    
    private void PlayerInRange()
    {
        playerInRange = Physics2D.Raycast(controllShoot.position, direction, shootDistance, playerLayer);

        if(IsPlayerAlive() && playerInRange && !enemyLife.takingDamage && !enemyLife.isDead)
        {
            if(Time.time > timeBetweenShoots + timeLastShoot)
            {
                SoundManager.Instance.PlaySound2D("ShootingEnemy");
                timeLastShoot = Time.time;
                lockedDirection = Direction();
                isShooting = true;
                anim.SetTrigger("Shoot");
                Invoke(nameof(Shoot), waitTimeToShoot);
            }
        }
    }

    private void Shoot()
    {
        if(!enemyLife.takingDamage && isShooting)
        {
            GameObject bullet = Instantiate(enemyBullet, controllShoot.position, controllShoot.rotation);
            BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
            bulletScript.SetInitialDirection(lockedDirection);
            StartCoroutine(DisableIsShooting());
        }
    }
    
    IEnumerator DisableIsShooting()
    {
        yield return new WaitForSeconds(0.5f);
        isShooting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Sword") && !enemyLife.isDead)
        {
            enemyLife.TakeDamage(1);
            if (isShooting)
            {
                CancelShoot();
            }
        }
    }

    private void CancelShoot()
    {
        isShooting = false;
        anim.ResetTrigger("Shoot");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !enemyLife.takingDamage && !enemyLife.isDead)
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            playerLifeScript.TakeDamage(directionDamage, reboundPower, 1);
        }
    }

    private bool IsPlayerAlive()
    {
        return playerIsAlive != playerLifeScript.isDeath;
    }

    private void AnimationState()
    {
        anim.SetBool("Hit", enemyLife.takingDamage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 debugDirection = (playerTransform != null) ? new Vector3(playerTransform.position.x - transform.position.x, 0, 0).normalized : Vector3.left;
        Gizmos.DrawLine(controllShoot.position, controllShoot.position + debugDirection * shootDistance);
    }
}
