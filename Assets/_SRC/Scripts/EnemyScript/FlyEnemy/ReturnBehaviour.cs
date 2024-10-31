using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;
    private Vector3 initialPoint;
    private FlyEnemyController flyEnemyController;
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
        Vector2 direction = (initialPoint - animator.transform.position).normalized;
        rb.velocity = direction * speedMovement;

        flyEnemyController.FlipSprite(initialPoint);

        if(Vector2.Distance(animator.transform.position, initialPoint) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Arrived");
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
