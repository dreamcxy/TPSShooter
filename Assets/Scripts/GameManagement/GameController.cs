using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameObject player;
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
        player = GameObject.FindGameObjectWithTag("Player");
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
            if (playerUI)
            {
                if(playerUI.healthBar && playerUI.healthText){
                    playerUI.healthBar.value = playerStates.health;
                    playerUI.healthText.text = Mathf.Round(playerUI.healthBar.value).ToString();
                }
            }
        }
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