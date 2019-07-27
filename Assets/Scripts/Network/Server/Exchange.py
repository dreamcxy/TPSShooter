# coding=utf-8
class Publisher(object):
    def __init__(self):
        self.observers = set()
        

    def addObserver(self, observer):
        # print "observers: "+self.observers
        if observer not in self.observers:
            self.observers.add(observer)
            print("Success to add :{}".format(observer))
            print self.observers
        else:
            print("Failed to add: {}".format(observer))
            return

    def removeOberser(self, observer):
        try:
            self.observers.remove(observer)
        except ValueError:
            print("Failed to remove: {}".format(observer))

    def notifyObserver(self):
        [o.notify(self) for o in self.observers]


# 发布敌人状态更新
class EnemyStatusUpdate(Publisher):
    def __init__(self):

        Publisher.__init__(self)

    def notifyObserver(self):
        pass

# 发布玩家状态更新


class PlayerPosUpdate(Publisher):
    def __init__(self):
        Publisher.__init__(self)

    def notifyObserver(self):
        pass

# 发布关卡要求


class LevelPublisher(Publisher):
    def __init__(self):
        Publisher.__init__(self)

    def notifyObserver(self):
        pass


class RoomPublisher(Publisher):
    def __init__(self):
        Publisher.__init__(self)
        self.observers = set()

    def notifyObserver(self):
        pass
