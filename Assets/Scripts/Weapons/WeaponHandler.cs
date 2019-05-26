using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

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

        public Transform weaponContainer;
        // 手枪和步枪不装备的位置
        public Transform handgunUnequipSpot;
        public Transform infantryUnequipSpot;
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
    public Container container;
    

    bool aim;
    bool shootSingle;


    public bool reload { get; private set; }

    int weaponTypeInt;
    bool isSwitchingWeapon; //是否正在切换武器

    private void Start()
    {
        currentWeapon = userSettings.weaponContainer.GetComponentInChildren<Weapon>();
        container = GetComponentInChildren<Container>();
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
        if(!container){
            Debug.LogError("<Color=Red><a>Missing Container</a></Color>");
        }
    }

    // 初始的时候是两把武器，一把手枪，一把步枪
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
                    weaponList[i].SetOwner(this);
                    weaponList[i].gameObject.SetActive(true);
                }
            }
        }
        // Debug.LogFormat("currentWeapon.weaponType:{0}, currentWeapon.ammo.AmmoID:{1}, currentWeapon.ammo.clipAmmo:{2}, container.GetContainerItem(currentWeapon.ammo.AmmoID):{3}", currentWeapon.weaponType, currentWeapon.ammo.AmmoID, currentWeapon.ammo.clipAmmo, container.GetContainerItem(currentWeapon.ammo.AmmoID));

        Animate();
    }


    void Animate()
    {
        if (!animator)
        {
            return;
        }
        if (!currentWeapon)
        {
            weaponTypeInt = 0;
        }
        // if (currentWeapon.weaponType == WeaponType.AK47 || currentWeapon.weaponType == WeaponType.AKsu ||
        //     currentWeapon.weaponType == WeaponType.Fal || currentWeapon.weaponType == WeaponType.G36) weaponTypeInt = 1;
        // else if (currentWeapon.weaponType == WeaponType.Deserteagle || currentWeapon.weaponType == WeaponType.Glock) weaponTypeInt = 2;
        // else if (currentWeapon.weaponType == WeaponType.Knife) weaponTypeInt = 3;
        // else weaponTypeInt = 4;
        if (currentWeapon.weaponType == WeaponType.Infantry)    weaponTypeInt = 1;
        else if(currentWeapon.weaponType == WeaponType.Handgun)     weaponTypeInt = 2;
        else if(currentWeapon.weaponType == WeaponType.Knife)   weaponTypeInt = 3;
        else weaponTypeInt = 4;
        // Debug.LogFormat("weaponTypeInt:{0}", weaponTypeInt);
        clipOverrides["infantry_combat_idle"] = currentWeapon.weaponSettings.idleAnimation;
        clipOverrides["infantry_combat_walk"] = currentWeapon.weaponSettings.walkAnimation;
        clipOverrides["infantry_combat_walk_back"] = currentWeapon.weaponSettings.walkBackAnimation;
        clipOverrides["infantry_combat_walk_left"] = currentWeapon.weaponSettings.walkLeftAnimation;
        clipOverrides["infantry_combat_walk_right"] = currentWeapon.weaponSettings.walkRightAnimation;
        clipOverrides["infantry_combat_run"] = currentWeapon.weaponSettings.runAnimation;
        clipOverrides["infantry_combat_run_back"] = currentWeapon.weaponSettings.runBackAnimation;
        clipOverrides["infantry_combat_run_left"] = currentWeapon.weaponSettings.runLeftAnimation;
        clipOverrides["infantry_combat_run_right"] = currentWeapon.weaponSettings.runRightAnimation;
        animatorOverrideController.ApplyOverrides(clipOverrides);
        animator.SetInteger(animations.weaponTypeInt, weaponTypeInt);
        animator.SetBool(animations.reloadingBool, reload);
        animator.SetBool(animations.aimingBool, aim);
        animator.SetBool(animations.singleShootBool, shootSingle);
        
    }


    public void Reload()
    {
        if (reload || !currentWeapon) return;
        if(container.GetAmountRemaining(currentWeapon.ammo.AmmoID) <= 0 || currentWeapon.ammo.clipAmmo == currentWeapon.ammo.maxClipAmmo)   return;
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
        currentWeapon.transform.gameObject.SetActive(true);
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

    public void AddWeaponToList(Weapon weapon){
        if(weaponList.Contains(weapon)) return;
        weaponList.Add(weapon);
    }
}