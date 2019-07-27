using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

using System.Net;

using System.Net.Sockets;

public class NetWorkProcess : MonoBehaviour
{
    String host = "localhost";
    

    public string SendText(string text){
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(new IPEndPoint(ip, 21567));
            Debug.Log("连接服务器成功....");
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(text);
            clientSocket.Send(byteArray);
        }catch(Exception ex){
            Debug.LogFormat("Exception:\n{0}\n BaseException:\n{1} \n GetType:\n{2} \nMessage:\n{3}\n StackTrace:\n{4}", ex, ex.GetBaseException(), ex.GetType(), ex.Message, ex.StackTrace);
            return null;
        }
        byte[] result = new byte[1024];
        int receiveLength = clientSocket.Receive(result);
        string resultString = Encoding.ASCII.GetString(result, 0, receiveLength);
        clientSocket.Close();
        return resultString;
    }



    
}
