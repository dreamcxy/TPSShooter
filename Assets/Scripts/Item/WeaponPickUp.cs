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
            Weapon weaponGround = this.GetComponent<Weapon>();

            if (weaponHandler.currentWeapon.weaponType == WeaponType.Infantry || weaponHandler.currentWeapon.weaponType == WeaponType.Handgun)
            {

                List<Weapon> tempList = new List<Weapon>();
                currentWeaponList.ForEach(i => tempList.Add(i));
                int index = 0;
                for (int i = 0; i < tempList.Count; i++)
                {
                    Weapon subWeapon = tempList[i];
                    if (subWeapon.weaponType == weaponGround.weaponType)
                    {
                        index = i;
                        Destroy(subWeapon.gameObject);
                    }
                }
                currentWeaponList.RemoveAt(index);

                weaponGround.SetOwner(collider.gameObject.transform.GetComponent<WeaponHandler>());
                weaponGround.SetEquipped(true);
                // collider.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("WeaponContainer").transform);
                currentWeaponList.Add(weaponGround);
                collider.GetComponent<WeaponHandler>().currentWeapon = weaponGround;

            }

            this.GetComponent<SphereCollider>().enabled = false;
        }

    }
}