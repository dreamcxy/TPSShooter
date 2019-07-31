using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [System.Serializable]
    public class TankSetttings
    {
        [Header("Attack Settings")]
        public GameObject crossHairPrefeb;
        public float tankAttackRange = 1000;

        public float bulletSpawnSpeed = 5;
        public GameObject bulletPrefeb;

        public Transform bulletSpawnPosition;

        [Header("Move Settings")]

        public float horizontalSpeed = 2.0f;
        public float verticalSpeed = 2.0f;
        public float rotateSpeed = 1.0f;

        [Header("Tank Component")]
        public Transform turret;
        public Transform chassis;
        public Transform trackRight;
        public Transform trackLeft;

    }

    [SerializeField]
    public TankSetttings tankSetttings;




    public Camera followCamera;         //跟随相机
    public float cameraSmoothTime = 0;
    private Vector3 velocity = Vector3.zero;




    private float moveSpeed = 10.0f;
    private float tankRotateSpeed = 30.0f;

    bool isTankCanFire = true;

    private void Awake()
    {
        this.GetComponent<TankController>().enabled = false;
    }
    private void Start()
    {

        if (tankSetttings.crossHairPrefeb != null)
        {
            tankSetttings.crossHairPrefeb = Instantiate(tankSetttings.crossHairPrefeb);
        }
        
    }

    private void Update()
    {


        // if (!followCamera)
        // {
        //     followCamera = Camera.main;
        // }

        RotateTurret();
        DisplayCrossHair(true);
        LeaveTank();
        // transform.position = new Vector3(transform.position.x + 1f * Time.deltaTime, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        TankMove();
    }


    public void DisplayCrossHair(bool enabled)
    {
        tankSetttings.crossHairPrefeb.SetActive(enabled);
        Ray ray = followCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hitInfo))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = followCamera.transform.forward * tankSetttings.tankAttackRange;
        }

        tankSetttings.crossHairPrefeb.transform.position = targetPoint;
        Debug.Log(isTankCanFire);
        if (Input.GetButtonDown("Fire1") && isTankCanFire)
        {
            GameObject bullet = Instantiate(tankSetttings.bulletPrefeb,
             tankSetttings.bulletSpawnPosition.position, tankSetttings.bulletSpawnPosition.rotation) as GameObject;
            isTankCanFire = false;
            StartCoroutine(LoadNextTankBullet());
        }


    }

    IEnumerator LoadNextTankBullet()
    {
        yield return new WaitForSeconds(tankSetttings.bulletSpawnSpeed);
        isTankCanFire = true;
    }

    public void TankMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Rotate(Vector3.up * tankRotateSpeed * horizontal * Time.deltaTime);
        transform.position += tankSetttings.chassis.forward * moveSpeed * vertical * Time.deltaTime;


    }
    // 旋转炮管
    void RotateTurret()
    {
        float x = tankSetttings.rotateSpeed * Input.GetAxis("Mouse X");
        //以下为相机与角色同步旋转是

        followCamera.transform.rotation = Quaternion.Euler(
        tankSetttings.turret.transform.rotation.eulerAngles +
        Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );//原理： 物体当前的欧拉角 + 鼠标x轴上的增量所产生的夹角

        tankSetttings.turret.transform.rotation = Quaternion.Euler(
            tankSetttings.turret.transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );//同理
        Vector3 TargetCameraPosition = tankSetttings.turret.transform.TransformPoint(new Vector3(0, 3f, -7f));//获取相机跟随的相对位置，再转为世界坐标

        followCamera.transform.position = Vector3.SmoothDamp(
            followCamera.transform.position,
            TargetCameraPosition,
            ref velocity,
            cameraSmoothTime, //最好为0
            Mathf.Infinity,
            Time.deltaTime
        );
    }



    void LeaveTank()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            // 从坦克中出来

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (player.GetComponent<CharacterStates>().isInTank == true)
                {
                    player.transform.Find("soldier").gameObject.SetActive(true);

                    player.GetComponent<CharacterStates>().isInTank = false;
                    player.transform.position = this.transform.position + new Vector3(1f, 0, 1f);

                    break;

                }
            }
            this.GetComponent<TankController>().enabled = false;
            // Camera.main.enabled = true;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ragdoll");
            foreach(var enemy in enemies)
            {

            }

        }
    }


}
