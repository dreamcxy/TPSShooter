using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
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

    LogIn logInOption = new LogIn();    
    // 按了no，也就是不离开游戏
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
        Container container = player.GetComponentInChildren<Container>();

        
        List<WeaponInfo> weaponInfos = new List<WeaponInfo>();
        foreach(Weapon weapon in currentWeaponList){
            string weaponName = weapon.name;
            int clipAmmos = weapon.ammo.clipAmmo;
            int clipAll = player.GetComponentInChildren<Container>().GetAmountRemaining(weapon.ammo.AmmoID);
            weaponInfos.Add(new WeaponInfo(weaponName, clipAmmos, clipAll));
        }
        // 构造需要上传的参数

        RootObject rb = new RootObject();
        rb.playerName = player.GetComponent<CharacterStates>().playerName;
        rb.password = player.GetComponent<CharacterStates>().password;
        rb.playerState = new PlayerState(player.GetComponent<CharacterStates>().health);
        rb.weaponInfos = new List<WeaponInfo>();
        foreach(Weapon weapon in currentWeaponList){
            WeaponInfo weaponInfo = new WeaponInfo(weapon.weaponSettings.weaponName, weapon.ammo.clipAmmo, container.GetAmountRemaining(weapon.ammo.AmmoID));
            rb.weaponInfos.Add(weaponInfo);
        }

        rb.signal = "0";
        string jsonStr = JsonConvert.SerializeObject(rb);
        logInOption.SendText(jsonStr);
        Debug.Log("push data over....");
        
        // Data data = new Data(playerName, "&&&", health, weaponInfos);
        
        // SceneManager.LoadScene(0);
    }
}