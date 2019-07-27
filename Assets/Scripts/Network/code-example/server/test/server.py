# coding = utf-8
import sys


sys.path.append('./network')

import simpleServer


def main():
    server = simpleServer.SimpleServer()
    print server

if __name__ == '__main__':
    main()
