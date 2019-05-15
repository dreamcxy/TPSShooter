using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    //Infantry
    Fal,
    AK47,
    AKsu,
    G36,
    //Handgun
    Deserteagle,
    Glock,
    //Knife
    Knife,
    //Heavy
    PKM,
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class Weapon:MonoBehaviour{


    public WeaponType weaponType;

    [System.Serializable]
    public class UserSettings{
        public Transform leftHandIKTarget;
        public Vector3 spineRotation;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class WeaponSettings{
        [Header("Other")]
        public GameObject crossHairPrefeb;
        public float reloadDuration = 2.0f;
        
        [Header("Bullet Options")]
        public float fireRate = 0.2f;
        public float damage = 20.0f;
        public Transform bulletSpawn;   //枪口位置
        public float bulletSpread = 2.0f;   //枪口晃动
        public float range = 200.0f;    //射程

        [Header("Effects")]
        public GameObject muzzleFlash;  // 枪口火花

        [Header("Position")]
        public Vector3 equipPosition;
        public Vector3 equipRotation;
        public Vector3 unequipPosition;
        public Vector3 unequipRotation;
    }
    [SerializeField]
    public WeaponSettings weaponSettings;

    [System.Serializable]
    public class Ammunition{
        public int AmmoID;
        public int clipAmmo;
        public int maxClipAmmo;
    }
    [SerializeField]
    public Ammunition ammo;
    public Ray shootRay{protected get; set;}
    public bool ownerAiming{get; set;}
    WeaponHandler owner;
    bool equipped;  // 武器是否在被装备
    bool pullingTrigger;
    
    bool resettingCartridge;  // 切换武器

    Collider colBox;
    Collider colSphere;
    Rigidbody rgbody;
    Animator animator;



    private void Start() {
        colBox = GetComponent<BoxCollider>();
        colSphere = GetComponent<SphereCollider>();
        rgbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (weaponSettings.crossHairPrefeb != null)
        {
            weaponSettings.crossHairPrefeb = Instantiate(weaponSettings.crossHairPrefeb);
            ToglleCrosshair(false);
        }    
    }

    private void Update() {
        if (owner)
        {
           if (equipped)
           {
               if(owner.userSettings.rightHand){
                    Euiq();
                    if (pullingTrigger)
                    {
                        Fire(shootRay);
                    }
                    if (ownerAiming)
                    {
                        ToglleCrosshair(true);
                    }else{
                        ToglleCrosshair(false);
                    }
               }
           }else{
            //    Unequip(weaponType);
           }
        }else{
            ownerAiming = false;
            ToglleCrosshair(false);

        }
    }


    // 开枪 
    void Fire(Ray ray){
        if (ammo.clipAmmo <= 0 || resettingCartridge || owner.reload)
        {
            return;
        }
        RaycastHit aimHit;
        Vector3 startPos = ray.origin;
        Vector3 aimDir = ray.direction;
        Physics.Raycast(startPos, aimDir, out aimHit);

        RaycastHit hit;
        Transform bSpawn = weaponSettings.bulletSpawn;
        Vector3 bSpawnPoint = bSpawn.position;
        Vector3 dir = aimHit.point - bSpawnPoint;

        dir += (Vector3)Random.insideUnitCircle * weaponSettings.bulletSpread;
        if (Physics.Raycast(bSpawnPoint, dir, out hit, weaponSettings.range))
        {
            // 伤害判定
            HitEffect(hit);
        }

        GunEffects();
        // 弹药控制
        ammo.clipAmmo--;
        resettingCartridge = true;
        StartCoroutine(LoadNextBullet());
    }

    IEnumerator LoadNextBullet(){
        yield return new WaitForSeconds(weaponSettings.fireRate);
        resettingCartridge = false;
    }

    void ToglleCrosshair(bool enabled){
        if (weaponSettings.crossHairPrefeb != null)
        {
            weaponSettings.crossHairPrefeb.SetActive(enabled);
        }
    }

    // 对敌人造成伤害
    void HitEffect(RaycastHit hit){

    }

    // 攻击的一些效果
    void GunEffects(){
        #region muzzle flash
        if (weaponSettings.muzzleFlash)
        {
            Vector3 bulletSpawnPos = weaponSettings.bulletSpawn.position;
            GameObject muzzleFlash = Instantiate(weaponSettings.muzzleFlash, 
            bulletSpawnPos, Quaternion.identity) as GameObject;
            Transform muzzleT = muzzleFlash.transform;
            muzzleT.SetParent(weaponSettings.bulletSpawn);
            Destroy(muzzleFlash, 1.0f);
        }

        #endregion
    }


    void Euiq(){
        if (!owner)
        {
            return;
        }
        else if(!owner.userSettings.rightHand) return;
        transform.SetParent(owner.userSettings.rightHand);
        transform.localPosition = weaponSettings.equipPosition;
        Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
        transform.localRotation = equipRot;
    }

}
