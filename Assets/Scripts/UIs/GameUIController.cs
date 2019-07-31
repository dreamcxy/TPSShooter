using System;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine;

class GameUIController : MonoBehaviour
{
    [Header("Game UI")]
    
    public Canvas playerInfoCanvas;
    public Canvas cancelCanvas;
    public Canvas gameOverCanvas;

    [Header("Player Prefeb")]
    public GameObject playerPrefeb;


    bool isGameStart;
    private void Awake()
    {
        
    }

    private void Start()
    {
        playerInfoCanvas.enabled = false;
        cancelCanvas.enabled = false;
        gameOverCanvas.enabled = false;
    }

    private void Update()
    {
        isGameStart = GameObject.Find("NetworkProcess").GetComponent<NetWorkProcess>().gameStart;

        if (isGameStart)
        {
            playerInfoCanvas.enabled = true;
        }
    }

}

