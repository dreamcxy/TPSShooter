using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponPickUp:PickUpItems{
    public override void OnPickUp(Collider collider){
        base.OnPickUp(collider);
        WeaponHandler weaponHandler = collider.gameObject.GetComponent<WeaponHandler>();
        Weapon weapon = weaponHandler.GetComponent<Weapon>();
        if(weaponHandler && weapon){
            if(weaponHandler.weaponList.Contains(weapon))   return;
            gameObject.GetComponent<SphereCollider>().enabled =  false;
            weaponHandler.AddWeaponToList(weapon);
            if(weaponHandler.currentWeapon == null) weaponHandler.currentWeapon = weapon;
        }
    }
}