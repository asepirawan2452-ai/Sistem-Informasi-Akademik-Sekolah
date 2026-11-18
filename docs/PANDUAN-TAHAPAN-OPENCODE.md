# Panduan Tahapan Pengerjaan di opencode — Pertemuan 6
**Implementasi 1: Setup ASP.NET Core MVC & Fondasi Modul Master — Sistem Informasi Nilai Akademik**

Catatan: isi file materi bertajuk "Pertemuan 5", sedangkan nama filenya "Pertemuan 6". Panduan ini mengikuti isi materinya (Sesi 1–5).

## Cara pakai
1. Buat folder kosong proyek, ekstrak `AGENTS.md`, `docs/`, dan `seed/` ke dalamnya. Opsional: tambahkan SRS sebagai `docs/SRS.pdf` atau `docs/SRS.md` (hasil salin teks) agar opencode bisa mencocokkan FR.
2. Buka terminal di folder itu, jalankan `opencode`. opencode membaca `AGENTS.md` otomatis.
3. Tekan **Tab** untuk mode *Plan* (opencode hanya merencanakan, tidak mengubah file) bila ingin meninjau dulu, lalu Tab lagi ke mode *Build* untuk eksekusi.
4. Tempel **satu prompt per tahap**. Setelah selesai, cek bagian "Cek hasil" sebelum lanjut. Bila salah, `/undo`.
5. Ambil bukti (screenshot/log) di tiap tahap untuk laporan.

Pemetaan ke materi: Tahap 1–2 = Sesi 1 · Tahap 3 = Sesi 2 · Tahap 4–6 = Sesi 3 · Tahap 7 = Sesi 4 · Tahap 8–9 = Sesi 5.

## Peta tahap

| Tahap | Isi | Siapa | Cabang |
|-------|-----|-------|--------|
| 0 | Persiapan lingkungan | Semua | – |
| 1 | Cek ERD revisi | PIC Backend/DBA | – |
| 2 | Inisialisasi proyek MVC + Git | PIC Backend/DBA | `main` |
| 3 | EF Core, connection string, DbContext | PIC Backend/DBA | `main` |
| 4 | Models dari ERD + konfigurasi | PIC Backend/DBA | `main` |
| 5 | Migration + update database | PIC Backend/DBA | `main` |
| 6 | Seeder data awal (36 akun, mapel, kelas, dll.) | PIC Backend/DBA | `main` |
| 7 | Scaffolding CRUD modul Master | Tiap anggota | `feature/master-*` |
| 8 | Bagi tugas, push, clone | Semua | – |
| 9 | Verifikasi akhir & bukti laporan | Semua | – |

Tahap 2–6 dikerjakan **satu orang** (materi Sesi 5 butir 1), lalu di-push ke `main` sebelum yang lain mulai Tahap 7.

---

## Tahap 0 — Persiapan (manual, di terminal)
```bash
dotnet --version          # harus 8.x (materi: .NET 6 atau 8; seluruh tim harus sama)
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator
git --version
# Di Laragon klik "Start All" (MySQL harus hidup), lalu buka tombol Terminal Laragon:
mysql -u root -e "SELECT VERSION();"
```
Cek hasil: ketiga perintah menampilkan versi. Bila MySQL Laragon memakai port/kata sandi lain, catat untuk connection string. Database dikelola lewat tombol **Database** di Laragon (HeidiSQL).

## Tahap 1 — Cek ERD revisi
Baca `docs/ERD-revisi.md` (15 entitas). Tahap ini hanya agar opencode dan tim sepakat sebelum membuat kode.

**Prompt:**
```
Baca AGENTS.md dan docs/ERD-revisi.md. Jangan ubah file apa pun.
1) Ringkas daftar entitas, kolom, PK/FK, dan unique constraint dalam satu tabel.
2) Cocokkan dengan FR-01 s.d. FR-11 pada docs/SRS.md (jika ada). Sebutkan bila ada kebutuhan yang belum punya tabel atau kolom.
3) Daftar pertanyaan/asumsi yang perlu dikonfirmasi ke tim.
```
Cek hasil: tidak ada entitas yang terlewat. Poin tambahan SRS (FR-12 sampai FR-15) ada di bagian 4 ERD-revisi; bahas dengan tim apakah dimasukkan.

