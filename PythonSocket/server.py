import socket
import json
from constants import HOST, PORT
import socket_utils as su
from landmark import landmark



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

if __name__ == "__main__":
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen()
        print(f"Listening on port {PORT}")
        conn, addr = s.accept()
        with conn:
            print(f"Connected by {addr}")
            image_path = "ignored/image-saved-by-server.png"
            su.receive_file(conn, image_path)
            detected_lm = landmark(image_path)
            lm_json = _format_landmark(detected_lm)
            su.send_string(conn, json.dumps(lm_json))

            # detected_lm.face_blendshapes
            su.send_string(conn, "HELLOO  FROM SERVER")
