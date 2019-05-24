using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : EnemyAIState
{
    [HideInInspector]
    public EnemyAI enemyAI;
    private EnemyMovement enemyMovement;
    private Animator animator;

    private Timer timer;

    private bool attacked;


    public ChasingState(EnemyAI new_enemyAI)
    {
        enemyAI = new_enemyAI;
        enemyMovement = enemyAI.GetComponent<EnemyMovement>();
        animator = enemyAI.GetComponent<Animator>();
        timer = GameObject.FindGameObjectWithTag("GameController").GetComponent<Timer>();
        attacked = false;
    }

    public override void AIBehavior()
    {
        // enemyAI.forward = 0;
        if (enemyAI.target == null)
        {
            enemyAI.SetEnemyState(enemyAI.guardState);
            return;
        }
        Debug.LogFormat("enemy target:{0}", enemyAI.target);
        if (!enemyAI.navMeshAgent.isOnNavMesh || enemyAI.guardSettings.wayPoints.Length == 0) return;
        enemyAI.navMeshAgent.SetDestination(enemyAI.target.position);
        enemyAI.LookAtPosition(enemyAI.navMeshAgent.steeringTarget);


        if (enemyAI.navMeshAgent.remainingDistance <= enemyAI.attackSettings.attackRange - 1)
        {
            Debug.Log("prepare attack...");
            enemyAI.walkingToDest = false;
            enemyAI.forward = 0;
            Attacking();
        }
        else
        {
            enemyAI.walkingToDest = true;
            enemyAI.forward = enemyAI.LerpSpeed(enemyAI.forward, 1f, 15);
            // enemyAI.forward = 1;
            animator.SetBool("Attack", false);
        }
        enemyMovement.JustAnimate(enemyAI.forward, 0);
    }

    public void Attacking()
    {
        Debug.LogFormat("attacked:{0}", attacked);
        if (attacked == false && enemyAI.target != null && enemyAI.attackSettings.attackLatTime -  enemyAI.attackSettings.attackForTime > enemyAI.attackSettings.attackCoolDown)
        {
            // Debug.LogFormat("animator:{0}",animator);
            Debug.Log("attack....");
            Debug.Log(animator.name);
            
            attacked = true;
            animator.SetBool("Attack", attacked);
            attacked = false;
            // animator.SetBool("Attack", attacked);
            enemyAI.attackSettings.attackForTime = enemyAI.attackSettings.attackLatTime;
            // StartCoroutine(StopAttack());
            // timer.Add(() => { attacked = false; },
            // enemyAI.attackSettings.attackCoolDown);
        }
    }

}