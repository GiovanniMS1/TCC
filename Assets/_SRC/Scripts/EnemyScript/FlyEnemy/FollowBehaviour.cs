using UnityEngine;

public class FollowBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;
    private Transform player;
    private EnemyLife enemyLife;
    private FlyEnemyController flyEnemyController;
    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        flyEnemyController = animator.gameObject.GetComponent<FlyEnemyController>();
        enemyLife = animator.gameObject.GetComponent<EnemyLife>();
        rb = animator.gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.transform.position = Vector2.MoveTowards(animator.transform.position, player.position, speedMovement * Time.deltaTime);
        if (enemyLife.takingDamage || flyEnemyController.CalculateDistance() >= 5f)
        {
            animator.SetTrigger("Return");
            return;
        }

        // Direção para o jogador
        Vector2 direction = (player.position - animator.transform.position).normalized;

        // Aplica a velocidade no Rigidbody2D
        rb.velocity = direction * speedMovement;

        // Registra o caminho para poder retornar mais tarde
        flyEnemyController.StorePathPosition();

        // Vira o sprite para a direção do jogador
        flyEnemyController.FlipSprite(player.position);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Para o movimento ao sair do estado
        rb.velocity = Vector2.zero;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
