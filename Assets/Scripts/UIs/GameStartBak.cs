using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
public class GameStartBak : MonoBehaviour
{
    public Canvas startCanvas;
    public Canvas playerInfoCanvas;
    public Canvas cancelCanvas;
    public Canvas roomCanvas;

    public Canvas overCanvas;
    public GameObject playerPrefeb;

    Text roomInfoText;
    GameObject startGameButton;


    LogIn logInOption = new LogIn();


    bool debug = true;

    private void Awake()
    {
        roomInfoText = GameObject.Find("RoomInfo").GetComponent<Text>();
        startGameButton = GameObject.Find("StartGameButton");
        
    }
    private void Start()
    {

        startCanvas.enabled = true;
        playerInfoCanvas.enabled = false;
        cancelCanvas.enabled = false;
        overCanvas.enabled = false;

        roomCanvas.enabled = true;
        if (startGameButton)
        {
            Debug.Log("button found...");
            startGameButton.SetActive(true);
        }
        
        if (!debug)
        {
            GetRoomInfo();
        }
    }


    public void LoginButton()
    {
        InputField[] inputFields = startCanvas.GetComponentsInChildren<InputField>();
        string playerName = inputFields[0].text;
        string password = inputFields[1].text;

        LoginInfo logInInfo = new LoginInfo();
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

        // 创建玩家

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


    //  加入房间
    public void AttendRoom()
    {
        InputField[] inputFields = roomCanvas.GetComponentsInChildren<InputField>();

        string roomName = inputFields[0].text;
        string playerName = inputFields[1].text;
        string playerPassword = inputFields[2].text;

        string signal = "attend";
        RoomInfo roomInfo = new RoomInfo(roomName, playerName, playerPassword, signal);
        string jsonStr = JsonConvert.SerializeObject(roomInfo);
        string returnRoomInfo = logInOption.SendText(jsonStr);
        Dictionary<string, List<string>> result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(returnRoomInfo);
        roomInfoText.text = "wait room owner start...";


    }


    // 获取房间信息
    public void GetRoomInfo()
    {
        RoomInfo roomInfo = new RoomInfo("", "", "", "search");
        string jsonStr = JsonConvert.SerializeObject(roomInfo);
        string returnRoomInfo = logInOption.SendText(jsonStr);
        Debug.Log(returnRoomInfo);

        // 显示文本

        Dictionary<string, List<string>> result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(returnRoomInfo);

        Debug.Log(result);

        UpdateRoomsText(result);
    }

    // 创建房间
    public void CreateRoom()
    {
        InputField[] inputFields = roomCanvas.GetComponentsInChildren<InputField>();

        string roomName = inputFields[0].text;
        string playerName = inputFields[1].text;
        string playerPassword = inputFields[2].text;

        string signal = "create";
        RoomInfo roomInfo = new RoomInfo(roomName, playerName, playerPassword, signal);
        string jsonStr = JsonConvert.SerializeObject(roomInfo);
        string returnRoomInfo = logInOption.SendText(jsonStr);
        Dictionary<string, List<string>> result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(returnRoomInfo);
        Debug.Log(result[roomName].Count);
        if (result[roomName].Count == 1 )
        {
            Debug.Log("等待玩家加入...");
            roomInfoText.text = "" + roomName + ":" + result[roomName][0];
        }
        else if (result[roomName].Count > 1){
            startGameButton.SetActive(true);

        }   
        StartCoroutine(QueryRoomInfo(roomName));
    }

    // 更新房间板的信息
    public void UpdateRoomsText(Dictionary<string, List<string>> result)
    {
        if (roomInfoText)
        {
            Debug.Log("update room text...");
            var roomText = "";
            foreach (var key in result.Keys)
            {
                string players = "";
                foreach (var player in result[key])
                {
                    players = players + player + "\t";
                }
                roomText = roomText + key + ":" + players + "\n";
            }
            roomInfoText.text = roomText;

        }
    }

    IEnumerator QueryRoomInfo(string roomName){
        while(true){
            yield return new WaitForSeconds(2);
            RoomInfo roomInfo = new RoomInfo(roomName, "", "", "query");
            string jsonStr = JsonConvert.SerializeObject(roomInfo);
            string returnRoomInfo = logInOption.SendText(jsonStr);
            Dictionary<string, List<string>> result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(returnRoomInfo);
            if (result[roomName].Count > 1)
            {
                startGameButton.SetActive(true);
                var roomText = "";
                string players = "";
                foreach (var player in result[roomName])
                {
                    players = players + player + "\t";
                }
                roomText = roomText + roomName + ":" + players + "\n";
                break;
            }else{
                Debug.Log("no player attend...");
            }
        
        }
    }


    public void StartGame(){

        roomCanvas.enabled = false;


        string playerName = "test1";
        string password = "";
        Vector3 pos = GameObject.Find("bornPosition").transform.position;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject player = Instantiate(playerPrefeb, pos, rot) as GameObject;

        // player.transform.position = GameObject.Find("bornPosition").transform.position;
        // player.transform.Translate( GameObject.Find("bornPosition").transform.position);
        player.GetComponent<CharacterStates>().playerName = playerName;
        player.GetComponent<CharacterStates>().password = password;
        // startCanvas.enabled = false;
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