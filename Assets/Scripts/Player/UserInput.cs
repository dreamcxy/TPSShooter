using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 处理用户的按键行为
public class UserInput:MonoBehaviour
{
    public CharacterMovement characterMovement{get; protected set;}
    public WeaponHandler weaponHandler{get; protected set;}

    // 注册所有的按键
    [System.Serializable]
    public class InputSettings{

        // 武器类的button
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string reloadButton = "Reload";
        public string swtichWeaponButton = "SwitchWeapon";
        public string dropWeaponButton = "DropWeapon";
        
        // 移动类的button
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";
        public string runButton = "Fire3";
        // public string crouchButton  = "Fire1";
    }
    [SerializeField]
    public InputSettings inputSettings;

    [System.Serializable]
    public class OtherSettings{
        public float lookSpeed = 5.0f;
        public float lookDistance = 10.0f;
        public bool requireInputForTurn = true;
        public LayerMask aimDetectionLayers;
    }
    [SerializeField]
    public OtherSettings otherSettings;
    public bool debugAim;
    public Transform spine;
    public bool aiming{get; set;}


    public Camera tpsCamera;

    bool canJump = true;
    
    bool isFireButtonDown;
    bool isReloadButtonDown;
    bool isDropButtonDown;
    bool isSwitchButtonDown;
    Quaternion newRotation;
    Vector3 spineLookAt;

    bool isJumpButtonDown;
    float verticalAxis;
    float horizontalAxis;
    bool isRunButtonDown;

    [HideInInspector]
    public static bool isMouseOnUI = false;
        

    private void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        tpsCamera = Camera.main;
        LockCursor();
    }

    private void Update() {
        HandleInput();
        GetSpineTransform();
        CharacterLogic();
        CameraLookLoagic();
    }

    private void LateUpdate() {
        if(aiming){
            
            PositionSpine(spineLookAt);
        }
        
    }

    #region public Methods
        public static void LockCursor(){
            isMouseOnUI = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        public static void UnLockCursor(){
            isMouseOnUI = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    
    #endregion

    void HandleInput(){
        aiming = (Input.GetButton(inputSettings.aimButton) || debugAim) && !characterMovement.isRun;
        isFireButtonDown = Input.GetButton(inputSettings.fireButton);
        isReloadButtonDown = Input.GetButtonDown(inputSettings.reloadButton);
        isDropButtonDown = Input.GetButtonDown(inputSettings.dropWeaponButton);
        isSwitchButtonDown = Input.GetButtonDown(inputSettings.swtichWeaponButton);

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
        characterMovement.ApplyGravity(verticalAxis, horizontalAxis);
        characterMovement.Animate(verticalAxis, horizontalAxis);
    }

    void CharacterLook(){
        Transform mainCamT = tpsCamera.transform;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * otherSettings.lookDistance);
        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;
        newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * otherSettings.lookSpeed);
        transform.rotation = newRotation;
    }

    void CameraLookLoagic(){
        if(!tpsCamera) return;
        if(otherSettings.requireInputForTurn){
            if(Input.GetAxis(inputSettings.horizontalAxis) !=0 || Input.GetAxis(inputSettings.verticalAxis) != 0){
                CharacterLook();
            }
        }
        else{
            CharacterLook();
        }
    }


    IEnumerator CanJump(){
        yield return new WaitForSeconds(characterMovement.movementSettings.jumpTime + 0.1f);
        canJump = true;
    }

    void GetSpineTransform(){
        if(!spine)  return;
        Transform mainCamT = tpsCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 dir = mainCamT.forward;
        Ray ray = new Ray(mainCamPos, dir);
        spineLookAt = ray.GetPoint(400f);
    }

    void PositionSpine(Vector3 spineLookAt){
        if(!spine)  return;
        spine.LookAt(spineLookAt);
        spine.localEulerAngles = spine.localEulerAngles + new Vector3(0, 0, -90f);
        
        // 人物方向跟随枪管方向移动
        // Vector3 eulerAngleOffset = Vector3.zero;
        Vector3 eulerAngleOffset = weaponHandler.currentWeapon.userSettings.spineRotation;
        spine.Rotate(eulerAngleOffset);
    }

}