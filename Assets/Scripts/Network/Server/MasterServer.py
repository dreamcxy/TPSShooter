# coding=utf-8

import conf
import json
import threading
import Queue
from socketserver import BaseRequestHandler, ThreadingTCPServer
from collections import defaultdict

client_sockets = defaultdict()

recv_queue = defaultdict()  # 收到的数据的字典
send_queue = defaultdict()  # 发送的数据的字典


class Handler(BaseRequestHandler):
    def handle(self):
        if client_sockets.__len__() < conf.MAX_CONNECT_CLIENTS:
            clientID = self.client_address[1]
            client_sockets[clientID] = self.request
            recv_queue[clientID] = Queue.Queue()
            client_connect = True
            while client_connect:
                try:
                    data = self.request.recv(conf.MAX_DATA_LENGTH)
                    # recv_queue放入数据
                    if len(data) > 0:
                        recv_queue[clientID].put(data)

                except BaseException as e:
                    print e


        else:
            return


def deal_recv_thread(connection):
    while connection:
        if len(recv_queue) > 0:
            for client in recv_queue.keys():
                data_queue = recv_queue[client]
                recv_queue[client] = Queue.Queue()
                # 分别处理每个客户端发送过来的消息
                while not data_queue.empty():
                    data = data_queue.get()

                    try:
                        text = json.loads(data)
                    except ValueError:
                        print 'server value error data:{0}'.format(data)
                    # print "[receive]client:{0} text:{1}".format(client, text)
                    if text["signal"] == "sync":
                        # 同步消息，发送给其他客户端
                        # print 'sync '
                        for other_client in client_sockets:
                            send_queue[other_client] = Queue.Queue()
                            if other_client != client:
                                print '[send]:{0}'.format(data)
                                send_queue[other_client].put(data)
                        pass
                    elif text["signal"] == "start":

                        pass
                    elif text["signal"] == "login":

                        pass
                    elif text["singal"] == "exit":
                        pass


def deal_send_thread(connection):
    while connection:
        if len(send_queue) > 0:
            for client in send_queue.keys():
                while not send_queue[client].empty():
                    data = send_queue[client].get()
                    client_sockets[client].sendall(data)


def main():
    connection = True
    threading.Thread(target=deal_recv_thread,
                     args=(connection,)).start()
    threading.Thread(target=deal_send_thread,
                     args=(connection,)).start()
    host = "127.0.0.1"
    port = 2000
    addr = (host, port)
    server = ThreadingTCPServer(addr, Handler)
    print 'server start listening...'
    server.serve_forever()


if __name__ == '__main__':
    main()
