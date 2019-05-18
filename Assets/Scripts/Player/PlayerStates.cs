using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates:MonoBehaviour{
    [Range(0, 100)]
    public float health = 100.0f;
    public int ID;
    public string playerName;

    public WeaponHandler weaponHandler;


    public void ApplyDamage(float number){
        health -= number;
        if (health < 0)
        {
            Debug.Log("You are dead...");
        }
        if (health > 100)
        {
            health = 100;
        }
    }
}