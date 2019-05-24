using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardState:EnemyAIState
{
    private EnemyAI enemyAI;
    private EnemyMovement enemyMovement;

    private int wayPointIndex;  // 巡逻地点编号
    private float currentWaitTime;  //巡逻点等待时间

    public GuardState(EnemyAI new_enemyAI){
        enemyAI = new_enemyAI;
        enemyMovement = new_enemyAI.GetComponent<EnemyMovement>();
        wayPointIndex = 0;
        currentWaitTime = 5f;
    }

    // AI 巡逻
    public override void AIBehavior(){
        if (!enemyAI.navMeshAgent.isOnNavMesh || enemyAI.guardSettings.wayPoints.Length == 0)   return;
        
        enemyAI.navMeshAgent.SetDestination(enemyAI.guardSettings.wayPoints[wayPointIndex].destination.position);
        enemyAI.LookAtPosition(enemyAI.navMeshAgent.steeringTarget);
        
        // 走到一个点，走向下一个点
        if (enemyAI.navMeshAgent.remainingDistance <= enemyAI.navMeshAgent.stoppingDistance)
        {
            enemyAI.walkingToDest =false;
            enemyAI.forward = enemyAI.LerpSpeed(enemyAI.forward, 0, 15);    //停下
            // enemyAI.forward = 0;

            currentWaitTime -= Time.deltaTime;
            if (enemyAI.guardSettings.wayPoints[wayPointIndex].lookAtTarget != null)
            {
                enemyAI.SetLookAtTransform(enemyAI.guardSettings.wayPoints[wayPointIndex].lookAtTarget);
            }
            if (currentWaitTime <= 0)
            {
                wayPointIndex = (wayPointIndex + 1)% enemyAI.guardSettings.wayPoints.Length;
                currentWaitTime = enemyAI.guardSettings.wayPoints[wayPointIndex].waitTime;
            }
        }else
        {
            // 走向目标点
            enemyAI.walkingToDest = true;
            enemyAI.forward = enemyAI.LerpSpeed(enemyAI.forward, 0.5f, 15);
            // enemyAI.forward = 0.5f;
            currentWaitTime = enemyAI.guardSettings.wayPoints[wayPointIndex].waitTime;
        }
        enemyMovement.AnimateAndMove(enemyAI.forward, 0);
    }



}