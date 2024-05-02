import socket
from socket import SOL_SOCKET, SO_REUSEADDR
import json
from constants import HOST, PORT, PATH_SPLIT
import socket_utils as su
from face import detect_face
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
    blendshapes = lm.face_blendshapes[0] if lm.face_blendshapes else []
    result["face_blendshapes"] = [_format_face_blendshape(bs) for bs in blendshapes]
    return result

def start_server():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    with s:
        s.bind((HOST, PORT))
        s.listen()
        print(f"Listening on port {PORT}")
        conn, addr = s.accept()
        with conn:
            print(f"Connected by {addr}")
            i = 0
            while True:
                image_path = f"ignored{PATH_SPLIT}received-image-{i}-{str(uuid4())}.png"
                received_file = su.receive_file(conn, image_path)
                if not received_file:
                    continue

                response = {"success": False}
                # detect face, if no face detected, send error message, otherwise send landmark data
                face_detected = detect_face(image_path)
                if not face_detected:
                    # send error message
                    response["error"] = "No face detected"
                else:
                    # send landmark data
                    detected_lm = landmark(image_path)
                    lm_json = _format_landmark(detected_lm)
                    response["success"] = True
                    response.update(lm_json)
                su.send_string(conn, json.dumps(response))
                os.remove(image_path)  # delete temp image
                i += 1


if __name__ == "__main__":
    while True:
        try:
            start_server()
        except Exception as exc:
            logging.exception("Something went wrong")

