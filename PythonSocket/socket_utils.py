import os


def receive_string(conn):
    # get the expected length (eight bytes long, always)
    expected_size = b""
    while len(expected_size) < 8:
        more_size = conn.recv(8 - len(expected_size))
        if not more_size:
            raise Exception("Short file length received")
        expected_size += more_size

    # convert to int, the expected file length
    expected_size = int.from_bytes(expected_size, 'big')
    print(f"expected_size={expected_size}")

    # until we've received the expected amount of data, keep receiving
    packet = ""
    while len(packet) < expected_size:
        buffer = conn.recv(expected_size - len(packet)).decode()
        if not buffer:
            raise Exception("Incomplete file received")
        packet += buffer
    return packet

def send_string(conn, string):
    remaining_string = string
    conn.sendall(len(remaining_string).to_bytes(8,'big'))
    while remaining_string:
        sent_bytes = conn.send(remaining_string.encode())  # send the chunk
        remaining_string = remaining_string[sent_bytes:]

def receive_file(conn, filename):
    # get the expected length (eight bytes long, always)
    expected_size = b""
    while len(expected_size) < 8:
        more_size = conn.recv(8 - len(expected_size))
        if not more_size:
            raise Exception("Short file length received")
        expected_size += more_size

    # convert to int, the expected file length
    expected_size = int.from_bytes(expected_size, 'big')
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
        # print(f"Received {len(packet)}/{expected_size}")
    with open(filename, 'wb') as file:
        file.write(packet)
    print(f"Received file")


def send_file(conn, file_path, buffer_size=8):
    file_size_bytes = os.stat(file_path).st_size
    conn.sendall(file_size_bytes.to_bytes(8,'big'))
    with open(file_path, 'rb') as file:  # open our file to read
        while True:
            chunk = file.read(buffer_size)  # get next chunk
            if not chunk:  # empty chunk indicates EOF
                break
            conn.sendall(chunk)  # send the chunk
