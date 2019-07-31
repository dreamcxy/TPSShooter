#coding=utf-8
# from collections import defaultdict
# from contextlib import contextmanager


# class Exchange(object):
#     def __init__(self):
#         self._subscribers = set()

#     def attach(self, task):
#         self._subscribers.add(task)

#     def detach(self, task):
#         self._subscribers.remove(task)

#     def send(self, message):
#         for subscriber in self._subscribers:
#             subscriber.send(message)

#     @contextmanager
#     def subscribe(self, *tasks):
#         for task in tasks:
#             self.attach(task)
#         try:
#             yield
#         finally:
#             for task in tasks:
#                 self.detach(task)


# _exchanges = defaultdict(Exchange)


# def get_exchange(name):
#     return _exchanges[name]


# class Task(object):
#     def send(self, message):
        
#         print(message)


# task1 = Task()
# task2 = Task()



# exchage = get_exchange("message")
# exchage.attach(task1)
# exchage.attach(task2)

# exchage = get_exchange("message")
# exchage.send("hello")


# exchage.detach(task1)
# exchage.detach(task2)


# exchage = get_exchange("message")
# with exchage.subscribe(task1, task2):
#     exchage.send("hello world")
#
# import Exchange
#
# publisher = Exchange.Publisher()
# roomPublisher = Exchange.RoomPublisher()
#
# roomPublisher.addObserver("123234")
# roomPublisher.notifyObserver()

# import Queue
# a = {
#     "1": Queue.Queue()
# }
# a["1"].put("jell")
# b = Queue.Queue()
# b.put("h")
# b.put("ee")
# while not b.empty():
#     a["1"].put(b.get())
# while not a["1"].empty():
#
#     print a["1"].get()

import socket
sock = socket.socket(socket.AF_INET,socket.SOCK_STREAM) # 生成socket对象
sock.bind(('localhost', 2000))                          #绑定主机ip和端口号
sock.listen(5)
while True:
    connection,addr = sock.accept()                     #接受客户端的连接
    try:
        connection.settimeout(5)
        buf = connection.recv(1024)
        print buf
        connection.send("hello")
        # if buf == 1:
        #     connection.send('welcome to server!')      #向客户端发送一个字符串信息
        # else:
        #     connection.send("Failed")
    except socket.timeout:                             #如果出现超时
        print 'time out'
    connection.close()
