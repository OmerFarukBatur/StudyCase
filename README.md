🏖️ İzin Talep ve Onay Uygulaması

Bu proje, küçük ölçekli bir şirketin çalışan izin taleplerini yönetmek amacıyla geliştirilmiş temel bir web uygulamasıdır.
Çalışanlar izin talebi oluşturabilir, yöneticiler bu talepleri onaylayabilir veya reddedebilir.
Uygulama, rol bazlı yetkilendirme, doğrulama kuralları, audit log ve basit raporlama özellikleri içerir.

🚀 Amaç

Küçük bir şirket için temel İzin Talep / Onay sistemini geliştirmek.
Projede aşağıdaki özellikler bulunmaktadır:

Çalışanlar izin talebi oluşturabilir, düzenleyebilir veya silebilir.

Yöneticiler talepleri onaylayabilir veya reddedebilir.

Listeleme, filtreleme, basit raporlama ve rol bazlı menü görünümü vardır.

Tüm işlemler audit log yapısı ile izlenmektedir.

🧩 Kullanılan Teknolojiler
Katman	Teknoloji
Backend	.NET 7/8 – ASP.NET Core MVC (Razor Views, Controller/Action)
ORM	Entity Framework Core (Code First veya mevcut SQL şeması)
Veritabanı	SQL Server (DDL + seed script dahil)
Frontend	Razor + minimal JS (fetch veya jQuery)
Kimlik Doğrulama	Cookie Authentication (Employee / Manager)
Loglama	Serilog (opsiyonel)
Validasyon	FluentValidation (opsiyonel)
🔐 Kimlik & Rol Yönetimi

Login Ekranı:
Kullanıcı girişi sonrası cookie auth ile oturum açılır.

Rol Bazlı Menü:

Employee: Kendi izin taleplerini yönetir.

Manager: Bekleyen izinleri onaylar veya reddeder.

🧾 İzin Talebi (Employee)

Talep Alanları:

StartDate

EndDate

LeaveType → (Annual, Sick, Unpaid)

Reason

Doğrulama Kuralları:

StartDate ≤ EndDate

Geçmiş tarihe izin: yalnızca bugünden en fazla 7 gün geriye kadar.

Çakışan (PENDING veya APPROVED) izin varsa kayıt engellenir.

İşlevler:

Kendi taleplerini listeleme

Duruma göre filtreleme (PENDING, APPROVED, REJECTED)

PENDING durumundaki talepleri güncelleme veya silme

👨‍💼 Onay Süreci (Manager)

Yöneticiler için özellikler:

Bekleyen izin taleplerini listeleme

Tarih, çalışan, izin tipi filtreleri

Sayfalama desteği

Talepleri Approve / Reject etme

Reject işlemi açıklama gerektirir

Concurrency (eş zamanlı işlem) kontrolü için RowVersion alanı kullanılır

Onay / ret işlemleri transactional olarak yürütülür

📊 Raporlama
🗓️ Aylık İzin Özeti

Ay / Çalışan bazında toplam onaylı izin günleri

CSV export desteği

📈 Dashboard Kartları

Bu ay onaylanan izin sayısı

En çok kullanılan izin türü

Bekleyen talep sayısı

🧾 Audit Log

Her talep için aşağıdaki bilgiler tutulur:

CreatedBy, CreatedAt

UpdatedBy, UpdatedAt

Onay ve ret işlemleri ayrı bir Approvals tablosunda saklanır.

🗂 Proje Yapısı
StudyCase/
├── Core/
│   ├── Entities/           # Domain modelleri
│   ├── Enums/             # Enum tanımları
│   └── Interfaces/        # Repository interface'leri
├── Infrastructure/
│   ├── Data/              # DbContext ve konfigürasyon
│   ├── Repositories/      # Repository implementasyonları
│   └── Migrations/        # EF Core migrations
├── Application/
│   ├── DTOs/              # Data Transfer Objects
│   ├── Services/          # Business logic
│   └── Validators/        # FluentValidation kuralları
├── AlturStudyCase(Web/MVC)/
    ├── Controllers/       # MVC Controller'lar
    ├── Views/             # Razor view'lar
    ├── ViewModels/        # View modelleri
    └── wwwroot/           # Static dosyalar
