from socket import *
from time import ctime
import os
#import message_pb2
import json


def init_Env():
    if not os.path.exists("Users"):
        os.makedirs("Users")


def init_Net():
    HOST = '127.0.0.1'
    PORT = 21567
    BUFSIZ = 4096
    ADDR = (HOST, PORT)

    print("Serer Information:")
    print("IP:127.0.0.1")
    print("Port:21567")

    tcpSerSock0 = socket(AF_INET, SOCK_STREAM)
    tcpSerSock0.bind(ADDR)
    tcpSerSock0.listen(5)
    return tcpSerSock0


def deal_Register(user_info):
    print("Dealing the client register now...")
    temp_str1 = json.loads(user_info)
    # Register code
    user_files = os.listdir("Users")
    if temp_str1["playerName"] in user_files:
        return -1  # Means the user has been registered.
    else:
        user_file = "Users\\"+temp_str1["playerName"]
        f = open(user_file, "w")
        f.write(json.dumps(temp_str1))
        f.close()
        return 1


def deal_Storage(user_info):
    print("Dealing the client storage now...")
    temp_str2 = json.loads(user_info)
    # Storage code
    if temp_str2["playerName"] not in os.listdir("Users"):
        return -2  # Means the user not in the server dataBase.
    else:
        user_file = "Users\\"+temp_str2["playerNmae"]
        f = open(user_file, "w")
        f.write(json.dumps(temp_str2))
        f.close()
        return 2


def deal_Login(user_info):
    print("Dealing the client login now...")
    temp_str3 = json.loads(user_info)
    if temp_str3["playerName"] not in os.listdir("Users"):
        return -3
    else:
        # Here should return the user data to the client.
        return 3


init_Env()
tcpSerSock = init_Net()

while True:
    print("Waiting for connection...")
    tcpCliSock, addr = tcpSerSock.accept()
    print("...Connected from:", addr)

    print("Receive the message from the client:")
    json_str = tcpCliSock.recv(1024)
    text = json.loads(json_str)

    if(text["signal"] == '0'):  # Register message.
        print("User register process...")
        str1 = '0'
        result = deal_Register(json_str)
        if(result == -1):
            print("Register failed, the database has already have the user info.")
            server_msg = "-1"
            send_bytes = server_msg.encode('utf-8')
            tcpCliSock.send(send_bytes)
        elif(result == 1):
            server_msg = "1"
            print("Register successful!")
            send_bytes = server_msg.encode('utf-8')
            tcpCliSock.send(send_bytes)
    elif(text["signal"] == '1'):  # Storage message.
        print("User storage process...")
        str1 = '1'
        result = deal_Storage(json_str)
        if(result == -2):
            print("Storage failed, the databas doesn't have the user information.")
            server_msg = "-2"
            tcpCliSock.send(server_msg.encode('utf-8'))
        elif(result == 2):
            print("Storage successful!")
            server_msg = "2"
            tcpCliSock.send(server_msg.encode('utf-8'))
    elif(text["signal"] == "2"):  # Login message
        print("User login process...")
        str1 = "2"
        result = deal_Login(json_str)
        if(result == -3):
            print("Login failed, check the user name and password.")
            server_msg = "-3"
            tcpCliSock.send(server_msg.encode('utf-8'))
        elif(result == 3):
            print("Login successful!")
            # server_msg=="3"
            data_file = "Users\\"+text["playerName"]
            f_read = open(data_file, "r")
            server_msg = f_read.read()
            tcpCliSock.send(server_msg.encode('utf-8'))

    tcpCliSock.close()

tcpSerSock.close()
