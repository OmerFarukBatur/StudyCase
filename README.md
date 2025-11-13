Amaç
Küçük bir şirket için temel izin talep/ onay uygulaması geliştirin. Çalışanlar izin talebi oluşturur, yöneticiler onay/ret verir. Listeleme, filtreleme, basit rapor, rol bazlı yetkilendirme, doğrulama ve audit gereklidir.
Teknolojiler (zorunlu)
.NET 7/8 – ASP.NET Core MVC (Razor Views, Controller/Action)
Entity Framework Core (Code First veya mevcut SQL şemasına bağlanma)
SQL Server (DDL + seed script sağlanmalı)
Frontend: Razor + minimal JS (fetch veya jQuery kabul)
Kimlik: Basit cookie auth (in-memory veya DB) — 2 rol: Employee, Manager
Paketler: Serilog (opsiyonel), FluentValidation (opsiyonel)


1) Kimlik & Rol
Login ekranı 
Rol’e göre menü öğeleri değişmeli (Employee/Manager).
2) İzin Talebi (Employee)
Talep oluştur: StartDate, EndDate, LeaveType (Annual, Sick, Unpaid), Reason.
Doğrulamalar:
StartDate ≤ EndDate
Geçmişe atılan izin: sadece bugünden en fazla 7 gün geriye kadar (örnek kural).
Çakışan (overlap) PENDING/APPROVED izin varsa uyar (kaydetme).
Listele (kullanıcının kendi talepleri), durum filtreleri: PENDING/APPROVED/REJECTED.
Talep PENDING iken güncelle/sil.
3) Onay Süreci (Manager)
Bekleyen talepleri listele (sayfalama + tarih/çalışan/leaveType filtreleri).
Approve/Reject (zorunlu açıklama sadece Reject’te).
Karar işlemleri transactional olmalı; aynı kayıt paralele onaylanmaya çalışılırsa concurrency hatası vermeli (RowVersion).
4) Raporlama
“Aylık İzin Özeti” sayfası:
Ay/Çalışan kırılımında Toplam Onaylı İzin Günleri.
CSV/Excel export (CSV yeterli).
Basit dashboard kartları:
Bu ay onaylanan izin sayısı
En çok kullanılan izin türü
Bekleyen talep sayısı
5) Audit Log
Her talep için CreatedBy/At, UpdatedBy/At.
Onay/ret işlemleri Approvals tablosunda izlenmeli.
