using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json;


public class GameOver : MonoBehaviour
{
    public Canvas overCanvas;

    LogIn logInOption = new LogIn();


    private void Update()
    {
        transform.Rotate(Vector3.up * 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("game over...");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ragdoll");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyStates>().EnemyDie();
                LeftGame();
            }
        }
    }
    //离开游戏，摧毁所有，上传状态
    public void LeftGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        List<Weapon> currentWeaponList = player.GetComponent<WeaponHandler>().weaponList;
        Container container = player.GetComponentInChildren<Container>();


        List<WeaponInfo> weaponInfos = new List<WeaponInfo>();
        foreach (Weapon weapon in currentWeaponList)
        {
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
        foreach (Weapon weapon in currentWeaponList)
        {
            WeaponInfo weaponInfo = new WeaponInfo(weapon.weaponSettings.weaponName, weapon.ammo.clipAmmo, container.GetAmountRemaining(weapon.ammo.AmmoID));
            rb.weaponInfos.Add(weaponInfo);
        }

        rb.signal = "0";
        string jsonStr = JsonConvert.SerializeObject(rb);
        logInOption.SendText(jsonStr);

        Debug.Log("push data over....");

        overCanvas.enabled = true;
        Time.timeScale = 0;
        // SceneManager.LoadScene(0);
    }
}