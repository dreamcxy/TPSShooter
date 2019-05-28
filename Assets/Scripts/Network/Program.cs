﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;





namespace ConsoleApp1
{ 
    class Program
{
    private static byte[] result = new byte[1024];
    static void Main(string[] args)
    {
        //设定服务器IP地址 
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(new IPEndPoint(ip, 21567)); //配置服务器IP与端口 
            Console.WriteLine("连接服务器成功");
            string file_str = File.ReadAllText("msg.json"); //客户端给服务器传输的字符串赋值语句，将文件读取操作更改为赋值操作即可。
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(file_str);
            clientSocket.Send(byteArray);
        }
        catch(Exception ex)
        {
            Console.WriteLine("连接服务器失败，请按回车键退出！");
            Console.WriteLine("Exception:\n{0}\n BaseException:\n{1} \n GetType:\n{2} \nMessage:\n{3}\n StackTrace:\n{4}", ex, ex.GetBaseException(), ex.GetType(), ex.Message, ex.StackTrace);
            return;
        }
        //通过clientSocket接收数据 
        int receiveLength = clientSocket.Receive(result);
        string result_string = Encoding.ASCII.GetString(result, 0, receiveLength);
        Console.WriteLine("接收服务器消息：{0}", result_string);
            switch (result_string) {
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
                case "3":
                    Console.WriteLine("登录成功，欢迎进入游戏");
                    break;
            }
        Console.WriteLine("按下回车键退出.");
        Console.ReadLine();
        clientSocket.Close();
    }
}
}