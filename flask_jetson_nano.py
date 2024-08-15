from flask import Flask, request, jsonify
import cv2
import pytesseract
import re
import requests
import threading
import time

app = Flask(__name__)
A_SERVER_URL = 'https://localhost:44381/'       #  'http://IP:8088/WebMVC_OCR' 
camera_active = False
camera_thread = None

roi = None

def select_roi(image):
    global roi
    r = cv2.selectROI("Select ROI", image, fromCenter=False, showCrosshair=True)
    roi = (int(r[0]), int(r[1]), int(r[0] + r[2]), int(r[1] + r[3]))
    print("Select ROI:",roi)
    cv2.destroyWindow("Select ROI")

def preprocess_image(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    _, thresh = cv2.threshold(gray, 150, 255, cv2.THRESH_BINARY_INV)
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
    dilated = cv2.dilate(thresh, kernel, iterations=1)
    return dilated

def math_identify(image):
    pytesseract.pytesseract.tesseract_cmd = r"C:\\Program Files\\Tesseract-OCR\\tesseract.exe" #在 jetson nano 記得註解
    dilated = preprocess_image(image)
    if roi:
        roi_image = dilated[roi[1]:roi[3], roi[0]:roi[2]]
    else:
        roi_image = dilated
    custom_config = r'--oem 3 --psm 6 outputbase digits'
    text = pytesseract.image_to_string(roi_image, config=custom_config).strip()
    res_text = re.findall(r'\d+', text)
    return res_text

def recognize_text_from_camera():
    global camera_active
    cap = cv2.VideoCapture(0)
    ret, frame = cap.read()
    if ret:
        select_roi(frame)  
    while camera_active and cap.isOpened():
        cv2.imshow("Webcam", frame)
        if cv2.waitKey(1) == 27: #Esc leave
            break
        ret, frame = cap.read()
        if not ret:
            break
        digits = math_identify(frame)  # <class 'list'>
        if len(digits) > 0:
            data = {'digits': digits, 'datetime': time.strftime("%Y-%m-%d %H:%M:%S", time.localtime())}
            response = requests.post(f'{A_SERVER_URL}/Camera/ReceiveDigit', json=data, verify=False)
            print(f"Response from server: {response.status_code} - {response.text}")
    cap.release()
    cv2.destroyAllWindows()

@app.route('/start_recognition', methods=['POST'])
def start_recognition():
    global camera_active, camera_thread
    camera_active = True
    camera_thread = threading.Thread(target=recognize_text_from_camera)
    camera_thread.start()
    return jsonify({'success': True})

@app.route('/stop_recognition', methods=['POST'])
def stop_recognition():
    global camera_active, camera_thread
    camera_active = False
    if camera_thread is not None:
        camera_thread.join()
    return jsonify({'success': True})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
