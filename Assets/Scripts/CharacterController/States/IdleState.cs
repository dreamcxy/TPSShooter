using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleState : EnemyAIState
{
    private EnemyAI enemyAI;

    #region public methods
    public IdleState(EnemyAI new_enemyAI)
    {
        enemyAI = new_enemyAI;
    }

    public override void AIBehavior(){
        
    }
    #endregion
}