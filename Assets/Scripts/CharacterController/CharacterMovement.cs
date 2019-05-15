using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement:MonoBehaviour{
    Animator animator;
    CharacterController characterController;

    // Animator中Move Controller的参数
    [System.Serializable]
    public class AnimationSettings{
        public string verticalVelocityFloat = "Forward";
        public string horizontalVelocityFloat = "Strafe";
        
        public string groudedBool = "isGrounded";
        public string runBool = "Run";
        public string jumpBool = "isJumping";

        // 跳跃过程中在空中
        public string airBool = "isAir";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    /*
        起跳速度
        起跳时间
     */
    [System.Serializable]
    public class MovementSettings{
        public float jumpSpeed = 4.0f;
        public float jumpTime = 1.0f;
        public float margin = 0.1f;
    }
    [SerializeField]
    public MovementSettings movementSettings;

    /*
        重力加速度
        默认重力
        最初下落速度
     */
    [System.Serializable]
    public class PhysicsSettings{
        public float gravityModfier = 9.8f;
        public float resetDownSpeed = 1.2f;
    }
    [SerializeField]
    public PhysicsSettings physicsSettings;

    [HideInInspector]
    public bool isRun{get;set;}
    bool isGrounded;
    bool jumping;
    bool resetGravity;
    float downSpeed;    //下落速度

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    private void Update() {
        // ApplyGravity();
        isGrounded = characterController.isGrounded; //调整CharacterController使得适配人物高度
        // Debug.LogFormat("characterController ground : {0}", isGrounded);
    }

    // 控制人物移动
    public void ApplyGravity(float verticalAxis, float horizontalAxis){
        if(isGrounded){
            downSpeed = physicsSettings.resetDownSpeed;
        }
        else{
            downSpeed += Time.deltaTime * physicsSettings.resetDownSpeed;
        }
        Vector3 gravityVector = new Vector3(horizontalAxis, 0, verticalAxis);
        if(!jumping){
            gravityVector.y = -downSpeed;
        }
        else{
            gravityVector.y = movementSettings.jumpSpeed - downSpeed;
        }
        characterController.Move(gravityVector * Time.deltaTime);
        
    }


    public void Jump(){
        // 跳跃过程中，跑步过程中都没有办法跳
        if(jumping || animator.GetBool("isJumping") || isRun){
            return;
        }
        // 起跳时间和高度控制
        if(isGrounded){
            jumping = true;
            StartCoroutine(StopJump());
        }
    }
    IEnumerator StopJump(){
        yield return new WaitForSeconds(movementSettings.jumpTime);
        jumping = false;
    }


    public void Animate(float forward, float strafe){
        animator.SetFloat(animationSettings.verticalVelocityFloat, forward);
        animator.SetFloat(animationSettings.horizontalVelocityFloat, strafe);
        animator.SetBool(animationSettings.groudedBool, isGrounded);
        animator.SetBool(animationSettings.jumpBool, jumping);
        animator.SetBool(animationSettings.runBool, isRun);
    }
}