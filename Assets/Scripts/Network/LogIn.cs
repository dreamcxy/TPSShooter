using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
public class LogIn: MonoBehaviour
{
    public string SendText(string text){
        
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try{
            clientSocket.Connect(new IPEndPoint(ip , 21567));
            Debug.Log("连接服务器成功....");
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(text);
            clientSocket.Send(byteArray);
        }catch(Exception ex){
            Debug.LogFormat("Exception:\n{0}\n BaseException:\n{1} \n GetType:\n{2} \nMessage:\n{3}\n StackTrace:\n{4}", ex, ex.GetBaseException(), ex.GetType(), ex.Message, ex.StackTrace);
            return null;
        }
        byte[] result = new byte[1024];
        int receiveLength = clientSocket.Receive(result);
        string result_string = Encoding.ASCII.GetString(result, 0, receiveLength);
        // Debug.LogFormat("接收服务器消息：{0}", result_string);
        switch(result_string){
             case "-1":
                    Console.WriteLine("注册失败，服务器中已经有该用户名数据。");
                    break;
                case "1":
                    Console.WriteLine("注册成功，欢迎进入游戏。");
                    break;
                case "-2":
                    Console.WriteLine("存储失败，服务器中没有该用户的注册信息。");
                    break;
                case "2":
                    Console.WriteLine("存储成功，欢迎下次继续游戏。");
                    break;
                case "-3":
                    Console.WriteLine("登录失败，请检查你的用户名和密码。");
                    break;
                default:
                    Console.WriteLine("登录成功，欢迎进入游戏");
                    break;
        }
        clientSocket.Close();
        return result_string;
    }
}
