using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    //  weaponType = 1
    Infantry,

    // Fal,
    // AK47,
    // AKsu,
    // G36,
    //  weaponType = 2
    Handgun,
    // Deserteagle,
    // Glock,
    //Knife, weaponType = 3
    Knife,
    //Heavy, weapontType = 4
    PKM,
}


public class Weapon : MonoBehaviour
{


    public WeaponType weaponType;

    [System.Serializable]
    public class UserSettings
    {
        // public Transform leftHandIKTarget;
        public Vector3 spineRotation;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class WeaponSettings
    {
        [Header("Other")]
        public GameObject crossHairPrefeb;
        public float reloadDuration = 1.0f;

        [Header("Bullet Options")]
        public float fireRate = 0.2f;
        public float damage = 20.0f;
        public Transform bulletSpawn;   //枪口位置
        public GameObject bulletPrefeb;
        public float bulletSpread = 0f;   //枪口晃动
        public float range = 200.0f;    //射程

        public int burstShootBulletNums = 3;

        [Header("AnimationClips")]
        public AnimationClip idleAnimation;
        public AnimationClip walkAnimation;
        public AnimationClip walkBackAnimation;
        public AnimationClip walkLeftAnimation;
        public AnimationClip walkRightAnimation;
        public AnimationClip runAnimation;
        public AnimationClip runBackAnimation;
        public AnimationClip runLeftAnimation;
        public AnimationClip runRightAnimation;

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
    public class SoundSettings
    {
        public AudioClip[] gunshotSounds;
        public AudioClip reloadSound;
        [Range(0, 3)]
        public float pitchMin = 1f;
        [Range(0, 3)]
        public float pitchMax = 1.2f;
        public AudioSource audioSource;
    }
    [SerializeField]
    public SoundSettings soundSettings;


    [System.Serializable]
    public class Ammunition
    {
        public int AmmoID;
        public int clipAmmo;
        public int maxClipAmmo;
    }
    [SerializeField]
    public Ammunition ammo;
    public Ray shootRay {get; set; }
    public bool ownerAiming { get; set; }
    WeaponHandler owner;
    bool equipped;  // 武器是否在被装备
    bool pullingTrigger;


    bool resettingCartridge;  // 切换武器

    Collider colBox;
    Collider colSphere;
    Rigidbody rgbody;
    Animator animator;
    SoundController sc;
    LineRenderer lineRenderer;

    private void Start()
    {
        colBox = GetComponent<BoxCollider>();
        colSphere = GetComponent<SphereCollider>();
        rgbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        sc = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();

        if (weaponSettings.crossHairPrefeb != null)
        {
            weaponSettings.crossHairPrefeb = Instantiate(weaponSettings.crossHairPrefeb);
            ToglleCrosshair(false);
        }
    }

    private void Update()
    {
        if (owner)
        {
            if (equipped)
            {
                if (owner.userSettings.weaponContainer)
                {
                    Equiq();
                    if (pullingTrigger)
                    {
                        Fire(shootRay);
                        //发射子弹
            //             Instantiate(weaponSettings.bulletPrefeb, weaponSettings.bulletSpawn.position,
            // weaponSettings.bulletSpawn.rotation);
                    }

                    // if (ownerAiming)
                    // {
                    //     ToglleCrosshair(true);
                    // }
                    
                    // else
                    // {
                    //     ToglleCrosshair(false);
                    // }
                    ToglleCrosshair(true);
                }
            }
            else
            {
                UnEquip(weaponType);
            }
        }
        else
        {
            ownerAiming = false;
            ToglleCrosshair(false);
        }
    }


    // 开枪 
    void Fire(Ray ray)
    {
        Debug.Log(ray);
        if (ammo.clipAmmo <= 0 || resettingCartridge || owner.reload)
        {
            return;
        }
        Debug.Log("weapon fire");
        // 瞄准点
        RaycastHit aimHit;
        Vector3 startPos = ray.origin;
        Vector3 aimDir = ray.direction;
        Physics.Raycast(startPos, aimDir, out aimHit);
        Debug.LogFormat("aimDir.point:{0},{1},{2}", aimDir.x, aimDir.y, aimDir.z);

        // 向瞄准点射击
        RaycastHit hit;
        Transform bSpawn = weaponSettings.bulletSpawn;
        Vector3 bSpawnPoint = bSpawn.position;
        Vector3 dir = aimHit.point - bSpawnPoint;

        // 子弹射线
        lineRenderer.SetPosition(0, bSpawnPoint);
        // lineRenderer.SetPosition(1, bSpawnPoint + (Camera.main.transform.forward * 2000));
        // StartCoroutine(StopShowLine());

        // 子弹发射

        // dir += (Vector3)Random.insideUnitCircle * weaponSettings.bulletSpread;
        
        if (Physics.Raycast(bSpawnPoint, dir, out hit, weaponSettings.range))
        {
            lineRenderer.SetPosition(1, hit.point);
            StartCoroutine(StopShowLine());
            // 伤害判定
            HitEffect(hit, weaponSettings.burstShootBulletNums);
        }

        GunEffects();
        // 弹药控制
        ammo.clipAmmo--;
        resettingCartridge = true;
        StartCoroutine(LoadNextBullet());
    }

    IEnumerator StopShowLine()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }
    IEnumerator LoadNextBullet()
    {
        yield return new WaitForSeconds(weaponSettings.fireRate);
        resettingCartridge = false;
    }

