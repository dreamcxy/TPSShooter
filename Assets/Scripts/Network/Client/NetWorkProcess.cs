using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using System.Net;

public class NetWorkProcess : MonoBehaviour
{
    [Header("Network")]
    public String host = "127.0.0.1";
    public Int32 port = 2000;
    public bool debug;


    [Header("GameData")]
    public Canvas startCanvas;
    public GameObject playerPrefeb;

    public bool gameStart;

    internal Boolean socket_ready = false;

    TcpClient tcp_socket;
    //Socket tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;


    ConcurrentQueue<Message> sendQueue = new ConcurrentQueue<Message>();
    ConcurrentQueue<string> recvQueue = new ConcurrentQueue<string>();

    

    GameObject localPlayer;
    GameObject[] enemies;


    Dictionary<string, Transform> playersDict = new Dictionary<string, Transform>();
    
    bool gameOver;

    void Awake()
    {
        if (!debug)
        {
            startCanvas.enabled = true;
        }
        SetupSocket();
        if (debug) gameStart = true;
        
    }
    private void Start()
    {
    
    }

    void TargetLocalCharacters()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        enemies = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<CharacterStates>().isLocal == true)
            {
                localPlayer = player;
                Debug.LogFormat("localPlayer:{0}",localPlayer);
                playersDict.Add(localPlayer.GetComponent<CharacterStates>().ID, localPlayer.transform);
                break;
            }
        }
    }


    void Update()
    {
        if (!localPlayer)
        {
            TargetLocalCharacters();
        }


        if (gameStart && !gameOver)
        {
            StartCoroutine(FillSendQueue());
            Debug.Log(sendQueue.Count);
            if (socket_ready)
            {
                while (!sendQueue.IsEmpty)
                {
                    Message sendContent;
                    sendQueue.TryDequeue(out sendContent);
                    if (sendContent is AvatarStates)
                    {
                        if (sendContent.tag == "Player")
                        {
                            string sendJsonStr = JsonConvert.SerializeObject(sendContent);
                            //Debug.LogFormat("Send:{0}", sendJsonStr);
                            WriteSocket(sendJsonStr);
                        }

                    }

                }

                StartCoroutine(FillRecvQueue());
                while (!recvQueue.IsEmpty)
                {
                    string recvStr;
                    recvQueue.TryDequeue(out recvStr);
                    if (recvStr.Contains("sync"))
                    {
                        try
                        {
                            AvatarStates avatarStates = JsonConvert.DeserializeObject<AvatarStates>(recvStr);
                        
                        if (avatarStates.tag == "Player")
                        {
                            
                            if(avatarStates.isLocal == false)
                            {
                                if (!playersDict.ContainsKey(avatarStates.playerID))
                                {
                                    GameObject networkPlayer = Instantiate(playerPrefeb,
                                    new Vector3(avatarStates.position.x, avatarStates.position.y, avatarStates.position.z),
                                    Quaternion.Euler(new Vector3(avatarStates.rotation.x, avatarStates.rotation.y
                                    , avatarStates.rotation.z))) as GameObject;
                                    networkPlayer.GetComponent<UserInput>().enabled = false;
                                        networkPlayer.GetComponent<CharacterStates>().ID = avatarStates.playerID;
                                    playersDict.Add(avatarStates.playerID, networkPlayer.transform);
                                }
                                else
                                {
                                        Debug.LogFormat("Recv Player{0}:{1}, {2}, {3}", avatarStates.playerID, avatarStates.position.x,
                                       avatarStates.position.y, avatarStates.position.z);
                                        Debug.Log("network player move");
                                        Debug.Log(playersDict[avatarStates.playerID]);
                                        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
                                        foreach(var t in temp)
                                        {
                                            if(t.GetComponent<CharacterStates>().ID == avatarStates.playerID)   t.transform.position = new Vector3(avatarStates.position.x, avatarStates.position.y, avatarStates.position.z);
                                        }
                                    //playersDict[avatarStates.playerID].position = new Vector3(avatarStates.position.x, avatarStates.position.y, avatarStates.position.z);
                                }
                                
                            }
                        }
                        else if (avatarStates.tag == "Ragdoll")
                        {
                            //Debug.LogFormat("Recv Enemy:{0}, {1}, {2}", avatarStates.position.x,
                            //avatarStates.position.y, avatarStates.position.z);
                        }
                        }
                        catch (Exception e)
                        {
                            Debug.LogFormat("recvStr：{0}，", recvStr);

                        }
                    }

                }
            }

        }
    }


    void OnApplicationQuit()
    {
        WriteSocket("");
        closeSocket();
        gameOver = true;
    }

    IEnumerator FillSendQueue()
    {
        AvatarStates localPlayerStates = new AvatarStates();
        localPlayerStates.health = localPlayer.GetComponent<CharacterStates>().health;
        localPlayerStates.forward = localPlayer.GetComponent<Animator>().GetFloat("Forward") * Conf.MULTIPLE;
        localPlayerStates.strafe = localPlayer.GetComponent<Animator>().GetFloat("Strafe") * Conf.MULTIPLE;
        Debug.LogFormat("forward:{0}, strafe:{1}", localPlayerStates.forward, localPlayerStates.strafe);
        localPlayerStates.isRun = localPlayer.GetComponent<CharacterMovement>().isRun;
        localPlayerStates.isFire = localPlayer.GetComponent<WeaponHandler>().shootSingle;
        localPlayerStates.isReload = localPlayer.GetComponent<WeaponHandler>().reload;
        localPlayerStates.isInTank = localPlayer.GetComponent<CharacterStates>().isInTank;
        //localPlayerStates.isLocal = localPlayer.GetComponent<CharacterStates>().isLocal;
        localPlayerStates.playerID = localPlayer.GetComponent<CharacterStates>().ID;
        localPlayerStates.tag = localPlayer.tag;
        localPlayerStates.position = new Position(localPlayer.transform.position.x,
            localPlayer.transform.position.y, localPlayer.transform.position.z);

        localPlayerStates.rotation = new Rotaion(localPlayer.transform.rotation.x,
            localPlayer.transform.rotation.y, localPlayer.transform.rotation.z);

        sendQueue.Enqueue(localPlayerStates);

        //foreach (var enemy in enemies)
        //{
        //    if (enemy) {
        //        AvatarStates enemyStates = new AvatarStates();
        //        enemyStates.health = enemy.GetComponent<EnemyStates>().health;
        //        enemyStates.forward = enemy.GetComponent<Animator>().GetFloat("Forward") * Conf.MULTIPLE;
        //        enemyStates.isFire = enemy.GetComponent<Animator>().GetBool("Attack");
        //        enemyStates.tag = enemy.tag;
        //        enemyStates.position = new Position(enemy.transform.position.x,
        //            enemy.transform.position.y, enemy.transform.position.z);
        //        enemyStates.rotation = new Rotaion(enemy.transform.rotation.x,
        //            enemy.transform.rotation.y, enemy.transform.position.z);
        //        sendQueue.Enqueue(enemyStates);
        //    }

        //}
        yield return new WaitForSeconds(Conf.CLICK_TIME);

    }

    IEnumerator FillRecvQueue()
    {
    
        yield return new WaitForSeconds(Conf.CLICK_TIME);
        recvQueue.Enqueue(ReadSocket());
    }

    public void SetupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
            // Something went wrong
            Debug.Log("Socket error: " + e);
        }

        //try
        //{
        //    IPAddress ip = IPAddress.Parse(host);
        //    tcp_socket = new Socket(AddressFamily.InterNetwork,
        //        SocketType.Stream, ProtocolType.Tcp);
        //    tcp_socket.Connect(new IPEndPoint(ip, port));
        //    socket_ready = true;
        //}catch(Exception ex)
        //{
        //    Debug.LogFormat("Exception:\n{0}\n BaseException:\n{1} \n GetType:\n{2} \nMessage:\n{3}\n StackTrace:\n{4}", ex, ex.GetBaseException(), ex.GetType(), ex.Message, ex.StackTrace);

        //}

    }

    public void WriteSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();

        //byte[] byteArray = Encoding.Default.GetBytes(line);
        //tcp_socket.Send(byteArray);

    }




    public string ReadSocket()
    {
        if (!socket_ready)
            return "";
        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();
        Debug.Log("no data found...");
        return "";


        //byte[] result = new byte[1024];
        //int receiveLength = tcp_socket.Receive(result);
        //string result_string = Encoding.ASCII.GetString(result, 0, receiveLength);
        //return result_string;
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }


    public void StartGameButton()
    {
        InputField[] inputFields = startCanvas.GetComponentsInChildren<InputField>();
        string playerName = inputFields[0].text;
        string password = inputFields[1].text;

        LoginInfo loginInfo = new LoginInfo();
        loginInfo.playerName = playerName;
        loginInfo.password = password;
        string loginJsonStr = JsonConvert.SerializeObject(loginInfo);
        Debug.Log(loginJsonStr);
        WriteSocket(loginJsonStr);
        byte[] result = new byte[1024]; 
        string returnPlayerInfo = ReadSocket();
        Debug.LogFormat("returnPlayerInfo:{0}", returnPlayerInfo);

        RootObject rb = JsonConvert.DeserializeObject<RootObject>(returnPlayerInfo);
        Vector3 bornPos = GameObject.Find("bornPosition").transform.position;
        Quaternion bornRot = Quaternion.Euler(Vector3.zero);
        GameObject player = Instantiate(playerPrefeb, bornPos, bornRot) as GameObject;
        Debug.LogFormat("name:{0}, password:{1}, isLocal:{2}, health:{3}",
            playerName, password, true, rb.playerState.health);
        
        #region update player state
        player.GetComponent<CharacterStates>().playerName = playerName;
        player.GetComponent<CharacterStates>().password = password;
        player.GetComponent<CharacterStates>().isLocal = true;
        player.GetComponent<CharacterStates>().health = rb.playerState.health;
        //player.GetComponent<CharacterStates>().money = 

        List<WeaponInfo> weaponInfos = rb.weaponInfos;
        Container container = player.GetComponentInChildren<Container>();
        List<Weapon> curWeaponList = player.GetComponent<WeaponHandler>().weaponList;
        Weapon[] weaponsInWeaponContainer = player.GetComponent<WeaponHandler>()
            .userSettings.weaponContainer.GetComponentsInChildren<Weapon>();
        foreach(Weapon weapon in weaponsInWeaponContainer)
        {
            foreach(WeaponInfo weaponInfo in weaponInfos)
            {
                if(weapon.weaponSettings.weaponName == weaponInfo.gunName.ToLower())
                {
                    curWeaponList.Add(weapon);
                    weapon.ammo.clipAmmo = weaponInfo.clipAmmos;
                    if(weaponInfo.clipLeft > container.GetAmountRemaining(weapon.ammo.AmmoID))
                    {
                        container.Put(weapon.ammo.AmmoID, weaponInfo.clipLeft - container.GetAmountRemaining(weapon.ammo.AmmoID));

                    }
                    else
                    {
                        container.TakeFromContainer(weapon.ammo.AmmoID, 
                            container.GetAmountRemaining(weapon.ammo.AmmoID) - weaponInfo.clipLeft);

                    }
                }
                else
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }
        player.GetComponent<WeaponHandler>().currentWeapon = curWeaponList[0];
        #endregion
        player.GetComponent<WeaponHandler>().currentWeapon.SetEquipped(true);
        gameStart = true;
        startCanvas.enabled = false;

    }


    IEnumerator StopWhile()
    {
        yield return new WaitForSeconds(1f);

    }
}