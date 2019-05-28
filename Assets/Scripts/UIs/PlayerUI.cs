using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI:MonoBehaviour{

    public Text infantryAmmoText;
    public Text handgunAmmoText;
    public Slider healthBar;
    public Text healthText;

    public GameObject damgeReact;


    public Canvas quitCanvas;
    public Canvas startCanvas;
    private void Start() {
        quitCanvas.enabled = false;
        
        
    }


    
    public void  QuitButtonPress()
    {
        quitCanvas.enabled = !quitCanvas.enabled;
        if (quitCanvas.enabled)
            UserInput.UnLockCursor();
        else
            UserInput.LockCursor();
    }

    //离开游戏，摧毁所有，上传状态
    public void LeftGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        List<Weapon> currentWeaponList = player.GetComponent<WeaponHandler>().weaponList;
        string playerName = player.GetComponent<CharacterStates>().playerName;
        float health = player.GetComponent<CharacterStates>().health;
        List<WeaponInfo> weaponInfos = new List<WeaponInfo>();
        foreach(Weapon weapon in currentWeaponList){
            string weaponName = weapon.name;
            int clipAmmos = weapon.ammo.clipAmmo;
            int clipAll = player.GetComponentInChildren<Container>().GetAmountRemaining(weapon.ammo.AmmoID);
            weaponInfos.Add(new WeaponInfo(weaponName, clipAmmos, clipAll));
        }
        // Data data = new Data(playerName, "&&&", health, weaponInfos);
        
        // SceneManager.LoadScene(0);
    }
}