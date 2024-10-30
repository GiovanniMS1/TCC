using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;
    private Transform player;
    private PlayerLife playerLife;
    private EnemyLife enemyLife;
    private FlyEnemyController flyEnemyController;
    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        flyEnemyController = animator.gameObject.GetComponent<FlyEnemyController>();
        enemyLife = animator.gameObject.GetComponent<EnemyLife>();
        rb = animator.gameObject.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.transform.position = Vector2.MoveTowards(animator.transform.position, player.position, speedMovement * Time.deltaTime);
        Vector2 direction = (player.position - animator.transform.position).normalized;
        rb.velocity = direction * speedMovement;

        flyEnemyController.FlipSprite(player.position);

        if(enemyLife.takingDamage || flyEnemyController.CalculateDistance() > 5f)
        {
            animator.SetTrigger("Return");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
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
