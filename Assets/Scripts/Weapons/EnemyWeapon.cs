using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyWeaponType
{
    Infantry,
    Handgun,
    Knife
}


// Enemy 手里的weapon，不需要和Player的Weapon那么复杂
public class EnemyWeapon : MonoBehaviour
{
    public EnemyWeaponType enemyWeaponType;

    [System.Serializable]
    public class EnemyWeaponSettings
    {
        [Header("Bullet Options")]
        public Transform bulletSpawn;
        public GameObject bulletPrefeb;

        [Header("Effects")]
        public GameObject muzzleFlash;

        [Header("GunAnimation")]
        public AnimationClip enemyCombatIdle;
        public AnimationClip enemyCombatRun;
        public AnimationClip enemyCombatShoot;
        public AnimationClip ememyCombatWalk;
        public AnimationClip enemyDeath;
        public AnimationClip enemyTakeDamage;


    }

    public bool isFire;
    [SerializeField]
    public EnemyWeaponSettings enemyWeaponSettings;

    List<GameObject> bullets;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        bullets = new List<GameObject>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (isFire)
        {
            Fire();
            var bullet = Instantiate(enemyWeaponSettings.bulletPrefeb, enemyWeaponSettings.bulletSpawn.position,
            enemyWeaponSettings.bulletSpawn.rotation) as GameObject;
            // bullets.Add(bullet);
            Destroy(bullet, 5);
        }
    }

    public void Fire()
    {
        if (enemyWeaponSettings.muzzleFlash)
        {
            Vector3 bulletSpawnPos = enemyWeaponSettings.bulletSpawn.position;
            GameObject muzzleFlash = Instantiate(enemyWeaponSettings.muzzleFlash,
            bulletSpawnPos, Quaternion.identity) as GameObject;
            Transform muzzleT = muzzleFlash.transform;
            muzzleT.SetParent(enemyWeaponSettings.bulletSpawn);
            Destroy(muzzleFlash, 1.0f);
        }
        isFire = false;
    }

}