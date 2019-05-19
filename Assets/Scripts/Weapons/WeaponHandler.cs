using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) {}

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
public class WeaponHandler : MonoBehaviour
{
    Animator animator;
    AnimatorOverrideController animatorOverrideController;
    AnimationClipOverrides clipOverrides;

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

        public string singleShootBool = "isShoot";
        public string burstShootBool = "isShootBurst";


    }
    [SerializeField]
    public Animations animations;

    public Weapon currentWeapon;
    public List<Weapon> weaponList;

    bool aim;
    bool shootSingle;


    public bool reload { get; private set; }

    int weaponType;
    bool isSwitchingWeapon; //是否正在切换武器

    private void Start()
    {
        weaponList = new List<Weapon>();
        currentWeapon = userSettings.weaponContainer.GetComponentInChildren<Weapon>();
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
        if (currentWeapon)
        {
            weaponList.Add(currentWeapon);
        }
    }

    private void Update()
    {
        if (currentWeapon)
        {
            if(currentWeapon.weaponType == WeaponType.Glock){
                Debug.Log("glock...");
                clipOverrides["infantry_combat_reload"] = currentWeapon.weaponSettings.reloadAnimation;
            }
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
        // if (weaponList.Count > 0)
        // {
        //     for (int i = 0; i < weaponList.Count; i++)
        //     {
        //         if (weaponList[i] != currentWeapon)
        //         {
        //             weaponList[i].SetEquipped(false);
        //             // weaponList[i].SetOwner(this);
        //         }

        //     }
        // }
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
        animator.SetBool(animations.singleShootBool, shootSingle);
        Debug.LogFormat("shootSingle:{0}", shootSingle);
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
        StartCoroutine(StopReload());
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

        if (!isSwitchingWeapon && pulling && aim && !reload)
        {
            shootSingle = true;
            StartCoroutine(StopShoot());
        }
        else
        {
            shootSingle = false;
        }
    }

    IEnumerator StopShoot()
    {
        yield return new WaitForSeconds(0.1f);
        shootSingle = false;
    }
}