import cv2
import sys

def StartDetection():    
    cascPath = ".\FaceDetection\haarcascade_frontalface_alt.xml"  
    faceCascade = cv2.CascadeClassifier(cascPath)
    video_capture = cv2.VideoCapture(0)

    while (1):
        ret, frame = video_capture.read()
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        faces = faceCascade.detectMultiScale(gray, 1.3, 5)

        #Colocando o retangulo
        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 2)
        #Mostrando o video
        cv2.imshow('Video', frame)
        #Aperatando q para sair
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

