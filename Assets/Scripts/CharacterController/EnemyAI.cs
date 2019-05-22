using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(EnemyMovement))]

[System.Serializable]
public class WayPointBase
{
    public Transform destination;
    public float waitTime;
    public Transform lookAtTarget;
}
public class EnemyAI : MonoBehaviour
{
    //     private NavMeshAgent m_navMeshAgent;
    //     public Transform player;
    //     private CharacterController characterController;

    //     private void Start() {
    //         characterController = GetComponent<CharacterController>();
    //         m_navMeshAgent = GetComponent<NavMeshAgent>();

    //     }

    //     private void Update() {
    //         m_navMeshAgent.SetDestination(player.position);
    //         characterController.Move(this.transform.TransformDirection(new Vector3(0, 0, 0.1f)));
    //     }

    #region public variable



    [HideInInspector]
    GuardState guardState;

    [System.Serializable]
    public class GuardSettings
    {
        public WayPointBase[] wayPoints;
    }
    public GuardSettings guardSettings;

    [System.Serializable]
    public class SightSettings
    {
        public LayerMask sightLayers;
        public float sightRange = 30f;
        public float fieldOfView = 80f;
        public float senseRange = 10f;
        public float eyeHeight = 2.0f;
    }
    [SerializeField]
    public SightSettings sightSettings;

    [System.Serializable]
    public class AttackSettings
    {
        public float damge = 5f;
        public float attackRange = 3f;
        public float attackDelay = 2f;
        public float attackCoolDown = 2f;
        public int attackType = 2;

    }
    [SerializeField]
    public AttackSettings attackSettings;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public bool walkingToDest;

    [HideInInspector]
    public float forward;
    [HideInInspector]
    public Transform target;    // 追逐Player

    #endregion

    #region private variable
    private EnemyMovement enemyMovement
    {
        get { return GetComponent<EnemyMovement>(); }
        set { enemyMovement = value; }
    }
    private Animator animator
    {
        get { return GetComponent<Animator>(); }
        set { animator = value; }
    }

    private EnemyStates enemyStates
    {
        get { return GetComponent<EnemyStates>(); }
        set { enemyStates = value; }

    }

    private Transform currentLookTransform;
    private Vector3 targetLastKnownPosition;
    private PlayerStates[] allPlayers;

    private EnemyAIState enemyState;
 
    #endregion

}