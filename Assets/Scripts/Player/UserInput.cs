using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 处理用户的按键行为
public class UserInput:MonoBehaviour
{
    public CharacterMovement characterMovement{get; protected set;}

    [System.Serializable]
    public class InputSettings{
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";
        public string runButton = "Fire3";
        
    }
    [SerializeField]
    public InputSettings inputSettings;

    public Camera tpsCamera;

    bool canJump = true;
    bool isJumpButtonDown;
    float verticalAxis;
    float horizontalAxis;
    bool isRunButtonDown;


    private void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        tpsCamera = Camera.main;
    }

    private void Update() {
        HandleInput();
        CharacterLogic();
    }

    void HandleInput(){
        isJumpButtonDown = Input.GetButtonDown(inputSettings.jumpButton);
        isRunButtonDown = Input.GetButton(inputSettings.runButton);
        verticalAxis = Input.GetAxis(inputSettings.verticalAxis);
        horizontalAxis = Input.GetAxis(inputSettings.horizontalAxis);
    }

    void CharacterLogic(){
        if(!characterMovement) return;
        if(isJumpButtonDown && canJump){
            characterMovement.Jump();
            canJump = false;
            StartCoroutine(CanJump());
        }
        characterMovement.isRun = isRunButtonDown;
        characterMovement.Animate(verticalAxis, horizontalAxis);
    }

    IEnumerator CanJump(){
        yield return new WaitForSeconds(characterMovement.movementSettings.jumpTime + 0.1f);
        canJump = true;
    }

}