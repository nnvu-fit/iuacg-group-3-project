import os
from socket import socket

byte_order = 'little'

def receive_string(conn):
    # get the expected length (eight bytes long, always)
    expected_size = b""
    while len(expected_size) < 8:
        more_size = conn.recv(8 - len(expected_size))
        if not more_size:
            raise Exception("Short file length received")
        expected_size += more_size

    # convert to int, the expected file length
    expected_size = int.from_bytes(expected_size, byte_order)
    print(f"expected_size={expected_size}")

    # until we've received the expected amount of data, keep receiving
    packet = ""
    while len(packet) < expected_size:
        buffer = conn.recv(expected_size - len(packet)).decode()
        if not buffer:
            raise Exception("Incomplete file received")
        packet += buffer
    return packet

def send_string(conn: socket, string):
    conn.send(len(string).to_bytes(4, byte_order))  # send the length of the string
    conn.sendall(string.encode())  # send the string

def receive_file(conn: socket, filename):
    # receive integer of buffer length from client 
    file_size_bytes = conn.recv(4)
    file_size = int.from_bytes(file_size_bytes, byte_order)
    print(f"Receiving file of size {file_size} bytes")

    # receive buffer from client
    buffer = conn.recv(file_size)

    # transform buffer to png file format
    packet = bytearray()
    packet.extend(buffer)

    # join current path with filename
    fullPath = os.path.join(os.getcwd(), filename)
    # check if file exists
    if os.path.exists(fullPath):
        os.remove(fullPath)
    # create folder if not exists
    os.makedirs(os.path.dirname(fullPath), exist_ok=True)
    # write the file
    with open(fullPath, 'wb') as file:
        file.write(packet)
    print(f"Received file")


def send_file(conn, file_path, buffer_size=8):
    file_size_bytes = os.stat(file_path).st_size
    conn.sendall(file_size_bytes.to_bytes(8, byte_order))
    with open(file_path, 'rb') as file:  # open our file to read
        while True:
            chunk = file.read(buffer_size)  # get next chunk
            if not chunk:  # empty chunk indicates EOF
                break
            conn.sendall(chunk)  # send the chunk
