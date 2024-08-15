# Jetson_nano_OCR
## 這程式在做甚麼的...
使用 C# ASP.NET MVC 架網站，輸入jetson nano API 網址，按下啟動在jetson nano上鏡頭的按鈕，啟動鏡頭後會開始辨識數字，網站上會顯示辨識結果。 當按下暫停按鈕後，關閉鏡頭停止辨識，清除網站上的結果。
## 準備哪些...
* 一顆外接式鏡頭
* 一塊 [jetson nano](https://www.nvidia.com/zh-tw/autonomous-machines/embedded-systems/jetson-nano/product-development/)
* 兩個螢幕/滑鼠/鍵盤，一台電腦(Window環境)
## 學到什麼呢~
1. 使用 Python的Flask建立鏡頭數字辨識的API
2. 前後端完成後，部屬上去，**此範例僅限於內網之間的互傳**

## flask_jetson_nano.py : 數字辨識、啟動/關閉鏡頭API
* Flask
* Tesseract-OCR

### 1. start_recognition () : 啟動鏡頭 
* [threading 模組](https://blog.gtwang.org/programming/python-threading-multithreaded-programming-tutorial/) : 撰寫多執行緒的平行計算程式，利用多顆 CPU 核心加速運算。
* [OpenCV開鏡頭](https://shengyu7697.github.io/python-opencv-camera/)
* select_roi() : 選取鏡頭拍到的辨識範圍 (cv2.selectROI)
* math_identify() : 數字辨識












