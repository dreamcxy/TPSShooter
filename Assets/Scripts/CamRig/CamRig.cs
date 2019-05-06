using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRig : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings{
        [Header("Camera Options")]
        public float mouseXSensitivity = 5.0f;
        public float mouseYSensitivity = 5.0f;

        public float rotateSpeed = 5.0f;
        public float minAngle = -30.0f;
        public float maxAngle = 60.0f;

    }
    [System.Serializable]
    public class InputSettings
    {
        [Header("Input settings")]
        public string verticalAxis = "Mouse X";    //鼠标移动纵向方向
        public string horizontalAxis = "Mouse Y";    //鼠标移动横向方向

    }
    [System.Serializable]
    public class MovementSettings
    {
        public float movementSpeed = 5.0f;
    }

    public Transform target;
    
    public CameraSettings cameraSettings;
    public MovementSettings movementSettings;
    public InputSettings inputSettings;
    float newCamX = 0.0f;
    float newCamY = 0.0f;

    Transform pivot;
    // Start is called before the first frame update
    void Start()
    {
        pivot = this.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            RotateCamera();
        }
    }

    void LateUpdate() {
        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;
        FollowTarget(targetPosition, targetRotation);
    }


    void FollowTarget(Vector3 targetPosition, Quaternion targetRotation){
        Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, 
            Time.deltaTime * movementSettings.movementSpeed);
        transform.position = newPos;
    }

    void RotateCamera(){
        if (!pivot)
        {
            return;
        }
        newCamX += cameraSettings.mouseXSensitivity * Input.GetAxis(inputSettings.verticalAxis);
        newCamY += cameraSettings.mouseYSensitivity * Input.GetAxis(inputSettings.horizontalAxis);

        Vector3 eulerAngleAxis =  new Vector3();
        eulerAngleAxis.x = -newCamY;
        eulerAngleAxis.y = newCamX;
        newCamX = Mathf.Repeat(newCamX, 360);
        newCamY = Mathf.Clamp(newCamY, cameraSettings.minAngle, cameraSettings.maxAngle);
        //从a角度转向b，经过t时间
        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * cameraSettings.rotateSpeed);
        pivot.localRotation = newRotation;
    }
}
