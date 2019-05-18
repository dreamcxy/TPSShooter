using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponHandler : MonoBehaviour
{
    Animator animator;

    [System.Serializable]
    public class UserSettings
    {
        // public Transform rightHand;
        public Transform weaponContainer;
    }

    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class Animations
    {

        public string weaponTypeInt = "WeaponTypeInt";
        public string reloadingBool = "isReloading";
        public string aimingBool = "aiming";

    }
    [SerializeField]
    public Animations animations;

    public Weapon currentWeapon;
    public List<Weapon> weaponList;

    bool aim;
    public bool reload { get; private set; }

    int weaponType;
    bool isSwitchingWeapon; //是否正在切换武器

    private void Start()
    {
        weaponList = new List<Weapon>();

        currentWeapon = userSettings.weaponContainer.GetComponentInChildren<Weapon>();

        animator = GetComponent<Animator>();
        if (currentWeapon)
        {
            weaponList.Add(currentWeapon);
        }
    }

    private void Update()
    {
        if (currentWeapon)
        {
            currentWeapon.SetEquipped(true);
            currentWeapon.SetOwner(this.GetComponent<WeaponHandler>());

            currentWeapon.ownerAiming = aim;
            if (currentWeapon.ammo.clipAmmo <= 0)
            {
                Reload();
            }
            // 正在换弹的时候切换武器，停止换弹
            if (reload)
            {
                if (isSwitchingWeapon)
                {
                    reload = false;
                }
            }
        }
        if (weaponList.Count > 0)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (weaponList[i] != currentWeapon)
                {
                    weaponList[i].SetEquipped(false);
                    // weaponList[i].SetOwner(this);
                }

            }
        }
        Animate();
    }


    void Animate()
    {
        if (!animator)
        {
            return;
        }
        
        
        animator.SetBool(animations.reloadingBool, reload);

        animator.SetBool(animations.aimingBool, aim);


        if (!currentWeapon)
        {
            weaponType = 0;
        }
        if (currentWeapon.weaponType == WeaponType.AK47 || currentWeapon.weaponType == WeaponType.AKsu ||
        currentWeapon.weaponType == WeaponType.Fal || currentWeapon.weaponType == WeaponType.G36) weaponType = 1;
        else if (currentWeapon.weaponType == WeaponType.Deserteagle || currentWeapon.weaponType == WeaponType.Glock) weaponType = 2;
        else if (currentWeapon.weaponType == WeaponType.Knife) weaponType = 3;
        else weaponType = 4;
        animator.SetInteger(animations.weaponTypeInt, weaponType);
    }


    public void Reload()
    {
        if (reload || !currentWeapon) return;
        reload = true;
    }

    IEnumerator StopReload()
    {
        yield return new WaitForSeconds(currentWeapon.weaponSettings.reloadDuration);
        if (reload && currentWeapon)
        {
            currentWeapon.LoadClip();
        }
        reload = false;
    }

    public void Aim(bool aiming)
    {
        aim = aiming;
    }

    public void SwitchWeapons()
    {
        if (isSwitchingWeapon || weaponList.Count <= 0)
        {
            return;
        }

        if (currentWeapon)
        {
            if (weaponList.Count == 1)
            {
                return;
            }
            int currentWeaponIndex = weaponList.IndexOf(currentWeapon);
            int nextWeaponIndex = (currentWeaponIndex + 1) % weaponList.Count;
            currentWeapon = weaponList[nextWeaponIndex];
        }
        else
        {
            currentWeapon = weaponList[0];
        }
        isSwitchingWeapon = true;
        StartCoroutine(StopSwitchingWeapon());
    }
    IEnumerator StopSwitchingWeapon()
    {
        yield return new WaitForSeconds(0.7f);
        isSwitchingWeapon = false;
    }

    public void FingerOnTrigger(bool pulling)
    {
        if (!currentWeapon)
        {
            return;
        }
        currentWeapon.PullTrigger(!isSwitchingWeapon && pulling && aim && !reload);
    }


}