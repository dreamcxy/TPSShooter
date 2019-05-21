using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStates:MonoBehaviour{
    [Range(0, 100f)]   
    public float health = 100f;

    public Respawner thisRespawner;

    Animator animator;
    EnemyAI enemyAI;
    EnemyMovement enemyMovement;
    CharacterController characterController;
    private Rigidbody[] bodyParts;
}