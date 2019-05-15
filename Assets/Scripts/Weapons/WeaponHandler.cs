using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponHandler:MonoBehaviour{
    Animator animator;

    [System.Serializable]
    public class UserSettings{
        public Transform rightHand;
    }
    
    [SerializeField]
    public UserSettings userSettings;


    public class Animations{
        public string weaponTypeInt = "WeaponType";
        public string reloadingBool = "isReloading";
        public string aimBool = "aiming";
    }
    [SerializeField]
    public Animations animations;

    public Weapon currentWeapon;
    public List<Weapon> weaponList;
    
    bool aim;
    public bool reload{get; private set;}
    
    int weaponType;
    bool isSwitchingWeapon;

    private void Start() {
        animator = GetComponent<Animator>();
        weaponList = new List<Weapon>();

    }

    private void Update() {
        if (currentWeapon)
        {
            // currentWeapon.SetEquipped(true);
            // currentWeapon.SetOwner(this);
        }
    }


}