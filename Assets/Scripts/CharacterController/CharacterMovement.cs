using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    Animator animator;
    CharacterController characterController;

    // Animator中Move Controller的参数
    [System.Serializable]
    public class AnimationSettings
    {
        public string verticalVelocityFloat = "Forward";
        public string horizontalVelocityFloat = "Strafe";

        public string groudedBool = "isGrounded";
        public string runBool = "Run";

        // 开始跳跃
        public string jumpBool = "isJumping";

        // 跳跃过程空中最高点
        public string airBool = "isAir";

        // 跳跃下降过程
        public string landBool = "isLand";

    }

    [SerializeField]
    public AnimationSettings animationSettings;

    /*
        起跳速度
        起跳时间
     */
    [System.Serializable]
    public class MovementSettings
    {
        public float jumpSpeed = 1f;
        public float jumpTime = 1f;
        public float margin = 0.1f;
        public float walkSpeed = 2f;
        public float runSpeed = 4f;
    }
    [SerializeField]
    public MovementSettings movementSettings;

    /*
        重力加速度
        默认重力
        最初下落速度
     */
    [System.Serializable]
    public class PhysicsSettings
    {
        public float gravityModfier = 9.8f;
        public float resetDownSpeed = 0.1f;
    }
    [SerializeField]
    public PhysicsSettings physicsSettings;

    [HideInInspector]
    public bool isRun { get; set; }
    bool isGrounded;
    bool jumping;
    bool airing;
    bool landing;


    bool resetGravity;
    float downSpeed;    //下落速度

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        isGrounded = characterController.isGrounded; //调整CharacterController使得适配人物高度
        // Debug.LogFormat("characterController ground : {0}", isGrounded);
    }

    // 控制人物移动
    // public void ApplyGravity(float verticalAxis, float horizontalAxis){
    //     if(isGrounded){
    //         downSpeed = physicsSettings.resetDownSpeed;
    //     }
    //     else{
    //         downSpeed += Time.deltaTime * physicsSettings.resetDownSpeed;
    //     }
    //     Vector3 gravityVector = new Vector3(horizontalAxis, 0, verticalAxis);
    //     if(!jumping){
    //         gravityVector.y = -downSpeed;
    //     }
    //     else{
    //         gravityVector.y = movementSettings.jumpSpeed - downSpeed;
    //     }
    //     characterController.Move(gravityVector * Time.deltaTime);

    // }

    public void ApplyGravity()
    {
        if (isGrounded)
        {
            downSpeed = physicsSettings.resetDownSpeed;
        }
        else
        {
            downSpeed += Time.deltaTime * physicsSettings.resetDownSpeed;
        }
        Vector3 gravityVector = new Vector3();
        if (!jumping)
        {
            gravityVector.y = -downSpeed;
        }
        else
        {
            gravityVector.y = movementSettings.jumpSpeed - downSpeed;
        }
        characterController.Move(gravityVector * Time.deltaTime);

    }


    public void AnimateAndMove(float forward, float strafe)
    {
        animator.SetFloat(animationSettings.verticalVelocityFloat, forward);
        animator.SetFloat(animationSettings.horizontalVelocityFloat, strafe);
        Vector3 direction = new Vector3(strafe, 0, forward);
        direction = transform.TransformDirection(direction);
        float speed = movementSettings.walkSpeed;
        if (isRun)
        {
            speed = movementSettings.runSpeed;
        }
        direction *= Time.deltaTime * speed;
        characterController.Move(direction);
    }


    public void Jump()
    {
        // 跳跃过程中，跑步过程中都没有办法跳
        if (jumping || animator.GetBool("isJumping") || isRun)
        {
            return;
        }
        // 起跳时间和高度控制
        if (isGrounded)
        {
            // Debug.LogFormat("起跳前，isGrounded:{0}, jumping:{1}, airing:{2}, landing:{3}", isGrounded, jumping, airing, landing);

            // jumping = true;
            // animator.SetBool(animationSettings.jumpBool, jumping);
            // animator.SetBool("isGrounded", false);
            // StartCoroutine(JumpToAir());
            // Debug.LogFormat("起跳后，isGrounded:{0}, jumping:{1}, airing:{2}, landing:{3}", isGrounded, jumping, airing, landing);
            // StartCoroutine(StopJump());
            animator.SetTrigger("up");
            StartCoroutine(JumpToAir());
            animator.SetTrigger("down");
            StartCoroutine(StopJump());
        }   

        else
        {
            landing = false;
        }
    }

    IEnumerator JumpToAir()
    {
        yield return new WaitForSeconds(1f);
        airing = true;
        animator.SetBool(animationSettings.airBool, airing);
    }
    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(movementSettings.jumpTime);
        jumping = false;
        // landing = true;
        // animator.SetBool(animationSettings.landBool, landing);
    }


    public void Animate(float forward, float strafe)
    {
        animator.SetFloat(animationSettings.verticalVelocityFloat, forward);
        animator.SetFloat(animationSettings.horizontalVelocityFloat, strafe);
        animator.SetBool(animationSettings.groudedBool, isGrounded);

        animator.SetBool(animationSettings.runBool, isRun);
        // animator.SetBool(animationSettings.jumpBool, jumping);

    }
}