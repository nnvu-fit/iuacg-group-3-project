import socket
import json
from constants import HOST, PORT
import socket_utils as su


if __name__ == "__main__":
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as c:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST, PORT))
        print("Connected server")

        su.send_file(s, "assets/test-image-1-face.png")
        lm_str = su.receive_string(s)
        lm_json = json.loads(lm_str)
        print(f"1 face: lm_json={lm_json}")

        su.send_file(s, "assets/test-image-0-face.png")
        lm_str = su.receive_string(s)
        lm_json = json.loads(lm_str)
        print(f"0 faces: lm_json={lm_json}")
