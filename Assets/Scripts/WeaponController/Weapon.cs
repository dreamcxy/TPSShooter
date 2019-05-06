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


public class Weapon:MonoBehaviour{
    

    public WeaponType weaponType;
}
