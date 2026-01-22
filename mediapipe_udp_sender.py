#!/usr/bin/env python3
"""
MediaPipe Holistic visual demo + UDP sender for head (nose) AND hand position.

- Visual styling follows the original notebook (face/hands/pose DrawingSpec).
- Sends messages "nx,ny,hx,hy,fist,t" via UDP to (host,port).
- Default: host=127.0.0.1, port=5005, show window.

Usage:
  pip install mediapipe opencv-python
  python mediapipe_holistic_udp.py --host 127.0.0.1 --port 5005 --fps 60
"""
import argparse
import socket
import time
import sys
import os
import math # --- TAMBAHAN MODUL MATH ---

# reduce TF/TFLite verbose logs (optional)
os.environ.setdefault('TF_CPP_MIN_LOG_LEVEL', '2')

try:
    import cv2
    import mediapipe as mp
except Exception as e:
    print("Missing modules:", e)
    print("Install with: pip install mediapipe opencv-python")
    sys.exit(1)


def get_face_connections(mp_holistic):
    """Return face connections compatible with draw_landmarks if available."""
    try:
        return mp_holistic.FACE_CONNECTIONS
    except Exception:
        pass
    try:
        return mp.solutions.face_mesh.FACEMESH_TESSELATION
    except Exception:
        pass
    try:
        return mp.solutions.face_mesh.FACEMESH_CONTOURS
    except Exception:
        pass
    return None

# --- TAMBAHAN FUNGSI DETEKSI KEPALAN TANGAN ---
def is_fist(landmarks):
    if not landmarks: return False
    wrist = landmarks.landmark[0]
    tips = [8, 12, 16, 20]
    pips = [6, 10, 14, 18]
    folded_count = 0
    for i in range(4):
        # Bandingkan jarak ujung jari ke pergelangan vs sendi tengah ke pergelangan
        tip_dist = math.sqrt((landmarks.landmark[tips[i]].x - wrist.x)**2 + (landmarks.landmark[tips[i]].y - wrist.y)**2)
        pip_dist = math.sqrt((landmarks.landmark[pips[i]].x - wrist.x)**2 + (landmarks.landmark[pips[i]].y - wrist.y)**2)
        if tip_dist < pip_dist:
            folded_count += 1
    return folded_count >= 3
# ----------------------------------------------

