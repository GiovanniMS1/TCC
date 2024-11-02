using UnityEngine;

public class ReturnBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;
    private FlyEnemyController flyEnemyController;
    private Vector3 initialPoint;
    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flyEnemyController = animator.gameObject.GetComponent<FlyEnemyController>();
        initialPoint = flyEnemyController.GetInitialPoint();
        rb = animator.gameObject.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (flyEnemyController.pathPositions.Count > 0)
        {
            // Determina o ponto alvo e a direção
            Vector3 targetPosition = flyEnemyController.pathPositions[flyEnemyController.pathPositions.Count - 1];
            Vector2 direction = (targetPosition - animator.transform.position).normalized;

            // Vira o sprite para a direção do caminho
            flyEnemyController.FlipSprite(targetPosition);

            // Move o morcego usando o Rigidbody2D
            rb.velocity = direction * speedMovement;

            // Checa se o morcego chegou perto do ponto
            if (Vector2.Distance(animator.transform.position, targetPosition) < 0.1f)
            {
                flyEnemyController.pathPositions.RemoveAt(flyEnemyController.pathPositions.Count - 1);
            }
        }
        else
        {
            // Para o morcego ao chegar ao ponto inicial
            animator.transform.position = initialPoint;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Arrived");
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flyEnemyController.ClearPath();
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
