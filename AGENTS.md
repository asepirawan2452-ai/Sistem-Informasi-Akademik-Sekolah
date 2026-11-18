# AGENTS.md — Sistem Informasi Nilai Akademik (Kelompok 4)

Konteks tetap untuk opencode. Baca file ini sebelum mengerjakan tugas apa pun.

## Proyek
Aplikasi web pengelolaan nilai akademik SMK (Admin, Guru, Wali Kelas). Tugas kuliah Rekayasa Perangkat Lunak.
Sumber kebenaran: `docs/ERD-revisi.md` (struktur data), `docs/SRS.md` jika ada (FR-01..FR-11), `seed/*.csv` (data awal).

## Stack (sesuai materi praktikum)
- ASP.NET Core MVC (Razor `.cshtml`), target .NET 8 (atau SDK terpasang yang paling dekat; jalankan `dotnet --version` dulu)
- Entity Framework Core **Code-First** + **MySQL/MariaDB dari Laragon** (paket `Pomelo.EntityFrameworkCore.MySql` versi 8.x yang cocok dengan EF Core 8, dan `Microsoft.EntityFrameworkCore.Tools`). Materi memakai SQL Server, tetapi alurnya (Code-First, migration) identik; hanya provider yang berbeda.
- Connection string default: `Server=localhost;Port=3306;Database=db_nilai_akademik;User=root;Password=;` (default Laragon: root tanpa kata sandi). Pakai `UseMySql(cs, ServerVersion.AutoDetect(cs))`. MySQL Laragon harus sudah berjalan (tombol Start All) sebelum `dotnet ef`.
- MySQL: setiap kolom string yang diberi unique index/FK WAJIB punya `[StringLength]`/`HasMaxLength` (tanpa itu jadi `longtext` dan migration gagal). Gunakan charset utf8mb4. Unique index pada kolom nullable otomatis mengizinkan banyak NULL.
- Pakai perintah `dotnet` CLI, bukan Package Manager Console:
  `dotnet ef migrations add <Nama>` dan `dotnet ef database update`

## Struktur folder
`Models/` entitas · `Data/` AppDbContext + seeder · `Controllers/` · `Views/` · `wwwroot/` aset statis · `appsettings.json` konfigurasi

## Aturan kerja
1. Kerjakan **satu tahap per permintaan**. Setelah selesai: ringkas apa yang dibuat/diubah, tampilkan hasil `dotnet build`, lalu berhenti dan tunggu perintah berikutnya.
2. Jangan ubah struktur tabel di luar `docs/ERD-revisi.md`. Jika ada yang kurang/janggal, laporkan, jangan menebak diam-diam.
3. **Jangan** scaffolding untuk modul Transaksi (Input Nilai, Rekap, Rapor). Itu ditulis manual di pertemuan berikutnya.
4. Perhitungan nilai tidak boleh di sisi klien (SRS FR-08). Belum dikerjakan di tahap ini.
5. Kata sandi selalu di-hash (SRS FR-01). Jangan pernah menyimpan atau mencetak teks biasa ke log.
6. Hapus data master = **hapus lunak** (ubah `Status`), bukan `DELETE`, bila data sudah dipakai nilai (SRS FR-02/03/04).
7. Semua FK `DeleteBehavior.Restrict`. Kolom nilai `decimal(5,2)`.
8. Antarmuka berbahasa Indonesia, pesan error jelas (NFR-05). Pakai Data Annotations + validasi server.
9. Jangan jalankan `git push`, `git reset --hard`, atau hapus folder tanpa diminta. Commit boleh hanya jika diminta.
10. Satu orang saja yang membuat migration (cegah konflik `AppDbContextModelSnapshot.cs`).

## Aturan bisnis penting (dari SRS)
- Rentang nilai 0–100. Bobot default Harian 30 / UTS 30 / UAS 40, total harus 100.
- Huruf: A ≥ 85, B ≥ 75, C ≥ 60, D ≥ 0 (disimpan di tabel `RENTANG_NILAI_HURUF`).
- Contoh uji: (80×30%)+(75×30%)+(90×40%) = 82,5 → B. Batas: 85→A, 74,9→C, 59,9→D.
- Peran: ADMIN (kelola master & akun), GURU (input nilai sesuai penugasan), WALI_KELAS (hak Guru + cetak rapor kelasnya).

## Nama lengkap jurusan (konfirmasi dulu ke tim sebelum dipakai di laporan)
TKJ = Teknik Komputer dan Jaringan · MPLB = Manajemen Perkantoran dan Layanan Bisnis · AKL = Akuntansi dan Keuangan Lembaga · TE = Teknik Elektronika · TBSM = Teknik dan Bisnis Sepeda Motor
(Dokumen sumber hanya menyebut singkatannya; materi mapel TKJ memakai istilah "TJKT".)

## Data awal (`seed/`)
`users.csv` (36 akun), `mapel.csv` (41 mapel), `kelas.csv` (15 rombel), `penugasan_mengajar.csv` (157 baris).
`password_plain` hanya untuk di-hash saat seeding; jangan commit file ini ke repo publik.
Seeder harus idempotent (aman dijalankan berulang).

## Git
Cabang: `main` (kerangka) → `feature/master-<entitas>` per anggota. Commit kecil, pesan jelas (mis. `feat(siswa): CRUD master siswa`).
