# #coding = utf-8
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
