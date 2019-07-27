#coding=utf-8
# 为服务器提供测试的客户端


import socket
import time
from threading import Thread


class Client(object):
    def __init__(self, name):
        self.name = name
    
    def connect(self):
        ss = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        ss.connect(("localhost", 21567))
        time.sleep(2)
        ss.send(self.name)
        ecv_data = ss.recv(1024)
        print ecv_data
        ss.close()
    

def main():
    
    client1 = Client("hello")
    # Thread(target=client1.connect, args=client1.name)
    client1.connect()

if __name__ == '__main__':
    main()
    