    void ToglleCrosshair(bool enabled)
    {
        if (weaponSettings.crossHairPrefeb != null)
        {
            weaponSettings.crossHairPrefeb.SetActive(enabled);
        }
    }

    // 对敌人造成伤害
    void HitEffect(RaycastHit hit, int bulletShootNums)
    {
        Debug.LogFormat("hit.transform.gameObject.tag:{0}", hit.transform.gameObject.tag);
        if (hit.transform.gameObject.tag == "Ragdoll")
        {
            EnemyStates health = hit.collider.GetComponent<EnemyStates>();
            if (health != null)
            {
                health.ApplyDamage(weaponSettings.damage);
                hit.transform.GetComponent<Animator>().ResetTrigger("TakeDamage");
                Debug.Log("hit enemy...");
            }
            else
            {
                Debug.Log("no enemy hit...");
            }
        }
    }

    // 攻击的一些效果
    void GunEffects()
    {
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

        if (sc == null)
        {
            Debug.LogError("sc == null...");
            return;
        }
        if (soundSettings.audioSource != null)
        {
            if (soundSettings.gunshotSounds.Length > 0)
            {
                sc.InstantiateClip(weaponSettings.bulletSpawn.position, soundSettings.gunshotSounds[Random.Range(0, soundSettings.gunshotSounds.Length)],
                2, true, soundSettings.pitchMin, soundSettings.pitchMax);
            }
        }
    }



    void Equiq()
    {
        if (!owner)
        {
            return;
        }
        else if (!owner.userSettings.weaponContainer) return;
        transform.SetParent(owner.userSettings.weaponContainer);
        transform.localPosition = weaponSettings.equipPosition;
        Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
        transform.localRotation = equipRot;
    }

    void UnEquip(WeaponType weaponType)
    {
        if (!owner) return;

        // if (weaponType == WeaponType.Glock || weaponType == WeaponType.Deserteagle)
        // {
        //     transform.SetParent(owner.userSettings.handgunUnequipSpot);
        // }
        // else if (weaponType == WeaponType.Fal || weaponType == WeaponType.AK47 || weaponType == WeaponType.AKsu || weaponType == WeaponType.G36)
        // {
        //     transform.SetParent(owner.userSettings.infantryUnequipSpot);
        // }
        if (weaponType == WeaponType.Infantry)
        {
            transform.SetParent(owner.userSettings.infantryUnequipSpot);
        }
        else if (weaponType == WeaponType.Handgun)
        {
            transform.SetParent(owner.userSettings.handgunUnequipSpot);
        }
        transform.localPosition = weaponSettings.unequipPosition;
        Quaternion unEquipRot = Quaternion.Euler(weaponSettings.unequipRotation);
        transform.localRotation = unEquipRot;
        ToglleCrosshair(false);
    }

    public void SetEquipped(bool equip)
    {
        equipped = equip;
    }
    public void SetOwner(WeaponHandler wp)
    {
        owner = wp;
        if (owner == null)
        {
            StartCoroutine(CanBePickUp());
        }
    }
    IEnumerator CanBePickUp()
    {
        yield return new WaitForSeconds(2.0f);
        colSphere.enabled = true;
    }

    public void LoadClip()
    {
        var container = owner.container;
        int ammoNeeded = ammo.maxClipAmmo - ammo.clipAmmo;
        ammo.clipAmmo += container.TakeFromContainer(ammo.AmmoID, ammoNeeded);
    }

    public void PullTrigger(bool isPulling)
    {
        pullingTrigger = isPulling;

    }


}