## Tahap 2 — Inisialisasi proyek ASP.NET Core MVC + Git (Sesi 1)
**Prompt:**
```
Buat proyek ASP.NET Core MVC bernama SistemNilaiAkademik di folder saat ini (jangan buat subfolder ganda), target .NET 8, tanpa autentikasi bawaan.
- Pastikan struktur: Models/, Views/, Controllers/, wwwroot/, appsettings.json. Tambah folder Data/.
- Jalankan `git init` bila belum ada repo, tambahkan .gitignore standar .NET (bin/, obj/, .vs/, appsettings.Development.json bila berisi rahasia) dan JANGAN menambahkan seed/users.csv ke repo (masukkan ke .gitignore).
- Di Views/Shared/_Layout.cshtml, ubah judul menjadi "Sistem Informasi Nilai Akademik" dan tambah menu navigasi "Data Master" dengan tautan placeholder: Siswa, Guru, Mata Pelajaran, Kelas, Periode, Jurusan (semua tautan dibuat sekarang agar anggota tim tidak bentrok mengedit _Layout nanti).
- Jalankan `dotnet build` dan tampilkan hasilnya.
- Lakukan satu commit: "chore: inisialisasi proyek ASP.NET Core MVC". Jangan push.
Berhenti setelah itu.
```
Cek hasil: `dotnet build` sukses; `dotnet run` membuka halaman beranda; `git log` berisi 1 commit.

## Tahap 3 — EF Core, connection string, DbContext (Sesi 2)
**Prompt:**
```
Tambahkan paket NuGet Pomelo.EntityFrameworkCore.MySql (versi 8.x) dan Microsoft.EntityFrameworkCore.Tools (versi yang sesuai dengan target framework).
- Tambahkan ConnectionStrings:DefaultConnection di appsettings.json: Server=localhost;Port=3306;Database=db_nilai_akademik;User=root;Password=;
- Buat Data/AppDbContext.cs (turunan DbContext) dengan constructor DbContextOptions<AppDbContext>. DbSet-nya diisi di Tahap 4.
- Daftarkan AppDbContext ke Dependency Injection di Program.cs dengan UseMySql(cs, ServerVersion.AutoDetect(cs)) dengan cs = GetConnectionString("DefaultConnection").
- dotnet build, tampilkan hasil. Commit: "feat: konfigurasi EF Core dan DbContext". Jangan push. Berhenti.
```
Cek hasil: build sukses. Bila nama server berbeda (Tahap 0), sesuaikan connection string.

## Tahap 4 — Models dari ERD + konfigurasi (Sesi 3, bagian 1)
**Prompt:**
```
Terjemahkan docs/ERD-revisi.md menjadi class C# di folder Models/ (satu file per entitas, 15 entitas) dan DbSet di AppDbContext.
Aturan:
- PascalCase; properti navigasi dua arah untuk relasi; Id memakai int identity.
- Data Annotations untuk validasi (Required, StringLength, Range 0–100 untuk nilai, pesan error bahasa Indonesia).
- Konfigurasi di OnModelCreating (Fluent API): semua unique index pada ERD, unique gabungan (Nilai: siswa+mapel+periode+komponen; Rekap: siswa+mapel+periode; BobotNilai: mapel+komponen; PenugasanMengajar: guru+mapel+kelas+periode), decimal(5,2) untuk nilai/persentase/nilai_akhir/nilai_min, CHECK constraint nilai 0–100, semua FK DeleteBehavior.Restrict.
- Guru.NipNrg nullable dengan unique index (MySQL mengizinkan banyak NULL). Semua string yang di-index wajib punya StringLength/HasMaxLength (kode, username, nis, nisn, nama_kelas, dll.), jika tidak MySQL menolaknya (longtext).
- Jangan tambahkan tabel/kolom di luar ERD. Bila ada yang kurang jelas, tulis di akhir jawaban.
dotnet build, tampilkan hasil. Commit: "feat: models dan konfigurasi dari ERD". Jangan push. Berhenti.
```
Cek hasil: build sukses; 15 file model; `OnModelCreating` memuat semua unique index. Ini inti tugas ("ubah ERD menjadi Class Models").

