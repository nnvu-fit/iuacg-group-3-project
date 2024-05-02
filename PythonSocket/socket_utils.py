import os
from socket import socket

byte_order = 'little'

def receive_string(conn):
    # get the expected length (eight bytes long, always)
    expected_size_byte_len = 4
    expected_size_bytes = conn.recv(expected_size_byte_len)
    expected_size = int.from_bytes(expected_size_bytes, byte_order)

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
    expected_size_byte_len = 4
    expected_size_bytes = conn.recv(expected_size_byte_len)
    expected_size = int.from_bytes(expected_size_bytes, byte_order)

    if not expected_size:
        print(f"return False")
        return False

    max_chunk_size_bytes = min(
        1048576,  # 1MB
        expected_size,
    )

    # until we've received the expected amount of data, keep receiving
    packet = b""  # use bytes, not str, to accumulate
    while len(packet) < expected_size:
        buffer = conn.recv(max_chunk_size_bytes)
        if not buffer:
            raise Exception("Incomplete file received")
        packet += buffer
    with open(filename, 'wb') as file:
        file.write(packet)
    print(f"Received file")
    return True


def send_file(conn, file_path, buffer_size=8):
    file_size_bytes = os.stat(file_path).st_size
    conn.sendall(file_size_bytes.to_bytes(4, byte_order))
    with open(file_path, 'rb') as file:  # open our file to read
        while True:
            chunk = file.read(buffer_size)  # get next chunk
            if not chunk:  # empty chunk indicates EOF
                break
            conn.sendall(chunk)  # send the chunk
