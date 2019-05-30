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

    public EnemyWeapon curEnemyWeapon;

    [HideInInspector]
    public GuardState guardState;
    [HideInInspector]
    public ChasingState chasingState;

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

        public float attackForTime;
        public float attackLatTime;

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
    private CharacterStates[] allPlayers;

    private EnemyAIState enemyAIState;

    #endregion

    private void Start()
    {
        attackSettings.attackForTime = Time.time;
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("no navmesh agent found....");
            enabled = false;
        }
        if (navMeshAgent.transform == this.transform)
        {
            Debug.LogError("navmesh agent should be a child of the character");
            enabled = false;
        }
        navMeshAgent.speed = 0;
        navMeshAgent.acceleration = 0;
        sightSettings.sightLayers = LayerMask.GetMask("Rigdoll");
        if (navMeshAgent.stoppingDistance == 0)
        {
            navMeshAgent.stoppingDistance = 1.5f;
        }
        InitializeAIState();
        GetAllCharacters();
    }

    private void Update()
    {
        
        navMeshAgent.transform.position = transform.position;
        GetAllCharacters();
        LookForTarget();
        enemyAIState.AIBehavior();
        // Debug.LogFormat("enemyAIState:{0}", enemyAIState.GetType().Name);
    }


    void GetAllCharacters()
    {
        allPlayers = GameObject.FindObjectsOfType<CharacterStates>();
        // Debug.LogFormat("allPlayers:{0}", allPlayers[0].gameObject.name);

    }

    void LookForTarget()
    {
        if (allPlayers.Length > 0)
        {
            CharacterStates c = ClosestCharacter();
            Vector3 start = transform.position + (transform.up * sightSettings.eyeHeight);
            Vector3 dir = (c.transform.position + c.transform.up) - start;

            float distance = Vector3.Distance(c.transform.position, start);
            float sightAngle = Vector3.Angle(dir, transform.forward);

            if (sightAngle < sightSettings.fieldOfView && distance < sightSettings.sightRange || distance < sightSettings.senseRange)
            {
                target = c.transform;
                targetLastKnownPosition = Vector3.zero;
                attackSettings.attackLatTime = Time.time;
                SetEnemyState(chasingState);
                animator.SetBool("Attack", false);
            }
            else{
                if (target != null)
                {
                    targetLastKnownPosition = target.position;
                }
                target = null;
            }
        }
    }

    public void SetEnemyState(EnemyAIState new_enemyAIState)
    {
        enemyAIState = new_enemyAIState;
    }


    // 最近的玩家
    CharacterStates ClosestCharacter()
    {
        CharacterStates closestCharacter = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (CharacterStates c in allPlayers)
        {
            float distToCharacter = Vector3.Distance(c.transform.position, transform.position);
            if (distToCharacter < minDistance)
            {
                closestCharacter = c;
                minDistance = distToCharacter;
            }
        }
        return closestCharacter;
    }

    public float LerpSpeed(float curSpeed, float destSpeed, float time)
    {
        return curSpeed = Mathf.Lerp(curSpeed, destSpeed, Time.deltaTime * time);
        // return curSpeed = destSpeed;
    }

    // enemy 朝向调整
    public void LookAtPosition(Vector3 pos){
        Vector3 dir = pos - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0;
        lookRot.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 5);
    }   

    public void SetLookAtTransform(Transform transform){
        currentLookTransform = transform;
    }

    void InitializeAIState(){
        guardState = new GuardState(this);
        chasingState = new ChasingState(this);
        enemyAIState = guardState;
    }
}