## Tahap 5 — Migration + update database (Sesi 3, bagian 2)
Pengganti `Add-Migration` / `Update-Database` di Package Manager Console Visual Studio.

**Prompt:**
```
Jalankan:
  dotnet ef migrations add InisialisasiAwal
  dotnet ef database update
Periksa berkas migration yang dihasilkan: pastikan 15 tabel, semua unique index, FK Restrict, dan CHECK constraint ada. Setelah update, jalankan `mysql -u root -e "USE db_nilai_akademik; SHOW TABLES;"` untuk menampilkan daftar tabel. Bila error (koneksi, versi dotnet-ef), jelaskan penyebabnya dan perbaiki konfigurasi, bukan model. Commit: "feat: migration InisialisasiAwal". Jangan push. Berhenti.
```
Cek hasil: buka Laragon → Database (HeidiSQL) → db_nilai_akademik berisi 15 tabel + `__EFMigrationsHistory`. **Screenshot ini bukti laporan** (materi: "Cek SSMS", di sini diganti HeidiSQL).

## Tahap 6 — Seeder data awal
Agar modul Master langsung terisi dan akun siap dipakai di pertemuan berikutnya.

**Prompt:**
```
Buat Data/DbSeeder.cs yang idempotent (cek dulu sebelum insert) dan dipanggil dari Program.cs hanya saat Development. Sumber data: folder seed/.
Isi:
1. Role: ADMIN, GURU, WALI_KELAS.
2. Jurusan: TKJ, MPLB, AKL, TE, TBSM (nama lengkap ada di AGENTS.md).
3. Komponen nilai: Harian, UTS, UAS. Rentang huruf: A=85, B=75, C=60, D=0. Satu Periode Akademik aktif "2026/2027 Ganjil".
4. Mapel dari seed/mapel.csv (kelompok Umum/Kejuruan; Kejuruan: isi id_jurusan dari prefiks kode TKJ→TKJ, MPL→MPLB, AKL→AKL, TE→TE, TB→TBSM).
5. Pengguna + Guru dari seed/users.csv. Kata sandi WAJIB di-hash dengan Microsoft.AspNetCore.Identity.PasswordHasher<Pengguna> (bukan teks biasa). Setiap baris GURU dan WALI_KELAS juga membuat satu record Guru (NipNrg dikosongkan). Admin hanya membuat Pengguna.
6. Kelas dari seed/kelas.csv (wali kelas = Guru dengan username terkait).
7. PenugasanMengajar dari seed/penugasan_mengajar.csv untuk periode aktif.
8. BobotNilai default 30/30/40 untuk setiap mapel.
Setelah dijalankan, verifikasi jumlah: Pengguna 36, Guru 34, Kelas 15, Mapel 41, PenugasanMengajar 157, BobotNilai 123, Role 3. Tampilkan hasil verifikasi. Pastikan kolom PasswordHash tidak berisi 'admin123'/'guru123'/'wali123'. Jalankan dua kali untuk memastikan jumlah tidak berlipat. Commit: "feat: seeder data awal". Jangan push. Berhenti.
```
Cek hasil: angka verifikasi cocok (36 / 34 / 15 / 41 / 157 / 123 / 3) dan tidak ada teks biasa di `PasswordHash`.

Opsional (bila dosen ingin data lebih hidup): minta opencode membuat ±5 siswa dummy per kelas dengan nama jelas fiktif; beri label "DUMMY" di kolom nama atau pisahkan dalam seeder terpisah yang bisa dimatikan.

Setelah Tahap 6: **`git push origin main`** (dilakukan manual oleh PIC) — lihat Tahap 8.

## Tahap 7 — Scaffolding CRUD modul Master (Sesi 4)
Dikerjakan masing-masing anggota di cabang sendiri setelah `git pull` dari `main` (Tahap 8). Satu prompt per entitas, ganti `<Entitas>`. Pembagian yang disarankan: Siswa, Guru, Mapel, Kelas, Periode+Jurusan, Pengguna (akun; opsional lanjut di pertemuan berikutnya).

