# 🐒 Monkey Feast

**Monkey Feast** adalah game interaktif berbasis **Unity** di mana pemain harus mengambil buah dan memasukkannya ke dalam keranjang menggunakan **hand tracking**. Sistem interaksi dalam game ini memanfaatkan **MediaPipe** untuk mendeteksi gerakan tangan pemain secara real-time sehingga pemain dapat berinteraksi langsung dengan objek di dalam permainan tanpa menggunakan controller.

Game ini dirancang sebagai media pembelajaran sekaligus eksperimen dalam penerapan **computer vision** dan **natural interaction** pada game interaktif.

---

# 🎮 Gameplay

Dalam **Monkey Feast**, pemain berperan sebagai pengumpul buah untuk memberi makan monyet. Pemain harus menggunakan tangan mereka untuk mengambil buah yang muncul di area permainan.

Alur permainan:

1. Kamera mendeteksi tangan pemain menggunakan **MediaPipe Hand Tracking**
2. Sistem menerjemahkan gerakan tangan menjadi interaksi dalam game
3. Pemain melakukan gesture **grab** untuk mengambil buah
4. Buah kemudian dimasukkan ke dalam **keranjang**
5. Setiap buah yang berhasil dimasukkan akan menambah skor pemain

Permainan menekankan **interaksi alami menggunakan tangan** tanpa perangkat tambahan.

---

# ✨ Fitur Utama

* 🖐 **Real-time Hand Tracking menggunakan MediaPipe**
* 🍎 **Fruit Grab Interaction**
* 🧺 **Object Drop ke Keranjang**
* 🎯 **Score System**
* 🌍 **3D Interactive Environment**
* 📷 **Computer Vision Based Interaction**

---

# 🛠️ Teknologi yang Digunakan

* **Unity Engine**
* **C# Programming**
* **MediaPipe Hand Tracking**
* **Computer Vision**
* **OpenCV / Python (opsional untuk integrasi MediaPipe)**

---

# 📂 Struktur Project

```
MonkeyFeast
│
├── Assets
│   ├── Scripts
│   ├── Prefabs
│   ├── Scenes
│   ├── Models
│   └── Materials
│
├── Packages
├── ProjectSettings
└── README.md
```

---

# 🚀 Cara Menjalankan Project

1. Clone repository

```bash
git clone https://github.com/halimmwn/MonkeyFeast-Mediapipe.git
```

2. Buka menggunakan **Unity Hub**

3. Gunakan versi Unity

```
Unity 2022.3 LTS
```

4. Buka scene utama

```
Assets/Scenes/MainScene
```

5. Jalankan game dengan menekan **Play**

Pastikan **kamera aktif** karena sistem menggunakan **hand tracking berbasis kamera**.

---

# 📸 Preview

Tambahkan screenshot atau GIF gameplay di sini.

<img width="1920" height="1080" alt="Screenshot 2025-12-05 135630" src="https://github.com/user-attachments/assets/0d17c3b4-5502-4625-b53b-abd611d300bb" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 135640" src="https://github.com/user-attachments/assets/6e08b097-0e78-4807-b8d5-142902310363" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 135649" src="https://github.com/user-attachments/assets/a0a83234-3ab7-49ec-9051-036ff61fc086" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 142246" src="https://github.com/user-attachments/assets/8348c909-3d59-4db8-8649-7fba09150c0c" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 140651" src="https://github.com/user-attachments/assets/928fd8f1-cd2e-4756-b69b-b7a99816b389" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 140543" src="https://github.com/user-attachments/assets/cb0a1888-abf6-4901-ab3c-75752d74761b" />
<img width="1920" height="1080" alt="Screenshot 2025-12-05 140550" src="https://github.com/user-attachments/assets/a3444ef3-1053-4e53-a8c6-a5f2c0509407" />



# 📌 Tujuan Project

Tujuan dari pengembangan **Monkey Feast** adalah:

* Mengimplementasikan **hand tracking berbasis computer vision**
* Mengembangkan **game interaktif tanpa controller**
* Mengeksplorasi integrasi **MediaPipe dengan Unity**
* Menciptakan pengalaman bermain yang lebih **natural dan immersive**

---

# 👨‍💻 Developer

* Lukman Hakim Badawi
* Ahmad Nur Fuady
* Mohammad Satria Halim Wicaksana
* Herlina Dewiyanti
---

# 📄 License

Project ini dibuat untuk **Tugas Akhir Matakuliah Piranti Muletimedia Interaktif**.
