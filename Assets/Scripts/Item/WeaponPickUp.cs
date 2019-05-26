using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponPickUp : PickUpItems
{
    public WeaponType weaponType;
    public override void OnPickUp(Collider collider)
    {
        base.OnPickUp(collider);
        if (collider.gameObject.tag == "Player")
        {
            WeaponHandler weaponHandler = collider.gameObject.GetComponent<WeaponHandler>();
            Weapon weapon = weaponHandler.GetComponent<Weapon>();
            List<Weapon> currentWeaponList = weaponHandler.weaponList;
            Debug.LogFormat("currentWeaponList:{0}", currentWeaponList);
            Weapon weaponGround = this.GetComponent<Weapon>();

            if (weaponHandler.currentWeapon.weaponType == WeaponType.Infantry || weaponHandler.currentWeapon.weaponType == WeaponType.Handgun)
            {
                foreach (Weapon subWeapon in currentWeaponList)
                {
                    if (subWeapon.weaponType == weaponGround.weaponType)
                    {
                        Destroy(subWeapon);
                        currentWeaponList.Remove(subWeapon);
                        weaponGround.SetEquipped(true);
                        weaponGround.SetOwner(collider.gameObject.transform.GetComponent<WeaponHandler>());
                        currentWeaponList.Add(weaponGround);
                    }
                }
            }

            this.GetComponent<SphereCollider>().enabled = false;
        }

        // WeaponHandler weaponHandler = collider.gameObject.GetComponent<WeaponHandler>();
        // Weapon weapon = weaponHandler.GetComponent<Weapon>();
        // if(weaponHandler && weapon){
        //     if(weaponHandler.weaponList.Contains(weapon))   return;
        //     gameObject.GetComponent<SphereCollider>().enabled =  false;
        //     weaponHandler.AddWeaponToList(weapon);
        //     if(weaponHandler.currentWeapon == null) weaponHandler.currentWeapon = weapon;
        // }
    }
}