**Prompt (contoh Siswa):**
```
Saya di cabang feature/master-siswa. Buat CRUD (Create, Read, Update, Delete) untuk entitas Siswa menggunakan scaffolding:
  dotnet aspnet-codegenerator controller -name SiswaController -m Siswa -dc AppDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
(pasang paket Microsoft.VisualStudio.Web.CodeGeneration.Design bila diminta; kalau scaffolding gagal, tulis controller+view manual dengan pola yang sama).
Setelah scaffold, rapikan sesuai SRS FR-02:
- Form: dropdown Kelas (dari tabel Kelas), jenis kelamin; validasi NIS & NISN unik (pesan "NIS sudah terdaftar").
- Daftar: kolom pencarian (nama/NIS) dan menampilkan nama kelas.
- Delete: jika siswa sudah punya Nilai, jangan hapus, ubah Status menjadi Nonaktif dan beri pesan; jika belum, boleh hapus.
- Semua teks antarmuka berbahasa Indonesia.
Hanya ubah file milik Siswa (SiswaController, Views/Siswa). Jangan sentuh Program.cs, _Layout, AppDbContext, atau migration. dotnet build lalu tampilkan hasilnya. Commit: "feat(siswa): CRUD master siswa". Jangan push. Berhenti.
```
Variasi singkat per entitas:
- **Guru** (FR-03): dropdown Pengguna (akun terkait, hanya yang belum dipakai guru lain), NIP/NRG unik bila diisi, hapus lunak bila punya penugasan/nilai.
- **Mapel** (FR-04): kode unik, kelompok Umum/Kejuruan, dropdown Jurusan (wajib bila Kejuruan), hapus lunak bila sudah ada nilai.
- **Kelas** (FR-05): dropdown Jurusan dan Wali Kelas (satu wali hanya satu kelas, tolak bila dobel), tingkat X/XI/XII.
- **Periode & Jurusan**: CRUD sederhana; hanya boleh satu periode berstatus Aktif.

Cek hasil: `dotnet run`, buka menu terkait, coba tambah/ubah/hapus, coba data duplikat (harus ditolak).

## Tahap 8 — Alur Git tim (Sesi 5, butir 2–4)
Dikerjakan manual di terminal biasa (bukan lewat opencode, agar kendali push tetap di manusia).
```bash
# PIC Backend/DBA, setelah Tahap 2–6
git branch -M main
git remote add origin <URL-repo>
git push -u origin main

# Anggota lain
git clone <URL-repo>
cd SistemNilaiAkademik
dotnet ef database update          # buat database lokal dari migration yang sudah ada
git checkout -b feature/master-siswa

# Setelah Tahap 7
git push -u origin feature/master-siswa
# lalu Pull Request ke main
```
Aturan tim:
- **Jangan buat migration baru** di cabang fitur. Bila perlu ubah struktur tabel, kabari PIC.
- `seed/users.csv` tidak ikut repo; PIC membagikannya lewat jalur terpisah, atau seeder dijalankan hanya oleh PIC.
- Satu cabang = satu entitas, supaya tidak terjadi konflik merge.

## Tahap 9 — Verifikasi akhir & bukti laporan
**Prompt:**
```
Audit read-only (jangan ubah file): jalankan dotnet build; daftar semua tabel dan jumlah barisnya dari database; daftar controller & view hasil scaffolding; periksa bahwa tidak ada kata sandi teks biasa di database/kode/log; periksa bahwa tidak ada controller untuk Nilai/Rekap/Rapor (belum boleh di-scaffold). Tampilkan ringkasan dalam tabel Lulus/Gagal beserta alasannya.
```
Daftar bukti untuk laporan/dosen:
1. Screenshot HeidiSQL (Laragon): 15 tabel hasil migration.
2. Potongan `Models/` dan `AppDbContext` (ERD → Class → Tabel).
3. Screenshot tiap halaman CRUD master (daftar, tambah, validasi duplikat).
4. `git log --oneline --graph` memperlihatkan cabang per anggota dan merge ke `main`.

## Di luar lingkup pertemuan ini (sengaja ditunda)
Login/otorisasi peran (FR-01), Input & perubahan nilai (FR-06/07), perhitungan nilai akhir & konversi huruf (FR-08/09, termasuk unit test dengan kasus 82,5→B), rapor (FR-10), grafik (FR-11). Modul Transaksi harus ditulis manual, tidak lewat scaffolding.
