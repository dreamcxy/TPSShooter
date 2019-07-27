using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameObject player;
    private UserInput userInput { get { return player.GetComponent<UserInput>(); } set { userInput = value; } }
    private PlayerUI playerUI { get { return FindObjectOfType<PlayerUI>(); } set { playerUI = value; } }
    private WeaponHandler wp { get { return player.GetComponent<WeaponHandler>(); } set { wp = value; } }
    private CharacterStates playerStates
    {
        get { return player.GetComponent<CharacterStates>(); }
        set { playerStates = value; }
    }
    private Container m_container;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {

        if (player)
        {
            var container = player.GetComponentInChildren<Container>();
            if (playerUI)
            {
                if (playerUI.healthBar && playerUI.healthText)
                {
                    playerUI.healthBar.value = playerStates.health;
                    playerUI.healthText.text = Mathf.Round(playerUI.healthBar.value).ToString();
                }
                List<Weapon> currentWeaponList = player.GetComponent<WeaponHandler>().weaponList;
                foreach (Weapon weapon in currentWeaponList)
                {
                    if (weapon.weaponType == WeaponType.Handgun)
                    {
                        if (playerUI.handgunAmmoText)
                        {
                            string text = string.Format("剩余子弹:{0}, 枪里面子弹:{1}, 总共能负弹:{2}", container.GetAmountRemaining(weapon.ammo.AmmoID), weapon.ammo.clipAmmo, 120);
                            playerUI.handgunAmmoText.text = text;
                        }

                    }
                    else if (weapon.weaponType == WeaponType.Infantry)
                    {

                        if (playerUI.infantryAmmoText)
                        {
                            string text = string.Format("剩余子弹:{0}, 枪里面子弹:{1}, 总共能负弹:{2}", container.GetAmountRemaining(weapon.ammo.AmmoID), weapon.ammo.clipAmmo, 180);
                            playerUI.infantryAmmoText.text = text;
                        }
                    }
                }
                if (Input.GetButtonDown(userInput.inputSettings.cancelButton))
                {
                    playerUI.QuitButtonPress();
                }
            }
        }
        else
        {
            GetPlayer();
        }
    }


    // 获取游戏对象
    public void GetPlayer()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");

    }

    public Container PlayerContainer()
    {
        if (m_container == null)
        {
            m_container = player.GetComponentInChildren<Container>();
        }
        return m_container;
    }

}