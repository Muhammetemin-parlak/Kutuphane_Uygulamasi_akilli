# Kütüphane Otomasyon Sistemi

Bu proje, bir kütüphanedeki kitap, yazar, kategori, kullanıcı ve ödünç alma (emanet) işlemlerini dijital ortamda yönetmek için geliştirilmiş bir **ASP.NET Core MVC** web uygulamasıdır.

## Özellikler

- **Kitap Yönetimi:** Kitap ekleme, düzenleme, silme (soft-delete) ve listeleme.
- **Gelişmiş Arama ve Filtreleme:** Kitap başlığına veya yazar adına göre arama, kategoriye göre filtreleme.
- **Sayfalama (Pagination):** Kitap listesinde performanslı sayfalama desteği.
- **İlişkisel Veri Yapısı:** Kitaplar, Yazarlar ve Kategoriler arasında tam entegrasyon.
- **Ödünç Alma Sistemi:** Kitapların kullanıcılara ödünç verilmesi ve takibi.
- **Kimlik Doğrulama:** Identity altyapısı ile kullanıcı kayıt ve giriş işlemleri.

## Kullanılan Teknolojiler
- **Framework:** .NET 8.0 ASP.NET Core MVC
- **Veritabanı:** SQL Server (LocalDB)
- **ORM:** Entity Framework Core 8.0
- **Frontend:** Bootstrap, Razor Views, jQuery

## Veritabanı Yapısı
Proje aşağıdaki tablolar üzerine inşa edilmiştir:
- `books`: Kitap bilgilerini ve silinme durumunu tutar.
- `authors`: Yazarların bilgilerini içerir.
- `categories`: Kitap kategorilerini tanımlar.
- `users`: Sistem kullanıcılarını ve rollerini yönetir.
- `loans`: Kitapların hangi kullanıcıda olduğunu takip eder.

## Kurulum ve Çalıştırma

### 1. Gereksinimler
- .NET 8.0 SDK
- Visual Studio 2022 veya VS Code
- SQL Server Express veya LocalDB

### 2. Veritabanı Yapılandırması

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KutuphaneProjesiDB;Trusted_Connection=True;..."
