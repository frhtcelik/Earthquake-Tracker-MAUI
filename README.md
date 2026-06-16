# 🌍 Earthquake Tracker & SOS Alert System (.NET MAUI)

[![.NET MAUI](https://img.shields.io/badge/.NET-MAUI-512BD4?logo=dotnet&logoColor=white)](#)
[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](#)
[![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white)](#)
[![Android](https://img.shields.io/badge/Android-Native_API-3DDC84?logo=android&logoColor=white)](#)

*Scroll down for the Turkish version.* / *Türkçe açıklamalar için aşağı kaydırın.*

## 🇬🇧 English

### Overview
This project is a cross-platform mobile application developed with **.NET MAUI**, designed to provide real-time seismic activity tracking and a critical life-saving **SOS Emergency Module**. Unlike standard applications that merely list earthquakes, this system bypasses operating system UI limitations to send background SMS alerts with precise GPS coordinates directly via the Android baseband during an emergency.

### Key Features
*   **Real-Time Seismic Data:** Asynchronously consumes RESTful APIs (AFAD/Kandilli) to list recent earthquakes.
*   **Native Background SMS (SOS):** Utilizes `Android.Telephony.SmsManager` to bypass standard SMS UI, allowing users to send instant SOS messages with a single tap during panic situations.
*   **Fault-Tolerant Geolocation:** Integrates `GetLastKnownLocationAsync` as a fallback mechanism. If GPS signal is lost (e.g., under rubble), it retrieves the cached location to generate a Google Maps link.
*   **Heatmap & Data Visualization:** Groups seismic data to display regional risk zones and activity density on a map.
*   **Local Storage:** Uses **SQLite** to securely store emergency contacts and user preferences (minimum magnitude, followed cities) directly on the device, ensuring offline availability.
*   **MVVM Architecture:** Built with a strict Model-View-ViewModel pattern utilizing Data Binding for optimal memory management and UI responsiveness.

### Technologies Used
*   **Framework:** .NET 9.0 MAUI
*   **Language:** C#, XAML
*   **Local Database:** SQLite-net-pcl
*   **Data Format:** JSON (Newtonsoft.Json)
*   **Native Integration:** Android Manifest Permissions, Location Services, SMS Manager API

---

## 🇹🇷 Türkçe

### Proje Özeti
Bu proje, **.NET MAUI** ile geliştirilmiş çapraz platform bir mobil uygulamadır. Geleneksel deprem listeleme uygulamalarından farklı olarak, anlık sismik veri takibi yapmanın yanı sıra, afet anında hayat kurtarmak üzere tasarlanmış **Native bir Acil Durum (SOS) Modülü** barındırır. Sistem, modern işletim sistemi kısıtlamalarını aşarak, kullanıcı paniği anında hiçbir mesajlaşma arayüzü açmadan doğrudan Android çekirdeği üzerinden arka planda SMS ve GPS koordinatı gönderebilmektedir.

### Temel Özellikler
*   **Gerçek Zamanlı Sismik Veri:** AFAD ve Kandilli Rasathanesi RESTful API'lerini asenkron olarak tüketir ve listeler.
*   **Arka Plan SMS İletimi (Native SOS):** Standart mesajlaşma ekranlarını atlamak için `Android.Telephony.SmsManager` sınıfını kullanır. Kullanıcı tek bir butona basarak hücresel ağ üzerinden anında yardım çağrısı başlatabilir.
*   **Hata Toleranslı Konum (Fallback):** Enkaz altı gibi canlı uydu bağlantısının koptuğu senaryolar için cihaz önbelleğindeki son konumu (`GetLastKnownLocationAsync`) çekerek Google Haritalar linki oluşturur. Boş mesaj gönderimini engeller.
*   **Isı Haritası (Heatmap) ve Analiz:** Sismik verileri kümeleyerek bölgesel risk yoğunluğunu görsel bir harita üzerinde kullanıcıya sunar.
*   **Yerel Veritabanı:** Çevrimdışı durumlarda dahi çalışabilmesi için acil durum kişileri ve şiddet filtreleri **SQLite** ile şifrelenmiş cihaz hafızasında tutulur.
*   **MVVM Mimarisi:** Model-View-ViewModel tasarım deseni ve "Data Binding" kullanılarak yüksek performanslı ve sürdürülebilir bir kod yapısı oluşturulmuştur.

### Kullanılan Teknolojiler
*   **Altyapı:** .NET 9.0 MAUI
*   **Dil:** C#, XAML
*   **Yerel Veritabanı:** SQLite-net-pcl
*   **Veri Formatı:** JSON (Newtonsoft.Json)
*   **Donanım Entegrasyonu:** Android Manifest İzinleri, Konum Servisleri, Çekirdek SMS Yönetimi

---
*Developed as a Computer Engineering Graduation Project.*
