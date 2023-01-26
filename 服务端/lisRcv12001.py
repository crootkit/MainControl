import socket
import threading

# 创建套接字
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 绑定端口
server_socket.bind(('0.0.0.0', 12001))

# 监听连接
server_socket.listen()

def handle_connection(client_socket, client_address):
    # 接收数据
    data = b''
    while True:
        chunk = client_socket.recv()
        if not chunk:
            break
        data += chunk

    # 关闭套接字
    client_socket.close()

    filename = data[-11:]
    # 写入文件
    with open(filename, 'wb') as f:
        f.write(data)

while True:
    # 接受连接
    client_socket, client_address = server_socket.accept()

    # 创建新线程来处理连接
    thread = threading.Thread(target=handle_connection, args=(client_socket, client_address))
    thread.start()

##########################################################################
'''

# 多进程的工作
import socket
from multiprocessing import Process

# 创建套接字
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 绑定端口
server_socket.bind(('0.0.0.0', 12001))

# 监听连接
server_socket.listen()

while True:
    # 接受连接
    client_socket, client_address = server_socket.accept()

    # 创建新进程来处理连接
    p = Process(target=handle_connection, args=(client_socket,))
    p.start()

import socket

def handle_connection(client_socket):
    # 接收数据
    data = b''
    while True:
        chunk = client_socket.recv(1024)
        if not chunk:
            break
        data += chunk

    # 关闭套接字
    client_socket.close()

    # 写入文件
    with open('received_file.bin', 'wb') as f:
        f.write(data)
'''