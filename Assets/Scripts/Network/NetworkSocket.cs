using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;



public class NetworkSocket : MonoBehaviour
{
    public String host = "127.0.0.1";
    public Int32 port = 2000;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;



    void Update()
    {
        string received_data = ReadSocket();
        WriteSocket(100, 1, "world");


        if (received_data != "")
        {
            // Do something with the received data,
            // print it in the log for now
            Debug.Log(received_data);
        }
    }


    void Awake()
    {
        SetupSocket();
    }

    void OnApplicationQuit()
    {
        closeSocket();
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

    //public void WriteSocket(string line)
    //{
    //    if (!socket_ready)
    //        return;

    //    line = line + "\r\n";
    //    socket_writer.Write(line);
    //    socket_writer.Flush();
    //}

   
    public void WriteSocket(ushort sid, ushort cid, string data)
    {
        if (!socket_ready) return;
        socket_writer.BaseStream.Write(Pickle.Serialize((uint)(Conf.NET_HEAD_LENGTH_SIZE * 3 + data.Length)), 0, 4);
        socket_writer.BaseStream.Write(Pickle.Serialize(sid), 0, 2);
        socket_writer.BaseStream.Write(Pickle.Serialize(cid), 0, 2);
        socket_writer.BaseStream.Write(Pickle.Serialize(data.Length), 0, 4);
        socket_writer.Write(data);
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