using System.Collections;
using UnityEngine;

public class FollowPlayerArea : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private float searchRadius;
    [SerializeField] private float maxDistance;
    [SerializeField] private float movemetVelocity;
    [SerializeField] private float reboundPower;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MovimentState actualState;
    public enum MovimentState
    {
        Waiting,
        Following,
        Returning,
        TakingDamage,
    }
    private Vector3 initialPoint;
    private Rigidbody2D rb2d;
    private Animator anim;
    private EnemyLife enemyLife;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    

    private void Start()
    {
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        enemyLife = GetComponent<EnemyLife>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initialPoint = transform.position;
    }

    private void Update()
    {
        switch (actualState)
        {
            case MovimentState.Waiting:
                WaitingState();
                break;
            case MovimentState.Following:
                FollowingState();
                break;
            case MovimentState.Returning:
                ReturningState();
                break;
            case MovimentState.TakingDamage:
                StartCoroutine(TakingDamage());
                break;
        }

        AnimationState();
    }

    private void WaitingState()
    {
        if(playerLifeScript.PlayerIsDeath()) return;

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, searchRadius, whatIsPlayer);

        if(playerCollider)
        {
            playerTransform = playerCollider.transform;

            actualState = MovimentState.Following;
        }
    }

    private void FollowingState()
    {
        if (enemyLife.takingDamage)
        {
            actualState = MovimentState.TakingDamage;
            playerTransform = null;
            return;
        }

        if(playerTransform == null)
        {
            actualState = MovimentState.Returning;
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb2d.velocity = direction * movemetVelocity;

        FlipSprite(playerTransform.position);        

        if(Vector2.Distance(transform.position, initialPoint) > maxDistance ||
        Vector2.Distance(transform.position, playerTransform.position) > maxDistance ||
        playerLifeScript.PlayerIsDeath())
        {
            actualState = MovimentState.Returning;
            playerTransform = null;
        }
    }

    private void ReturningState()
    {
        Vector2 direction = (initialPoint - transform.position).normalized;
        rb2d.velocity = direction * movemetVelocity;

        FlipSprite(initialPoint);

        if(Vector2.Distance(transform.position, initialPoint) < 0.1f)
        {
            transform.position = initialPoint;
            anim.SetBool("Desappear",false);
            rb2d.velocity = Vector2.zero;
            actualState = MovimentState.Waiting;
        }
    }

    private IEnumerator TakingDamage()
    {
        anim.SetBool("Desappear",true);
        yield return new WaitForSeconds(1);
        actualState = MovimentState.Returning;
    }

    private void FlipSprite(Vector3 target)
    {
        if(transform.position.x < target.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if(transform.position.x >= target.x)
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
            actualState = MovimentState.Returning;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Sword") && !enemyLife.isDead)
        {
            enemyLife.TakeDamage(1);
            actualState = MovimentState.TakingDamage;
        }
    
        if(collision.gameObject.CompareTag("Shield") && !enemyLife.isDead)
        {
            SoundManager.Instance.PlaySound2D("Blocked");
            actualState = MovimentState.Returning;
            playerMovementScript.DisableBlock();
        }
    }

    private void AnimationState()
    {
        anim.SetBool("Hit", enemyLife.takingDamage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(initialPoint, maxDistance);
    }
}