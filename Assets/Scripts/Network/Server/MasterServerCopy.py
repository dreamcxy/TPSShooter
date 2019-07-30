# coding=utf-8
import json
import threading
from socketserver import BaseRequestHandler, ThreadingTCPServer
import os
import Exchange

# 记录每个房间里面的玩家, 每个玩家的id也应该是独一无二的
Rooms = {
    "now1": ["test1", "test3"],
    "now2": ["test2"],
}
publisher = Exchange.Publisher()
roomPublisher = Exchange.RoomPublisher()
playerPublisher = Exchange.PlayerPosUpdate()
Playes = {
    "127.0.0.1:1223": ["test1", "163"],
}


class Handler(BaseRequestHandler):
    def handle(self):
        address, pid = self.client_address
        print('{0}:{1} connected!'.format(address, pid))
        clientId = "{0}:{1}".format(address, pid)
        while True:
            roomPublisher.addObserver(clientId)
            data = self.request.recv(1024)
            if len(data) > 0:
                print('receive=', data.decode('utf-8'))
                cur_thread = threading.current_thread()
                signal = json.loads(data)["signal"]
                if signal == "search":
                    # 搜寻房间
                    playerName = json.loads(data)["playerName"]
                    threading.Thread(target=deal_search_room_thread, args=(
                        "", self.request)).start()

                elif signal == "create":
                    # 创建房间
                    roomName = json.loads(data)["roomName"]
                    playerName = json.loads(data)["playerName"]
                    password = json.loads(data)["password"]
                    threading.Thread(target=deal_create_room_thread, args=(
                        roomName, playerName, password, self.request)).start()
                    break
                elif signal == "attend":
                    # 加入房间
                    roomName = json.loads(data)["roomName"]
                    playerName = json.loads(data)["playerName"]
                    password = json.loads(data)["password"]
                    threading.Thread(target=deal_attend_room_thread, args=(
                        roomName, playerName, password, self.request)).start()

                elif signal == "query":
                    # 定时查询房间
                    roomName = json.loads(data)["roomName"]
                    threading.Thread(target=deal_query_room_thread, args=(
                        roomName, self.request)).start()

                elif signal == "sync":
                    # 更新玩家信息，并下放到所有主机上去
                    pass
                elif signal == "register":
                    pass
                elif signal == "log":
                    pass

            else:
                print('close')
                break


def deal_query_room_thread(roomName, request):
    request.sendall(json.dumps(Rooms))
    return Rooms[roomName]


def deal_attend_room_thread(roomName, playerName, password, request):
    if roomName not in Rooms.keys():
        return - 1
    else:
        Rooms[roomName].append(playerName)
        request.sendall(json.dumps(Rooms))
        return 1


def deal_create_room_thread(roomName, playerName, password, request):
    playerInRoom = list()
    if roomName in Rooms.keys():
        request.sendall()
    else:
        playerInRoom.append(playerName)
        Rooms[roomName] = playerInRoom
        request.sendall(json.dumps(Rooms))


def deal_search_room_thread(playerName, request):
    print('response=', json.dumps(Rooms))
    try:
        request.sendall(json.dumps(Rooms))
    except Exception as e:
        print e
    finally:
        print "close"


def main():
    HOST = "localhost"
    PORT = 21567
    ADDR = (HOST, PORT)
    server = ThreadingTCPServer(ADDR, Handler)
    print 'listening....'

    server.serve_forever()


if __name__ == '__main__':
    main()
