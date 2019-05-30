using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class GameStart : MonoBehaviour
{
    public Canvas startCanvas;
    public Canvas playerInfoCanvas;
    public Canvas cancelCanvas;
    public Canvas overCanvas;
    public GameObject playerPrefeb;

    LogIn logInOption = new LogIn();

    private void Awake()
    {
    }
    private void Start()
    {

        startCanvas.enabled = true;
        playerInfoCanvas.enabled = false;
        cancelCanvas.enabled = false;
        overCanvas.enabled = false;
    }

    public void LoginButton()
    {
        InputField[] inputFields = startCanvas.GetComponentsInChildren<InputField>();
        string playerName = inputFields[0].text;
        string password = inputFields[1].text;

        LogInInfo logInInfo = new LogInInfo();
        logInInfo.playerName = playerName;
        logInInfo.password = password;
        string jsonStr = JsonConvert.SerializeObject(logInInfo);
        Debug.Log(jsonStr);
        // 返回的数据信息， json格式
        string result = logInOption.SendText(jsonStr);
        Debug.Log(result);
        startCanvas.gameObject.SetActive(false);
        Vector3 pos = GameObject.Find("bornPosition").transform.position;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        // 根据返回的数据生成人物，人物的name和pass可以根据输入获得
        GameObject player = Instantiate(playerPrefeb, pos, rot) as GameObject;
        player.GetComponent<CharacterStates>().playerName = playerName;
        player.GetComponent<CharacterStates>().password = password;
        // 解析json数据，获得武器信息
        // Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)
        RootObject rb = JsonConvert.DeserializeObject<RootObject>(result);
        // player.GetComponent<CharacterStates>().health = float.Parse(rb.playerState.health);
        player.GetComponent<CharacterStates>().health = rb.playerState.health;
        List<WeaponInfo> weaponInfos = rb.weaponInfos;

        Container container = player.GetComponentInChildren<Container>();
        List<Weapon> curWeaponList = player.GetComponent<WeaponHandler>().weaponList;

        Weapon[] weaponsInWeaponContainer = player.GetComponent<WeaponHandler>().userSettings.weaponContainer.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weaponsInWeaponContainer)
        {
            foreach (WeaponInfo weaponInfo in weaponInfos)
            {
                if (weapon.weaponSettings.weaponName == weaponInfo.gunName.ToLower())
                {
                    curWeaponList.Add(weapon);
                    weapon.ammo.clipAmmo = weaponInfo.clipAmmos;
                    if (weaponInfo.clipLeft > container.GetAmountRemaining(weapon.ammo.AmmoID))
                    {
                        container.Put(weapon.ammo.AmmoID, weaponInfo.clipLeft - container.GetAmountRemaining(weapon.ammo.AmmoID));
                    }
                    else
                    {
                        container.TakeFromContainer(weapon.ammo.AmmoID, container.GetAmountRemaining(weapon.ammo.AmmoID) - weaponInfo.clipLeft);
                    }
                }
                else
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }

        player.GetComponent<WeaponHandler>().currentWeapon = curWeaponList[0];

        // Debug.Log(player.GetComponent<WeaponHandler>().weaponList);
    }



    public void RegisterButton()
    {

        InputField[] inputFields = startCanvas.GetComponentsInChildren<InputField>();
        string playerName = inputFields[0].text;
        string password = inputFields[1].text;
        Vector3 pos = GameObject.Find("bornPosition").transform.position;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject player = Instantiate(playerPrefeb, pos, rot) as GameObject;

        // player.transform.position = GameObject.Find("bornPosition").transform.position;
        // player.transform.Translate( GameObject.Find("bornPosition").transform.position);
        player.GetComponent<CharacterStates>().playerName = playerName;
        player.GetComponent<CharacterStates>().password = password;
        // startCanvas.enabled = false;
        startCanvas.gameObject.SetActive(false);
        playerInfoCanvas.enabled = true;

        List<Weapon> weaponList = player.GetComponent<WeaponHandler>().weaponList;
        Weapon[] weaponsInWeaponContainer = player.GetComponent<WeaponHandler>().userSettings.weaponContainer.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weaponsInWeaponContainer)
        {
            if (weapon.weaponSettings.weaponName == "glock" || weapon.weaponSettings.weaponName == "fal")
            {
                weaponList.Add(weapon);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }

    }

}