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

    public override void AIBehavior(){
        
    }



}