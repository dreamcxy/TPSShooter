using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;

public class NetWorkProcess : MonoBehaviour
{
    
    public String host = "127.0.0.1";
    public Int32 port = 2000;

    internal Boolean socket_ready = false;
    
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;


    ConcurrentQueue<Message> sendQueue = new ConcurrentQueue<Message>();
    ConcurrentQueue<string> recvQueue = new ConcurrentQueue<string>();

    

    GameObject localPlayer;
    GameObject[] enemies;

    
    bool gameOver;

    void Awake()
    {
        SetupSocket();
    }
    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        enemies = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<CharacterStates>().isLocal == true) {
                localPlayer = player;
    
                break;
            }
        }
    
    }



    void Update()
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
                    if(sendContent.tag == "Player")
                    {
                        string sendJsonStr = JsonConvert.SerializeObject(sendContent);
                        Debug.LogFormat("Send:{0}", sendJsonStr);
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
                    AvatarStates avatarStates = JsonConvert.DeserializeObject<AvatarStates>(recvStr);
                    if(avatarStates.tag == "Player")
                    {
                        Debug.LogFormat("Recv Player{0}:{1}, {2}, {3}",avatarStates.playerID, avatarStates.position.x,
                            avatarStates.position.y, avatarStates.position.z);
                    }else if(avatarStates.tag == "Ragdoll")
                    {
                        //Debug.LogFormat("Recv Enemy:{0}, {1}, {2}", avatarStates.position.x,
                            //avatarStates.position.y, avatarStates.position.z);
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
        yield return new WaitForSeconds(Conf.CLICK_TIME);
        AvatarStates localPlayerStates = new AvatarStates();
        localPlayerStates.health = localPlayer.GetComponent<CharacterStates>().health;
        localPlayerStates.forward = localPlayer.GetComponent<Animator>().GetFloat("Forward") * Conf.MULTIPLE;
        localPlayerStates.strafe = localPlayer.GetComponent<Animator>().GetFloat("Strafe") * Conf.MULTIPLE;
        Debug.LogFormat("forward:{0}, strafe:{1}", localPlayerStates.forward, localPlayerStates.strafe);
        localPlayerStates.isRun = localPlayer.GetComponent<CharacterMovement>().isRun;
        localPlayerStates.isFire = localPlayer.GetComponent<WeaponHandler>().shootSingle;
        localPlayerStates.isReload = localPlayer.GetComponent<WeaponHandler>().reload;
        localPlayerStates.isInTank = localPlayer.GetComponent<CharacterStates>().isInTank;
        localPlayerStates.isLocal = localPlayer.GetComponent<CharacterStates>().isLocal;
        localPlayerStates.playerID = localPlayer.GetComponent<CharacterStates>().ID;
        localPlayerStates.tag = localPlayer.tag;
        localPlayerStates.position = new Position(localPlayer.transform.position.x,
            localPlayer.transform.position.y, localPlayer.transform.position.z);

        localPlayerStates.rotation = new Rotaion(localPlayer.transform.rotation.x,
            localPlayer.transform.rotation.y, localPlayer.transform.rotation.z);

        sendQueue.Enqueue(localPlayerStates);

        foreach (var enemy in enemies)
        {
            AvatarStates enemyStates = new AvatarStates();
            enemyStates.health = enemy.GetComponent<EnemyStates>().health;
            enemyStates.forward = enemy.GetComponent<Animator>().GetFloat("Forward") * Conf.MULTIPLE;
            enemyStates.isFire = enemy.GetComponent<Animator>().GetBool("Attack");
            enemyStates.tag = enemy.tag;
            enemyStates.position = new Position(enemy.transform.position.x,
                enemy.transform.position.y, enemy.transform.position.z);
            enemyStates.rotation = new Rotaion(enemy.transform.rotation.x,
                enemy.transform.rotation.y, enemy.transform.position.z);
            sendQueue.Enqueue(enemyStates);
        }

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
    }

    public void WriteSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }




    public String ReadSocket()
    {
        if (!socket_ready)
            return "";

        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();

        return "";
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



}