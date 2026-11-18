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

