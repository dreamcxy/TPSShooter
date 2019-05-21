using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

class EnemyMovement:MonoBehaviour{
    Animator animator;
    CharacterController characterController;

    [System.Serializable]
    public class AnimationSettings{
        public string verticalVelocityFloat = "Forward";

    }
    [SerializeField]
    public AnimationSettings animationSettings;

    [System.Serializable]
    public class MovementSettings{
        public float moveSpeed = 1f;
        
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

    float downSpeed;
    bool isGrounded;

    private void Awake() {
        animator = GetComponent<Animator>();

    }

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }


    private void Update() {
        ApplyGravity();
        isGrounded = characterController.isGrounded;
    }

    public void AnimateAndMove(float forward, float strafe){
        animator.SetFloat(animationSettings.verticalVelocityFloat, forward);
        Vector3 direction = new Vector3(0, 0, forward);
        direction = transform.TransformDirection(direction);
        direction *= movementSettings.moveSpeed * Time.deltaTime;
        characterController.Move(direction);
    }

    public void JustAnimate(float forward, float strafe){
        animator.SetFloat(animationSettings.verticalVelocityFloat, forward);
    }

    public void ApplyGravity(){
        if (isGrounded)
        {
            downSpeed = physicsSettings.resetDownSpeed;
        }else{
            downSpeed += Time.deltaTime * physicsSettings.gravityModfier;
        }
        Vector3 gravityVector = new Vector3();
        gravityVector.y = -downSpeed;
        if (characterController != null)
        {
            characterController.Move(gravityVector * Time.deltaTime);
        }
    }

}