def main(host, port, camera, mirror, target_fps, show_window):
    # UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    addr = (host, port)

    mp_drawing = mp.solutions.drawing_utils
    mp_holistic = mp.solutions.holistic

    FACE_CONNECTIONS = get_face_connections(mp_holistic)

    # Drawing specs taken to match the notebook's styling (ORIGINAL STYLING)
    face_conn_spec = mp_drawing.DrawingSpec(color=(80, 110, 10), thickness=1, circle_radius=1)
    face_land_spec = mp_drawing.DrawingSpec(color=(80, 256, 121), thickness=1, circle_radius=1)

    right_conn_spec = mp_drawing.DrawingSpec(color=(80, 22, 10), thickness=2, circle_radius=4)
    right_land_spec = mp_drawing.DrawingSpec(color=(80, 44, 121), thickness=2, circle_radius=2)

    left_conn_spec = mp_drawing.DrawingSpec(color=(121, 22, 76), thickness=2, circle_radius=4)
    left_land_spec = mp_drawing.DrawingSpec(color=(121, 44, 250), thickness=2, circle_radius=2)

    pose_conn_spec = mp_drawing.DrawingSpec(color=(245, 117, 66), thickness=2, circle_radius=4)
    pose_land_spec = mp_drawing.DrawingSpec(color=(245, 66, 230), thickness=2, circle_radius=2)

    cap = cv2.VideoCapture(camera)
    if not cap.isOpened():
        print(f"Cannot open camera {camera}")
        return

    frame_interval = 1.0 / max(1, target_fps)
    last_send = 0.0
    
    print(f"--- MEDIA PIPE SENDER STARTED ---")
    print(f"Target UDP: {host}:{port}")
    print("Lihat Window Kamera: Lingkaran UNGU = Posisi Cursor")

    # Use the holistic model
    with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5) as holistic:
        try:
            while True:
                t0 = time.time()
                ret, frame = cap.read()
                if not ret:
                    print("Failed to read frame")
                    break

                if mirror:
                    frame = cv2.flip(frame, 1)

                # Optional resize for performance:
                # frame = cv2.resize(frame, (640, 480))

                image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
                image_rgb.flags.writeable = False
                results = holistic.process(image_rgb)
                image_rgb.flags.writeable = True

                # Prepare image for display (BGR)
                image = cv2.cvtColor(image_rgb, cv2.COLOR_RGB2BGR)

                # --- 1. GAMBAR WAJAH (LOGIKA LAMA) ---
                if results.face_landmarks:
                    try:
                        if FACE_CONNECTIONS is not None:
                            mp_drawing.draw_landmarks(
                                image, results.face_landmarks, FACE_CONNECTIONS,
                                face_conn_spec, face_land_spec)
                        else:
                            mp_drawing.draw_landmarks(image, results.face_landmarks)
                    except Exception:
                        mp_drawing.draw_landmarks(image, results.face_landmarks)

                # --- 2. UPDATE: PROSES TANGAN (UNTUK CURSOR) ---
                hand_x = -1.0
                hand_y = -1.0
                is_clenched = False
                
                # Cek tangan mana yang aktif (Prioritas Kanan, lalu Kiri)
                active_hand = None
                if results.right_hand_landmarks:
                    active_hand = results.right_hand_landmarks
                elif results.left_hand_landmarks:
                    active_hand = results.left_hand_landmarks

                if active_hand:
                    # Ambil Ujung Telunjuk (Index 8)
                    index_tip = active_hand.landmark[8]
                    hand_x = index_tip.x
                    hand_y = index_tip.y
                    
                    # Cek Kepalan Tangan
                    if is_fist(active_hand):
                        is_clenched = True
                    
                    # --- VISUAL DEBUGGING ON CAMERA (BARU) ---
                    # Gambar lingkaran ungu di ujung jari agar kelihatan di layar
                    h, w, c = image.shape
                    cx, cy = int(hand_x * w), int(hand_y * h)
                    cv2.circle(image, (cx, cy), 15, (255, 0, 255), cv2.FILLED)
                    
                    # Tulis Status FIST/OPEN
                    status_text = "FIST (CLICK)" if is_clenched else "OPEN"
                    col = (0, 0, 255) if is_clenched else (0, 255, 0)
                    cv2.putText(image, status_text, (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, col, 3)
                else:
                    # Beri info kalau tangan tidak terlihat
                    cv2.putText(image, "NO HAND DETECTED", (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
                    cv2.putText(image, "(Show Shoulder/Body)", (50, 150), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 255), 2)

                # -----------------------------------------------

                # Right hand drawing (Original Styling)
                if results.right_hand_landmarks:
                    mp_drawing.draw_landmarks(
                        image, results.right_hand_landmarks, mp_holistic.HAND_CONNECTIONS,
                        right_conn_spec, right_land_spec)

                # Left hand drawing (Original Styling)
                if results.left_hand_landmarks:
                    mp_drawing.draw_landmarks(
                        image, results.left_hand_landmarks, mp_holistic.HAND_CONNECTIONS,
                        left_conn_spec, left_land_spec)

                # Pose (body)
                if results.pose_landmarks:
                    mp_drawing.draw_landmarks(
                        image, results.pose_landmarks, mp_holistic.POSE_CONNECTIONS,
                        pose_conn_spec, pose_land_spec)

                # --- 3. KIRIM DATA UDP (UPDATE FORMAT 6 KOLOM) ---
                # Mengirim posisi HIDUNG (Kepala) DAN TANGAN
                now = time.time()
                # Kita kirim jika pose terdeteksi (untuk kepala) ATAU tangan terdeteksi (untuk cursor)
                # Tambahkan (now - last_send) limit agar sesuai FPS
                if (results.pose_landmarks or active_hand) and (now - last_send) >= frame_interval:
                    try:
                        # Ambil posisi hidung
                        nx, ny = 0.5, 0.5
                        if results.pose_landmarks:
                            nose_landmark = mp_holistic.PoseLandmark.NOSE
                            nose = results.pose_landmarks.landmark[nose_landmark]
                            nx, ny = float(nose.x), float(nose.y)
                        
                        fist_val = 1 if is_clenched else 0
                        
                        # FORMAT PESAN: noseX, noseY, handX, handY, fist, timestamp
                        msg = f"{nx:.4f},{ny:.4f},{hand_x:.4f},{hand_y:.4f},{fist_val},{now:.4f}"
                        
                        try:
                            sock.sendto(msg.encode('utf-8'), addr)
                        except Exception as e:
                            if int(now) % 5 == 0: print("Send error:", e)
                        
                        last_send = now
                    except Exception as e:
                        if int(now) % 5 == 0: print("Extraction error:", e)

                # Show the window
                if show_window:
                    cv2.imshow("Raw Webcam Feed", image)
                    if cv2.waitKey(1) & 0xFF == ord('q'):
                        break

                # Maintain FPS
                elapsed = time.time() - t0
                time.sleep(max(0, frame_interval - elapsed))
        finally:
            cap.release()
            if show_window:
                cv2.destroyAllWindows()
            sock.close()


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="MediaPipe Holistic + UDP head sender (visual)")
    parser.add_argument("--host", default="127.0.0.1", help="UDP destination host (Unity)")
    parser.add_argument("--port", type=int, default=5005, help="UDP destination port")
    parser.add_argument("--camera", type=int, default=0, help="Camera index")
    parser.add_argument("--fps", type=int, default=60, help="Target send rate (Hz)")
    parser.add_argument("--no-mirror", dest="mirror", action="store_false", help="Disable horizontal flip")
    parser.add_argument("--no-window", dest="show_window", action="store_false", help="Disable cv2 window (if you don't want the visual)")
    args = parser.parse_args()
    try:
        main(host=args.host, port=args.port, camera=args.camera, mirror=args.mirror, target_fps=args.fps, show_window=args.show_window)
    except KeyboardInterrupt:
        pass