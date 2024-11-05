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
            // Pega o ponto atual e define a direção
            Vector3 targetPosition = flyEnemyController.pathPositions[flyEnemyController.pathPositions.Count - 1];
            Vector2 direction = (targetPosition - animator.transform.position).normalized;

            // Move o morcego usando o Rigidbody2D
            rb.velocity = direction * speedMovement;

            // Vira o sprite para a direção do caminho
            flyEnemyController.FlipSprite(targetPosition);

            // Se o morcego estiver perto o suficiente do ponto, remove o ponto
            if (Vector2.Distance(animator.transform.position, targetPosition) < 0.1f)
            {
                flyEnemyController.pathPositions.RemoveAt(flyEnemyController.pathPositions.Count - 1);
            }
        }
        else
        {
            // Mantém o morcego voltando ao ponto inicial sem teleporte
            Vector2 direction = (initialPoint - animator.transform.position).normalized;
            rb.velocity = direction * speedMovement;

            // Quando chega ao ponto inicial, finaliza o estado
            if (Vector2.Distance(animator.transform.position, initialPoint) < 0.1f)
            {
                animator.transform.position = initialPoint;
                rb.velocity = Vector2.zero;
                animator.SetTrigger("Arrived");
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flyEnemyController.ClearPath();
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
