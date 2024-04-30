import socket
import json
from constants import HOST, PORT
import socket_utils as su


if __name__ == "__main__":
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST, PORT))
        su.send_file(s, "assets/test-image.png")
        print("Sent file")
        lm_str = su.receive_string(s)
        lm_json = json.loads(lm_str)
        print(f"lm_json={lm_json}")
