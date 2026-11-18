# Mode Otomatis — satu prompt untuk Tahap 2–7 (opencode + Laragon MySQL)

## Sebelum menjalankan (manual, sekali saja)
1. Laragon: klik **Start All** sampai MySQL berstatus hidup.
2. PowerShell di folder proyek: `dotnet --version` (8.x), `dotnet tool install --global dotnet-ef`, `dotnet tool install --global dotnet-aspnet-codegenerator`, `git --version`.
3. Pastikan `opencode.json` (izin otomatis) ada di folder proyek, lalu jalankan `opencode` dari folder itu.
   `opencode.json` mengizinkan edit & perintah shell tanpa tanya, tetapi MENOLAK `git push`, `git reset --hard`, `git clean`, dan penghapusan file. Bila opencode versi Anda menolak format izin ini, hapus file itu dan pilih "Allow always" saat diminta.

## Prompt (tempel seluruhnya, mode Build)
```
Kerjakan pertemuan 6 secara otomatis dari awal sampai akhir tanpa berhenti bertanya, kecuali ada error yang tidak bisa kamu perbaiki. Baca dulu AGENTS.md dan docs/ERD-revisi.md (sumber kebenaran), serta docs/PANDUAN-TAHAPAN-OPENCODE.md (detail tiap tahap). Database: MySQL Laragon (localhost:3306, root tanpa password, database db_nilai_akademik); asumsikan MySQL sudah hidup, jika koneksi gagal berhenti dan beri tahu saya.

URUTAN (satu commit per tahap, di cabang main kecuali disebut lain; jangan git push):
A. Tahap 2: proyek ASP.NET Core MVC "SistemNilaiAkademik" di folder ini (net8.0, tanpa auth bawaan), git init, .gitignore (termasuk seed/users.csv), menu Data Master di _Layout dengan semua tautan placeholder.
B. Tahap 3: paket Pomelo.EntityFrameworkCore.MySql 8.x + Microsoft.EntityFrameworkCore.Tools, connection string, Data/AppDbContext, daftar ke DI dengan UseMySql + ServerVersion.AutoDetect.
C. Tahap 4: 15 class model dari ERD + DbSet + Fluent API (unique index, unique gabungan, decimal(5,2), CHECK nilai 0-100, FK Restrict, StringLength pada semua string ber-index).
D. Tahap 5: dotnet ef migrations add InisialisasiAwal, dotnet ef database update, lalu verifikasi 15 tabel dengan `mysql -u root -e "USE db_nilai_akademik; SHOW TABLES;"`.
E. Tahap 6: Data/DbSeeder idempotent dari folder seed/ (role, jurusan, komponen, rentang huruf, periode aktif 2026/2027 Ganjil, mapel, pengguna+guru dengan PasswordHasher, kelas, penugasan, bobot 30/30/40). Verifikasi jumlah: Pengguna 36, Guru 34, Kelas 15, Mapel 41, PenugasanMengajar 157, BobotNilai 123, Role 3; jalankan dua kali untuk memastikan tidak berlipat; pastikan PasswordHash bukan teks biasa.
F. Tahap 7: untuk setiap entitas Siswa, Guru, Mapel, Kelas, Periode, Jurusan: buat cabang feature/master-<entitas>, scaffold CRUD dengan dotnet aspnet-codegenerator (controller+view; jika gagal tulis manual), rapikan sesuai SRS (dropdown FK, pencarian, validasi unik, hapus lunak bila sudah dipakai nilai/penugasan, teks Indonesia), dotnet build, commit, kembali ke main, lalu `git merge --no-ff feature/master-<entitas>`. Hanya ubah file milik entitas itu.
G. Verifikasi akhir: dotnet build sukses; dotnet run singkat lalu cek setiap halaman master merespons HTTP 200 (matikan app setelahnya); tampilkan `git log --oneline --graph --all`.

ATURAN: jangan scaffold Nilai/Rekap/Rapor/Login; jangan buat migration tambahan kecuali memperbaiki kesalahan Tahap 4-5 (jelaskan alasannya); jangan menulis kata sandi teks biasa ke kode, log, atau database.
Setelah tiap tahap, tulis satu baris status "TAHAP X SELESAI: <bukti singkat>". Di akhir beri tabel Lulus/Gagal per tahap dan daftar hal yang perlu saya cek manual.
```

## Setelah selesai (manual)
1. Buka Laragon → **Database** → db_nilai_akademik: pastikan 15 tabel dan data seed (screenshot untuk laporan).
2. `dotnet run`, buka tiap menu Data Master, coba tambah/ubah/hapus dan data duplikat.
3. `git log --oneline --graph --all` untuk bukti cabang per entitas. Push ke GitHub dilakukan manual: `git remote add origin <URL>` lalu `git push -u origin --all`.

## Bila macet
- *Access denied / Unable to connect*: MySQL Laragon belum hidup atau kata sandi root bukan kosong; sesuaikan connection string.
- *Specified key was too long / BLOB/TEXT used in key*: ada string ber-index tanpa StringLength; minta opencode memperbaiki model lalu `dotnet ef migrations remove` dan buat ulang.
- *Pomelo versi tidak cocok*: minta opencode memakai Pomelo 8.0.x untuk net8.0.
