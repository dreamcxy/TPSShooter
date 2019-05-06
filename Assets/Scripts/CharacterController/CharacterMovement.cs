using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class CharacterMovement : MonoBehaviour
// {
    
//     public float speed = 6.0f;
//     public float jumpSpeed = 8.0f;
//     public float gravity = 10.0f;
//     public Vector3 moveDirection = Vector3.zero;

//     // Start is called before the first frame update
//     CharacterController controller;
//     Animator animator;

    
//     private void Awake() {
        
//         animator = GetComponent<Animator>();
//     }

//     void Start()
//     {
//         controller = GetComponent<CharacterController>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//     //     if (controller.isGrounded)
//     //     {
        
//         moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
//         moveDirection = transform.TransformDirection(moveDirection);
//         moveDirection *= speed;
//         if (Input.GetButton("Jump"))
//         {
//             moveDirection.y = jumpSpeed;
//         }
//         moveDirection.y -= gravity * Time.deltaTime;
//         controller.Move(moveDirection * Time.deltaTime);
//         // }
//     }

//     //根据WeaponType选取合适的动画，利用子物体WeaponContainer下的记录来记录soldier目前的武器
//     void SetupAnimator(){
        
//     }

// }




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
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    /*
        起跳速度
        起跳时间
     */
    [System.Serializable]
    public class MovementSettings{
        public float jumpSpeed = 6.0f;
        public float jumpTime = 0.5f;
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
        ApplyGravity();
        isGrounded = true; //目前人物和地面之间没有调整好，暂且人为设置为全在地面上
    }

    // 控制人物移动
    void ApplyGravity(){
        if(isGrounded){
            downSpeed = physicsSettings.resetDownSpeed;
        }
        else{
            downSpeed += Time.deltaTime * physicsSettings.resetDownSpeed;
        }
        Vector3 gravityVector = new Vector3();
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