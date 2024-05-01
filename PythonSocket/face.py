# STEP 1: Import the necessary modules.
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

def detect_face(image_path):
    # STEP 2: Create an FaceDetector object.
    base_options = python.BaseOptions(model_asset_path='assets/detector.tflite')
    options = vision.FaceDetectorOptions(base_options=base_options)
    detector = vision.FaceDetector.create_from_options(options)

    # STEP 3: Load the input image.
    image = mp.Image.create_from_file(image_path)

    # STEP 4: Detect faces in the input image.
    detection_result = detector.detect(image)

    # return true if a face is detected, false otherwise
    return len(detection_result.detections) > 0
