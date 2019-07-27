using UnityEngine;

public class CamRig : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Position")]
        // public Camera uICamera;
        public Vector3 camPositionOffsetLeft;
        public Vector3 camPositionOffsetRight;
        public Vector3 camAimPositionOffsetLeft;
        public Vector3 camAimPositionOffsetRight;

        [Header("Camera Options")]
        public float mouseXSensitivity = 5.0f;
        public float mouseYSensitivity = 5.0f;

        public float rotateSpeed = 5.0f;
        public float minAngle = -30.0f;
        public float maxAngle = 60.0f;

        [Header("Zoom")]
        public float fieldOfView = 70.0f;
        public float zoomFiledOfView = 30.0f;
        public float zoomSpeed = 4.0f;

    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings
    {
        [Header("Input settings")]
        public string verticalAxis = "Mouse X";    //鼠标移动纵向方向
        public string horizontalAxis = "Mouse Y";    //鼠标移动横向方向

        public string switchShoulderButton = "Fire4";

    }

    [SerializeField]
    public InputSettings inputSettings;

    [System.Serializable]
    public class MovementSettings
    {
        public float movementLerpSpeed = 5.0f;
    }
    [SerializeField]
    public MovementSettings movementSettings;


    public enum Shoulder
    {
        Right, Left
    }
    public Shoulder shoulder;


    public Transform target;
    UserInput userInput;
    float newCamX = 0.0f;
    float newCamY = 0.0f;

    Transform pivot;

    Camera mainCamera;
    GameObject tankCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        pivot = this.transform.GetChild(0);
        tankCamera = GameObject.Find("TankCamera");
        tankCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {



        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            userInput = player.GetComponent<UserInput>();
            if (target)
            {
                if (!UserInput.isMouseOnUI)
                {
                    RotateCamera();
                    Zoom(userInput.aiming);
                }

                if (Input.GetButtonDown(inputSettings.switchShoulderButton))
                {
                    SwitchShoulder();
                }
            }
            else
            {
                Transform playerT = player.transform;
                target = playerT;

            }

            if (player.GetComponent<CharacterStates>().isInTank == true)
            {
                tankCamera.SetActive(true);
                mainCamera.enabled = false;

            }
            else
            {
                tankCamera.SetActive(false);
                mainCamera.enabled = true;
            }

        }




    }

    void LateUpdate()
    {
        if (target)
        {
            Vector3 targetPosition = target.position;
            Quaternion targetRotation = target.rotation;
            FollowTarget(targetPosition, targetRotation);
        }

    }


    void FollowTarget(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 newPos = Vector3.Lerp(transform.position, targetPosition,
            Time.deltaTime * movementSettings.movementLerpSpeed);
        transform.position = newPos;
    }

    void RotateCamera()
    {
        if (!pivot)
        {
            return;
        }
        newCamX += cameraSettings.mouseXSensitivity * Input.GetAxis(inputSettings.verticalAxis);
        newCamY += cameraSettings.mouseYSensitivity * Input.GetAxis(inputSettings.horizontalAxis);

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = -newCamY;
        eulerAngleAxis.y = newCamX;
        newCamX = Mathf.Repeat(newCamX, 360);
        newCamY = Mathf.Clamp(newCamY, cameraSettings.minAngle, cameraSettings.maxAngle);
        //从a角度转向b，经过t时间
        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * cameraSettings.rotateSpeed);
        pivot.localRotation = newRotation;
    }

    void PositionCamera(Vector3 cameraPos)
    {
        if (!mainCamera)
        {
            return;
        }
        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.localPosition;
        Vector3 newPos = Vector3.Lerp(mainCamPos, cameraPos, Time.deltaTime * movementSettings.movementLerpSpeed);
        mainCamT.localPosition = newPos;
    }

    // 缩放照相机的view大小
    void Zoom(bool isZooming)
    {
        if (!mainCamera) return;
        if (isZooming)
        {
            float newFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.zoomFiledOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = newFieldOfView;
            // UICamera之后调整
            // if(cameraSettings.uICamera != null)
            // {
            //     cameraSettings.uICamera.fieldOfView = newFieldOfView;
            // }
            switch (shoulder)
            {
                case Shoulder.Left:
                    PositionCamera(cameraSettings.camAimPositionOffsetLeft);
                    break;
                case Shoulder.Right:
                    PositionCamera(cameraSettings.camAimPositionOffsetRight);
                    break;

            }
        }
        else
        {
            float originalFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.fieldOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = originalFieldOfView;
            // UICamera之后调整
            // if (cameraSettings.uICamera != null)
            // {
            //     cameraSettings.uICamera.fieldOfView = originalFieldOfView;
            // }
            switch (shoulder)
            {
                case Shoulder.Left:
                    PositionCamera(cameraSettings.camPositionOffsetLeft);
                    break;
                case Shoulder.Right:
                    PositionCamera(cameraSettings.camPositionOffsetRight);
                    break;
            }
        }
    }

    void SwitchShoulder()
    {
        switch (shoulder)
        {
            case Shoulder.Left:
                shoulder = Shoulder.Right;
                break;
            case Shoulder.Right:
                shoulder = Shoulder.Left;
                break;
        }
    }
}
