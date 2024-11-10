using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyController : MonoBehaviour
{
    [Header("Enemy Status")]
    [SerializeField] private float reboundPower;
    [SerializeField] private LayerMask obstacleLayer;
    private Transform playerTransform;
    private PlayerLife playerLifeScript;
    private PlayerBehaviour playerMovementScript;
    private float distanceBetweenPlayer;
    private Vector3 initialPoint;
    private Animator anim;
    private EnemyLife enemyLife;
    private Rigidbody2D rb2d;
    public List<Vector3> pathPositions = new List<Vector3>();
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLifeScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        anim = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        initialPoint = transform.position;
    }

    private void Update()
    {
        if(!playerLifeScript.PlayerIsDeath() && HasLineOfSight())
        {
            CalculateDistance();
        }
        
        AnimationState();
    }

    // Função para armazenar a posição atual no caminho
    public void StorePathPosition()
    {
        // Verifica a última posição registrada para evitar pontos muito próximos
        if (pathPositions.Count == 0 || Vector3.Distance(transform.position, pathPositions[pathPositions.Count - 1]) > 0.5f)
        {
            pathPositions.Add(transform.position);
        }
    }

    public void ClearPath()
    {
        // Limpa o caminho quando o morcego retorna ao ponto inicial
        pathPositions.Clear();
    }

    public float CalculateDistance()
    {
        return distanceBetweenPlayer = Vector2.Distance(transform.position, playerTransform.position);
    }

    private bool HasLineOfSight()
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Verifica se há algum obstáculo entre o morcego e o jogador
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        return hit.collider == null; // Se `null`, a linha de visão está limpa
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
        if(collision.gameObject.CompareTag("Sword") && !enemyLife.isDead)
        {
            enemyLife.TakeDamage(1);
        }
    
        if(collision.gameObject.CompareTag("Shield") && !enemyLife.isDead)
        {
            SoundManager.Instance.PlaySound2D("Blocked");
            anim.SetTrigger("Return");
            playerMovementScript.DisableBlock();
        }
    }
    
    private void AnimationState()
    {
        anim.SetFloat("Distance", distanceBetweenPlayer);
        anim.SetBool("Hit", enemyLife.takingDamage);
        anim.SetBool("PlayerIsAlive", !playerLifeScript.PlayerIsDeath());
    }
}
