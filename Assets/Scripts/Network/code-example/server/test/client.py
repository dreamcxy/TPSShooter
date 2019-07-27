# coding =utf-8
import socket


def main():
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    address = ("localhost", 50000)
    client.connect(address)

    pass


if __name__ == '__main__':
    main()
