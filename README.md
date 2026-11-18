# Sistem Informasi Nilai Akademik

Aplikasi ASP.NET Core MVC untuk pengelolaan data akademik, input dan rekap nilai, serta rapor siswa. Migrasi Code First disiapkan untuk SQL Server; konfigurasi Development juga mendukung database MySQL lokal yang sudah digunakan proyek.

## Struktur proyek

```text
SistemInformasiNilaiAkademik/
├── Controllers/       # Alur halaman dan aksi aplikasi
├── Data/              # AppDbContext, inisialisasi data, factory EF Core
├── Migrations/        # Riwayat perubahan skema database
├── Models/            # Entitas database
├── Properties/        # Konfigurasi peluncuran lokal
├── Services/          # Logika layanan aplikasi
├── ViewModels/        # Model data untuk halaman dan form
├── Views/             # Halaman Razor
├── wwwroot/           # CSS, JavaScript, dan aset statis
├── Program.cs         # Konfigurasi layanan dan pipeline aplikasi
├── appsettings.json   # Konfigurasi umum tanpa rahasia
└── README.md
```

## Persyaratan

- .NET SDK 8
- MySQL lokal untuk konfigurasi Development bawaan, atau SQL Server jika provider diubah
- Database yang dapat diakses oleh aplikasi

> Target .NET 8 dipertahankan agar sesuai dengan acuan tugas. Dukungan Microsoft untuk .NET 8 berakhir pada 10 November 2026. Sebelum hosting publik, rencanakan upgrade ke versi LTS yang masih didukung dan validasi dependensi aplikasi.

## Menjalankan untuk pengembangan

1. Salin `appsettings.Development.example.json` menjadi `appsettings.Development.json`. Template memakai MySQL lokal `db_sistem_nilai` pada `127.0.0.1:3306` dengan akun `root` tanpa password dan SSL nonaktif untuk koneksi localhost. Jika konfigurasi MySQL Anda berbeda, ubah connection string di file lokal tersebut.
2. Jalankan perintah berikut dari folder proyek:

   ```powershell
   dotnet tool restore
   dotnet restore
   dotnet run
   ```

   Lingkungan Development memastikan skema tersedia lalu mengisi data contoh jika tabel pengguna masih kosong. Jika database sudah memiliki tabel, `EnsureCreated` tidak memperbarui skema yang sudah ada. Akun contoh hanya untuk pengembangan lokal; jangan gunakan kredensial contoh pada server publik.

Untuk memakai migrasi Code First SQL Server secara lokal, ubah `Database:Provider` menjadi `SqlServer` dan atur `ConnectionStrings:SqlServerConnection`. Migrasi di folder `Migrations/` ditujukan untuk SQL Server.

Untuk menambahkan perubahan model database, buat migrasi lalu terapkan ke database pengembangan:

```powershell
dotnet ef migrations add NamaPerubahan
dotnet ef database update
```

Jangan menghapus atau mengubah migrasi yang sudah digunakan pada database bersama. Buat migrasi baru untuk perubahan berikutnya.

## Menyiapkan server

Konfigurasikan nilai berikut melalui environment variables atau secret manager milik platform hosting. Jangan menyimpan password database, password admin, atau file rahasia di Git.

| Environment variable | Kegunaan |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT=Production` | Menjalankan konfigurasi produksi |
| `ConnectionStrings__SqlServerConnection` | Koneksi ke SQL Server produksi |
| `BootstrapAdmin__Username` | Username admin awal jika database belum memiliki pengguna |
| `BootstrapAdmin__Password` | Password kuat untuk admin awal |
| `BootstrapAdmin__Name` | Nama admin awal (opsional) |
| `DataProtection__KeysDirectory` | Folder persisten untuk kunci cookie autentikasi |

Sebelum menjalankan aplikasi di server, pulihkan tool dan terapkan migrasi sebagai langkah rilis:

```powershell
dotnet tool restore
dotnet ef database update
dotnet publish -c Release -o publish
```

Environment variable koneksi database dan `ASPNETCORE_ENVIRONMENT` harus sudah tersedia ketika menjalankan perintah migrasi. Saat pertama kali aplikasi produksi dijalankan, aplikasi membuat akun admin dari konfigurasi bootstrap jika belum ada pengguna. Hapus `BootstrapAdmin__Password` dari konfigurasi runtime setelah admin awal berhasil dibuat. Pastikan folder Data Protection tersimpan secara persisten dan HTTPS aktif pada hosting.

## Push ke GitHub

Sebelum commit, tinjau perubahan dan pastikan file konfigurasi produksi, password, folder `.keys`, serta hasil build `bin`/`obj` tidak ikut terunggah. Simpan konfigurasi server di secret manager platform hosting.

Skema database aplikasi dikelola melalui migrasi EF Core SQL Server di folder `Migrations/`.
