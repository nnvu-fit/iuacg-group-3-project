import socket
import json
from constants import HOST, PORT
import socket_utils as su
from landmark import landmark
from uuid import uuid4
import os
import logging


def _format_face_blendshape(blendshape):
    return {
        "index": blendshape.index,
        "score": blendshape.score,
        "display_name": blendshape.display_name,
        "category_name": blendshape.category_name,
    }


def _format_landmark(lm):
    result = {}
    result["face_blendshapes"] = [[_format_face_blendshape(bs) for bs in lm.face_blendshapes[0]]]
    return result


def start_server():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen()
        print(f"Listening on port {PORT}")
        conn, addr = s.accept()
        with conn:
            print(f"Connected by {addr}")
            i = 0
            while True:
                image_path = f"ignored/received-image-{i}-{str(uuid4())}.png"
                su.receive_file(conn, image_path)
                detected_lm = landmark(image_path)
                lm_json = _format_landmark(detected_lm)
                su.send_string(conn, json.dumps(lm_json))
                os.remove(image_path)  # delete temp image
                i += 1


if __name__ == "__main__":
    while True:
        try:
            start_server()
        except Exception as exc:
            logging.exception("Something went wrong")

