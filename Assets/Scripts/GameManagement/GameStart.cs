using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
public class GameStart : MonoBehaviour
{
    public Canvas startCanvas;
    public Canvas playerInfoCanvas;
    public Canvas cancelCanvas;
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
        GameObject player = Instantiate(playerPrefeb, pos, rot) as GameObject;
        player.GetComponent<CharacterStates>().playerName = playerName;
        player.GetComponent<CharacterStates>().password = password;
        List<Weapon> curWeaponList = player.GetComponent<WeaponHandler>().weaponList;
        JsonConvert.DeserializeObject<>(result);
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


    }

}