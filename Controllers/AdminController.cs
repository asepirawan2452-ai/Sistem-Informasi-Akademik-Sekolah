using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.Models;
using SistemInformasiNilaiAkademik.ViewModels;
using SistemInformasiNilaiAkademik.Services.Testing;

namespace SistemInformasiNilaiAkademik.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISystemTestSuite _testSuite;

        public AdminController(AppDbContext context, ISystemTestSuite testSuite)
        {
            _context = context;
            _testSuite = testSuite;
        }

        private async Task<PeriodeAkademikAdminViewModel> BuildPeriodeAkademikViewModelAsync(PeriodeAkademikFormViewModel? form = null)
        {
            var daftarPeriode = await _context.PeriodeAkademik
                .OrderByDescending(p => p.TahunAjaran)
                .ThenByDescending(p => p.Semester == "Genap")
                .ToListAsync();
            var periodeAktif = daftarPeriode.FirstOrDefault(p => p.Status == "Aktif");
            var tahunAjaranBerikutnya = periodeAktif?.TahunAjaran ?? string.Empty;
            var bagianTahun = tahunAjaranBerikutnya.Split('/', StringSplitOptions.TrimEntries);
            if (periodeAktif?.Semester == "Genap" && bagianTahun.Length == 2 &&
                int.TryParse(bagianTahun[0], out var tahunMulai) && int.TryParse(bagianTahun[1], out var tahunSelesai))
            {
                tahunAjaranBerikutnya = $"{tahunMulai + 1}/{tahunSelesai + 1}";
            }

            return new PeriodeAkademikAdminViewModel
            {
                Form = form ?? new PeriodeAkademikFormViewModel
                {
                    TahunAjaran = tahunAjaranBerikutnya,
                    Semester = periodeAktif?.Semester == "Ganjil" ? "Genap" : "Ganjil"
                },
                DaftarPeriode = daftarPeriode,
                PeriodeAktif = periodeAktif
            };
        }

        // ==================== PERIODE AKADEMIK ====================
        public async Task<IActionResult> PeriodeAkademik()
        {
            return View(await BuildPeriodeAkademikViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TambahPeriodeAkademik(PeriodeAkademikAdminViewModel model)
        {
            model.Form ??= new PeriodeAkademikFormViewModel();
            var form = model.Form;
            var tahunParts = string.IsNullOrWhiteSpace(form.TahunAjaran)
                ? Array.Empty<string>()
                : form.TahunAjaran.Split('/', StringSplitOptions.TrimEntries);
            if (tahunParts.Length != 2 ||
                !int.TryParse(tahunParts[0], out var tahunMulai) ||
                !int.TryParse(tahunParts[1], out var tahunSelesai) ||
                tahunSelesai != tahunMulai + 1)
            {
                ModelState.AddModelError("Form.TahunAjaran", "Tahun ajaran harus berurutan, misalnya 2025/2026.");
            }

            if (form.TanggalMulai == default)
            {
                ModelState.AddModelError("Form.TanggalMulai", "Tanggal mulai wajib diisi.");
            }

            if (form.TanggalSelesai == default)
            {
                ModelState.AddModelError("Form.TanggalSelesai", "Tanggal selesai wajib diisi.");
            }
            else if (form.TanggalSelesai.Date < form.TanggalMulai.Date)
            {
                ModelState.AddModelError("Form.TanggalSelesai", "Tanggal selesai harus sama atau setelah tanggal mulai.");
            }

            if (await _context.PeriodeAkademik.AnyAsync(p => p.TahunAjaran == form.TahunAjaran && p.Semester == form.Semester))
            {
                ModelState.AddModelError("Form.Semester", "Periode untuk semester dan tahun ajaran tersebut sudah tersedia.");
            }

            if (!ModelState.IsValid)
            {
                return View("PeriodeAkademik", await BuildPeriodeAkademikViewModelAsync(form));
            }

            var periodeAktif = await _context.PeriodeAkademik.FirstOrDefaultAsync(p => p.Status == "Aktif");
            var periodeBaru = new PeriodeAkademik
            {
                NamaPeriode = $"Semester {form.Semester} {form.TahunAjaran}",
                Semester = form.Semester,
                TahunAjaran = form.TahunAjaran,
                TanggalMulai = form.TanggalMulai.Date,
                TanggalSelesai = form.TanggalSelesai.Date,
                Status = periodeAktif == null ? "Aktif" : "Nonaktif"
            };

            if (form.SalinPenugasanPeriodeAktif && periodeAktif != null)
            {
                var penugasanAktif = await _context.PenugasanMengajar
                    .Where(p => p.IdPeriode == periodeAktif.IdPeriode)
                    .ToListAsync();

                foreach (var penugasan in penugasanAktif)
                {
                    periodeBaru.PenugasanList.Add(new PenugasanMengajar
                    {
                        IdGuru = penugasan.IdGuru,
                        IdMapel = penugasan.IdMapel,
                        IdKelas = penugasan.IdKelas,
                        Periode = periodeBaru
                    });
                }
            }

            _context.PeriodeAkademik.Add(periodeBaru);
            await _context.SaveChangesAsync();

            var jumlahPenugasan = periodeBaru.PenugasanList.Count;
            TempData["SuccessMessage"] = periodeAktif == null
                ? $"{periodeBaru.NamaPeriode} dibuat dan langsung diaktifkan."
                : jumlahPenugasan > 0
                    ? $"{periodeBaru.NamaPeriode} dibuat dengan menyalin {jumlahPenugasan} penugasan. Aktifkan periode tersebut saat mulai digunakan."
                    : $"{periodeBaru.NamaPeriode} berhasil dibuat. Aktifkan periode tersebut saat mulai digunakan.";

            return RedirectToAction(nameof(PeriodeAkademik));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktifkanPeriodeAkademik(int id)
        {
            var periodeDipilih = await _context.PeriodeAkademik.FindAsync(id);
            if (periodeDipilih == null)
            {
                return NotFound();
            }

            var semuaPeriode = await _context.PeriodeAkademik.ToListAsync();
            foreach (var periode in semuaPeriode)
            {
                periode.Status = periode.IdPeriode == id ? "Aktif" : "Nonaktif";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Periode aktif sekarang {periodeDipilih.NamaPeriode}. Input nilai, rekap, dan rapor mengikuti periode ini.";
            return RedirectToAction(nameof(PeriodeAkademik));
        }

        // ==================== PENGUJIAN SISTEM OTOMATIS (NFR-03) ====================
        public async Task<IActionResult> Pengujian()
        {
            var result = await _testSuite.RunAllTestsAsync();
            return View(result);
        }

        // ==================== DASHBOARD ADMIN ====================
        public async Task<IActionResult> Index()
        {
            var periodeAktif = await _context.PeriodeAkademik.FirstOrDefaultAsync(p => p.Status == "Aktif");
            var totalSiswa = await _context.Siswa.CountAsync();
            var totalGuru = await _context.Guru.CountAsync();
            var totalKelas = await _context.Kelas.CountAsync();
            var totalMapel = await _context.Mapel.CountAsync();
            var totalPengguna = await _context.Pengguna.CountAsync();
            var totalNilai = periodeAktif == null
                ? 0
                : await _context.Nilai.CountAsync(n => n.IdPeriode == periodeAktif.IdPeriode);

            var siswaTerbaru = await _context.Siswa
                .Include(s => s.Kelas)
                .OrderByDescending(s => s.IdSiswa)
                .Take(5)
                .ToListAsync();

            var logTerbaru = await _context.LogPerubahanNilai
                .Include(l => l.Pengguna)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Mapel)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Siswa)
                .Where(l => periodeAktif != null && l.Nilai != null && l.Nilai.IdPeriode == periodeAktif.IdPeriode)
                .OrderByDescending(l => l.WaktuPerubahan)
                .Take(5)
                .ToListAsync();

            var distJurusan = await _context.Kelas
                .Include(k => k.Jurusan)
                .GroupBy(k => k.Jurusan != null ? k.Jurusan.Kode : "Lainnya")
                .Select(g => new { Jurusan = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Jurusan, x => x.Count);

            var model = new AdminDashboardViewModel
            {
                TotalSiswa = totalSiswa,
                TotalGuru = totalGuru,
                TotalKelas = totalKelas,
                TotalMapel = totalMapel,
                TotalPengguna = totalPengguna,
                TotalNilaiTercatat = totalNilai,
                NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif",
                SiswaTerbaru = siswaTerbaru,
                LogTerbaru = logTerbaru,
                DistribusiJurusan = distJurusan
            };

            return View(model);
        }

        // ==================== FR-02: MASTER DATA SISWA ====================
        public async Task<IActionResult> Siswa(string? search, int? kelasId)
        {
            var query = _context.Siswa.Include(s => s.Kelas).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                query = query.Where(s => s.NamaSiswa.ToLower().Contains(q) || s.Nis.Contains(q) || s.Nisn.Contains(q));
            }

            if (kelasId.HasValue && kelasId.Value > 0)
            {
                query = query.Where(s => s.IdKelas == kelasId.Value);
            }

            ViewBag.KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", kelasId);
            ViewBag.Search = search;

            var list = await query.OrderBy(s => s.NamaSiswa).ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSiswa()
        {
            var model = new SiswaFormViewModel
            {
                KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSiswa(SiswaFormViewModel model)
        {
            if (await _context.Siswa.AnyAsync(s => s.Nis == model.Nis))
            {
                ModelState.AddModelError("Nis", "NIS sudah terdaftar.");
            }
            if (await _context.Siswa.AnyAsync(s => s.Nisn == model.Nisn))
            {
                ModelState.AddModelError("Nisn", "NISN sudah terdaftar.");
            }

            if (!ModelState.IsValid)
            {
                model.KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", model.IdKelas);
                return View(model);
            }

            var siswa = new Siswa
            {
                Nis = model.Nis,
                Nisn = model.Nisn,
                NamaSiswa = model.NamaSiswa,
                JenisKelamin = model.JenisKelamin,
                TanggalLahir = model.TanggalLahir,
                Alamat = model.Alamat ?? string.Empty,
                IdKelas = model.IdKelas,
                Status = model.Status
            };

            _context.Siswa.Add(siswa);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Siswa {siswa.NamaSiswa} berhasil ditambahkan.";
            return RedirectToAction(nameof(Siswa));
        }

        [HttpGet]
        public async Task<IActionResult> EditSiswa(int id)
        {
            var s = await _context.Siswa.FindAsync(id);
            if (s == null) return NotFound();

            var model = new SiswaFormViewModel
            {
                IdSiswa = s.IdSiswa,
                Nis = s.Nis,
                Nisn = s.Nisn,
                NamaSiswa = s.NamaSiswa,
                JenisKelamin = s.JenisKelamin,
                TanggalLahir = s.TanggalLahir,
                Alamat = s.Alamat,
                IdKelas = s.IdKelas,
                Status = s.Status,
                KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", s.IdKelas)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSiswa(int id, SiswaFormViewModel model)
        {
            if (id != model.IdSiswa) return NotFound();

            if (await _context.Siswa.AnyAsync(s => s.Nis == model.Nis && s.IdSiswa != id))
            {
                ModelState.AddModelError("Nis", "NIS sudah digunakan oleh siswa lain.");
            }
            if (await _context.Siswa.AnyAsync(s => s.Nisn == model.Nisn && s.IdSiswa != id))
            {
                ModelState.AddModelError("Nisn", "NISN sudah digunakan oleh siswa lain.");
            }

            if (!ModelState.IsValid)
            {
                model.KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", model.IdKelas);
                return View(model);
            }

            var s = await _context.Siswa.FindAsync(id);
            if (s == null) return NotFound();

            s.Nis = model.Nis;
            s.Nisn = model.Nisn;
            s.NamaSiswa = model.NamaSiswa;
            s.JenisKelamin = model.JenisKelamin;
            s.TanggalLahir = model.TanggalLahir;
            s.Alamat = model.Alamat ?? string.Empty;
            s.IdKelas = model.IdKelas;
            s.Status = model.Status;

            _context.Siswa.Update(s);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Data siswa {s.NamaSiswa} berhasil diperbarui.";
            return RedirectToAction(nameof(Siswa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSiswa(int id)
        {
            var s = await _context.Siswa.Include(x => x.NilaiList).FirstOrDefaultAsync(x => x.IdSiswa == id);
            if (s == null) return NotFound();

            if (s.NilaiList.Any())
            {
                // Sesuai SRS FR-02: data yang memiliki transaksi nilai tidak dihapus permanen
                s.Status = "Nonaktif";
                await _context.SaveChangesAsync();
                TempData["WarningMessage"] = $"Siswa {s.NamaSiswa} memiliki transaksi nilai, sehingga statusnya diubah menjadi Nonaktif (soft delete).";
            }
            else
            {
                _context.Siswa.Remove(s);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Siswa {s.NamaSiswa} berhasil dihapus.";
            }

            return RedirectToAction(nameof(Siswa));
        }

        // ==================== FR-03: MASTER DATA GURU ====================
        public async Task<IActionResult> Guru(string? search)
        {
            var query = _context.Guru
                .Include(g => g.Pengguna)
                .Include(g => g.KelasWali)
                .Include(g => g.PenugasanList)
                    .ThenInclude(p => p.Mapel)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                query = query.Where(g => g.Nama.ToLower().Contains(q) || g.NipNrg.Contains(q));
            }

            var list = await query.OrderBy(g => g.Nama).ToListAsync();
            ViewBag.Search = search;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateGuru()
        {
            // Ambil akun pengguna dengan role Guru atau WaliKelas yang belum ditautkan ke data Guru
            var existingGuruUserIds = await _context.Guru.Select(g => g.IdPengguna).ToListAsync();
            var unlinkedUsers = await _context.Pengguna
                .Include(p => p.Role)
                .Where(p => !existingGuruUserIds.Contains(p.IdPengguna) && p.Role != null && (p.Role.NamaRole == "Guru" || p.Role.NamaRole == "WaliKelas"))
                .ToListAsync();

            var model = new GuruFormViewModel
            {
                PenggunaList = new SelectList(unlinkedUsers, "IdPengguna", "Nama")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGuru(GuruFormViewModel model)
        {
            if (await _context.Guru.AnyAsync(g => g.NipNrg == model.NipNrg))
            {
                ModelState.AddModelError("NipNrg", "NIP/NRG sudah terdaftar.");
            }

            // Jika admin mengisi username & password baru untuk membuat akun pengguna sekaligus
            if (model.IdPengguna == 0 && !string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
            {
                if (await _context.Pengguna.AnyAsync(p => p.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username sudah digunakan.");
                }
            }

            if (!ModelState.IsValid)
            {
                var existingGuruUserIds = await _context.Guru.Select(g => g.IdPengguna).ToListAsync();
                var unlinkedUsers = await _context.Pengguna
                    .Include(p => p.Role)
                    .Where(p => !existingGuruUserIds.Contains(p.IdPengguna) && p.Role != null && (p.Role.NamaRole == "Guru" || p.Role.NamaRole == "WaliKelas"))
                    .ToListAsync();
                model.PenggunaList = new SelectList(unlinkedUsers, "IdPengguna", "Nama", model.IdPengguna);
                return View(model);
            }

            int idPenggunaFinal = model.IdPengguna;
            if (idPenggunaFinal == 0)
            {
                var roleGuru = await _context.Roles.FirstAsync(r => r.NamaRole == "Guru");
                var newPengguna = new Pengguna
                {
                    Nama = model.Nama,
                    Username = model.Username.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password!),
                    IdRole = roleGuru.IdRole,
                    Status = "Aktif"
                };
                _context.Pengguna.Add(newPengguna);
                await _context.SaveChangesAsync();
                idPenggunaFinal = newPengguna.IdPengguna;
            }

            var guru = new Guru
            {
                IdPengguna = idPenggunaFinal,
                NipNrg = model.NipNrg,
                Nama = model.Nama,
                Status = model.Status
            };

            _context.Guru.Add(guru);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Guru {guru.Nama} berhasil ditambahkan.";
            return RedirectToAction(nameof(Guru));
        }

        [HttpGet]
        public async Task<IActionResult> EditGuru(int id)
        {
            var g = await _context.Guru.Include(x => x.Pengguna).FirstOrDefaultAsync(x => x.IdGuru == id);
            if (g == null) return NotFound();

            var model = new GuruFormViewModel
            {
                IdGuru = g.IdGuru,
                IdPengguna = g.IdPengguna,
                NipNrg = g.NipNrg,
                Nama = g.Nama,
                Status = g.Status,
                Username = g.Pengguna?.Username ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGuru(int id, GuruFormViewModel model)
        {
            if (id != model.IdGuru) return NotFound();

            if (await _context.Guru.AnyAsync(g => g.NipNrg == model.NipNrg && g.IdGuru != id))
            {
                ModelState.AddModelError("NipNrg", "NIP/NRG sudah digunakan guru lain.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var g = await _context.Guru.Include(x => x.Pengguna).FirstOrDefaultAsync(x => x.IdGuru == id);
            if (g == null) return NotFound();

            g.NipNrg = model.NipNrg;
            g.Nama = model.Nama;
            g.Status = model.Status;

            if (g.Pengguna != null)
            {
                g.Pengguna.Nama = model.Nama;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    g.Pengguna.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }
            }

            _context.Guru.Update(g);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Data guru {g.Nama} berhasil diperbarui.";
            return RedirectToAction(nameof(Guru));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGuru(int id)
        {
            var g = await _context.Guru
                .Include(x => x.NilaiDiinputList)
                .Include(x => x.PenugasanList)
                .Include(x => x.KelasWali)
                .FirstOrDefaultAsync(x => x.IdGuru == id);

            if (g == null) return NotFound();

            if (g.NilaiDiinputList.Any() || g.KelasWali != null)
            {
                g.Status = "Nonaktif";
                await _context.SaveChangesAsync();
                TempData["WarningMessage"] = $"Guru {g.Nama} memiliki transaksi nilai atau ditugaskan sebagai wali kelas, status diubah menjadi Nonaktif.";
            }
            else
            {
                _context.PenugasanMengajar.RemoveRange(g.PenugasanList);
                _context.Guru.Remove(g);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Guru {g.Nama} berhasil dihapus.";
            }

            return RedirectToAction(nameof(Guru));
        }

        // ==================== FR-04: MASTER DATA MAPEL ====================
        public async Task<IActionResult> Mapel(string? search, string? kelompok)
        {
            var query = _context.Mapel.Include(m => m.Jurusan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                query = query.Where(m => m.NamaMapel.ToLower().Contains(q) || m.KodeMapel.ToLower().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(kelompok))
            {
                query = query.Where(m => m.Kelompok == kelompok);
            }

            ViewBag.Search = search;
            ViewBag.Kelompok = kelompok;

            var list = await query.OrderBy(m => m.KodeMapel).ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMapel()
        {
            var model = new MapelFormViewModel
            {
                JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMapel(MapelFormViewModel model)
        {
            if (await _context.Mapel.AnyAsync(m => m.KodeMapel == model.KodeMapel))
            {
                ModelState.AddModelError("KodeMapel", "Kode mata pelajaran sudah digunakan.");
            }

            if (!ModelState.IsValid)
            {
                model.JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", model.IdJurusan);
                return View(model);
            }

            var mapel = new Mapel
            {
                KodeMapel = model.KodeMapel.Trim().ToUpper(),
                NamaMapel = model.NamaMapel.Trim(),
                Kelompok = model.Kelompok,
                IdJurusan = model.Kelompok == "Kejuruan" ? model.IdJurusan : null,
                Deskripsi = model.Deskripsi,
                Status = model.Status
            };

            _context.Mapel.Add(mapel);
            await _context.SaveChangesAsync();

            // Beri bobot default 30%, 30%, 40% otomatis
            var kompHarian = await _context.KomponenNilai.FirstOrDefaultAsync(k => k.NamaKomponen == "Harian");
            var kompUTS = await _context.KomponenNilai.FirstOrDefaultAsync(k => k.NamaKomponen == "UTS");
            var kompUAS = await _context.KomponenNilai.FirstOrDefaultAsync(k => k.NamaKomponen == "UAS");

            if (kompHarian != null && kompUTS != null && kompUAS != null)
            {
                _context.BobotNilai.AddRange(
                    new BobotNilai { IdMapel = mapel.IdMapel, IdKomponen = kompHarian.IdKomponen, Persentase = 30m },
                    new BobotNilai { IdMapel = mapel.IdMapel, IdKomponen = kompUTS.IdKomponen, Persentase = 30m },
                    new BobotNilai { IdMapel = mapel.IdMapel, IdKomponen = kompUAS.IdKomponen, Persentase = 40m }
                );
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Mata pelajaran {mapel.NamaMapel} berhasil ditambahkan.";
            return RedirectToAction(nameof(Mapel));
        }

        [HttpGet]
        public async Task<IActionResult> EditMapel(int id)
        {
            var m = await _context.Mapel.FindAsync(id);
            if (m == null) return NotFound();

            var model = new MapelFormViewModel
            {
                IdMapel = m.IdMapel,
                KodeMapel = m.KodeMapel,
                NamaMapel = m.NamaMapel,
                Kelompok = m.Kelompok,
                IdJurusan = m.IdJurusan,
                Deskripsi = m.Deskripsi,
                Status = m.Status,
                JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", m.IdJurusan)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMapel(int id, MapelFormViewModel model)
        {
            if (id != model.IdMapel) return NotFound();

            if (await _context.Mapel.AnyAsync(m => m.KodeMapel == model.KodeMapel && m.IdMapel != id))
            {
                ModelState.AddModelError("KodeMapel", "Kode mapel sudah digunakan.");
            }

            if (!ModelState.IsValid)
            {
                model.JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", model.IdJurusan);
                return View(model);
            }

            var m = await _context.Mapel.FindAsync(id);
            if (m == null) return NotFound();

            m.KodeMapel = model.KodeMapel.Trim().ToUpper();
            m.NamaMapel = model.NamaMapel.Trim();
            m.Kelompok = model.Kelompok;
            m.IdJurusan = model.Kelompok == "Kejuruan" ? model.IdJurusan : null;
            m.Deskripsi = model.Deskripsi;
            m.Status = model.Status;

            _context.Mapel.Update(m);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Mata pelajaran {m.NamaMapel} berhasil diperbarui.";
            return RedirectToAction(nameof(Mapel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMapel(int id)
        {
            var m = await _context.Mapel.Include(x => x.NilaiList).FirstOrDefaultAsync(x => x.IdMapel == id);
            if (m == null) return NotFound();

            if (m.NilaiList.Any())
            {
                m.Status = "Nonaktif";
                await _context.SaveChangesAsync();
                TempData["WarningMessage"] = $"Mapel {m.NamaMapel} sudah memiliki transaksi nilai, status diubah menjadi Nonaktif (soft delete).";
            }
            else
            {
                var bobots = await _context.BobotNilai.Where(b => b.IdMapel == id).ToListAsync();
                _context.BobotNilai.RemoveRange(bobots);
                _context.Mapel.Remove(m);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Mapel {m.NamaMapel} berhasil dihapus.";
            }

            return RedirectToAction(nameof(Mapel));
        }

        // ==================== FR-05: MASTER DATA KELAS ====================
        public async Task<IActionResult> Kelas()
        {
            var list = await _context.Kelas
                .Include(k => k.Jurusan)
                .Include(k => k.WaliKelas)
                .Include(k => k.SiswaList)
                .OrderBy(k => k.NamaKelas)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateKelas()
        {
            var assignedWaliIds = await _context.Kelas.Where(k => k.IdWaliKelas.HasValue).Select(k => k.IdWaliKelas!.Value).ToListAsync();
            var availableGurus = await _context.Guru.Where(g => !assignedWaliIds.Contains(g.IdGuru) && g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync();

            var model = new KelasFormViewModel
            {
                JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama"),
                GuruList = new SelectList(availableGurus, "IdGuru", "Nama")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKelas(KelasFormViewModel model)
        {
            if (await _context.Kelas.AnyAsync(k => k.NamaKelas == model.NamaKelas))
            {
                ModelState.AddModelError("NamaKelas", "Nama kelas sudah ada.");
            }

            if (model.IdWaliKelas.HasValue)
            {
                if (await _context.Kelas.AnyAsync(k => k.IdWaliKelas == model.IdWaliKelas.Value))
                {
                    ModelState.AddModelError("IdWaliKelas", "Guru ini sudah menjadi wali kelas untuk kelas lain.");
                }
            }

            if (!ModelState.IsValid)
            {
                var assignedWaliIds = await _context.Kelas.Where(k => k.IdWaliKelas.HasValue).Select(k => k.IdWaliKelas!.Value).ToListAsync();
                var availableGurus = await _context.Guru.Where(g => !assignedWaliIds.Contains(g.IdGuru) && g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync();
                model.JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", model.IdJurusan);
                model.GuruList = new SelectList(availableGurus, "IdGuru", "Nama", model.IdWaliKelas);
                return View(model);
            }

            var k = new Kelas
            {
                NamaKelas = model.NamaKelas.Trim(),
                Tingkat = model.Tingkat,
                IdJurusan = model.IdJurusan,
                IdWaliKelas = model.IdWaliKelas,
                Status = model.Status
            };

            _context.Kelas.Add(k);

            // Jika guru ditugaskan sebagai wali kelas, pastikan role Pengguna-nya adalah WaliKelas
            if (model.IdWaliKelas.HasValue)
            {
                var guru = await _context.Guru.Include(g => g.Pengguna).FirstOrDefaultAsync(g => g.IdGuru == model.IdWaliKelas.Value);
                if (guru?.Pengguna != null)
                {
                    var roleWali = await _context.Roles.FirstAsync(r => r.NamaRole == "WaliKelas");
                    guru.Pengguna.IdRole = roleWali.IdRole;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Kelas {k.NamaKelas} berhasil dibuat.";
            return RedirectToAction(nameof(Kelas));
        }

        [HttpGet]
        public async Task<IActionResult> EditKelas(int id)
        {
            var k = await _context.Kelas.FindAsync(id);
            if (k == null) return NotFound();

            var assignedWaliIds = await _context.Kelas.Where(x => x.IdWaliKelas.HasValue && x.IdKelas != id).Select(x => x.IdWaliKelas!.Value).ToListAsync();
            var availableGurus = await _context.Guru.Where(g => !assignedWaliIds.Contains(g.IdGuru) && g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync();

            var model = new KelasFormViewModel
            {
                IdKelas = k.IdKelas,
                NamaKelas = k.NamaKelas,
                Tingkat = k.Tingkat,
                IdJurusan = k.IdJurusan,
                IdWaliKelas = k.IdWaliKelas,
                Status = k.Status,
                JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", k.IdJurusan),
                GuruList = new SelectList(availableGurus, "IdGuru", "Nama", k.IdWaliKelas)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKelas(int id, KelasFormViewModel model)
        {
            if (id != model.IdKelas) return NotFound();

            if (await _context.Kelas.AnyAsync(k => k.NamaKelas == model.NamaKelas && k.IdKelas != id))
            {
                ModelState.AddModelError("NamaKelas", "Nama kelas sudah digunakan.");
            }

            if (model.IdWaliKelas.HasValue)
            {
                if (await _context.Kelas.AnyAsync(k => k.IdWaliKelas == model.IdWaliKelas.Value && k.IdKelas != id))
                {
                    ModelState.AddModelError("IdWaliKelas", "Guru ini sudah menjadi wali kelas di rombel lain.");
                }
            }

            if (!ModelState.IsValid)
            {
                var assignedWaliIds = await _context.Kelas.Where(x => x.IdWaliKelas.HasValue && x.IdKelas != id).Select(x => x.IdWaliKelas!.Value).ToListAsync();
                var availableGurus = await _context.Guru.Where(g => !assignedWaliIds.Contains(g.IdGuru) && g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync();
                model.JurusanList = new SelectList(await _context.Jurusan.OrderBy(j => j.Nama).ToListAsync(), "IdJurusan", "Nama", model.IdJurusan);
                model.GuruList = new SelectList(availableGurus, "IdGuru", "Nama", model.IdWaliKelas);
                return View(model);
            }

            var k = await _context.Kelas.FindAsync(id);
            if (k == null) return NotFound();

            k.NamaKelas = model.NamaKelas.Trim();
            k.Tingkat = model.Tingkat;
            k.IdJurusan = model.IdJurusan;
            k.IdWaliKelas = model.IdWaliKelas;
            k.Status = model.Status;

            if (model.IdWaliKelas.HasValue)
            {
                var guru = await _context.Guru.Include(g => g.Pengguna).FirstOrDefaultAsync(g => g.IdGuru == model.IdWaliKelas.Value);
                if (guru?.Pengguna != null)
                {
                    var roleWali = await _context.Roles.FirstAsync(r => r.NamaRole == "WaliKelas");
                    guru.Pengguna.IdRole = roleWali.IdRole;
                }
            }

            _context.Kelas.Update(k);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Kelas {k.NamaKelas} berhasil diperbarui.";
            return RedirectToAction(nameof(Kelas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKelas(int id)
        {
            var k = await _context.Kelas.Include(x => x.SiswaList).FirstOrDefaultAsync(x => x.IdKelas == id);
            if (k == null) return NotFound();

            if (k.SiswaList.Any())
            {
                TempData["ErrorMessage"] = $"Kelas {k.NamaKelas} tidak dapat dihapus karena masih memiliki {k.SiswaList.Count} siswa terdaftar.";
            }
            else
            {
                _context.Kelas.Remove(k);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Kelas {k.NamaKelas} berhasil dihapus.";
            }

            return RedirectToAction(nameof(Kelas));
        }

        // ==================== BOBOT NILAI (FR-08) ====================
        public async Task<IActionResult> Bobot()
        {
            var mapelList = await _context.Mapel
                .Include(m => m.BobotNilaiList)
                    .ThenInclude(b => b.KomponenNilai)
                .OrderBy(m => m.KodeMapel)
                .ToListAsync();

            var viewModels = new List<BobotMapelViewModel>();
            foreach (var m in mapelList)
            {
                var vm = new BobotMapelViewModel
                {
                    IdMapel = m.IdMapel,
                    KodeMapel = m.KodeMapel,
                    NamaMapel = m.NamaMapel,
                    BobotHarian = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "Harian")?.Persentase ?? 30m,
                    BobotUTS = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "UTS")?.Persentase ?? 30m,
                    BobotUAS = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "UAS")?.Persentase ?? 40m
                };
                viewModels.Add(vm);
            }

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> EditBobot(int idMapel)
        {
            var m = await _context.Mapel
                .Include(x => x.BobotNilaiList)
                    .ThenInclude(b => b.KomponenNilai)
                .FirstOrDefaultAsync(x => x.IdMapel == idMapel);

            if (m == null) return NotFound();

            var vm = new BobotMapelViewModel
            {
                IdMapel = m.IdMapel,
                KodeMapel = m.KodeMapel,
                NamaMapel = m.NamaMapel,
                BobotHarian = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "Harian")?.Persentase ?? 30m,
                BobotUTS = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "UTS")?.Persentase ?? 30m,
                BobotUAS = m.BobotNilaiList.FirstOrDefault(b => b.KomponenNilai?.NamaKomponen == "UAS")?.Persentase ?? 40m
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBobot(BobotMapelViewModel model)
        {
            decimal total = model.BobotHarian + model.BobotUTS + model.BobotUAS;
            if (total != 100m)
            {
                ModelState.AddModelError(string.Empty, $"Total bobot harus tepat 100%. Saat ini: {total}% (Harian {model.BobotHarian}%, UTS {model.BobotUTS}%, UAS {model.BobotUAS}%)");
                return View(model);
            }

            var kompHarian = await _context.KomponenNilai.FirstAsync(k => k.NamaKomponen == "Harian");
            var kompUTS = await _context.KomponenNilai.FirstAsync(k => k.NamaKomponen == "UTS");
            var kompUAS = await _context.KomponenNilai.FirstAsync(k => k.NamaKomponen == "UAS");

            var bobots = await _context.BobotNilai.Where(b => b.IdMapel == model.IdMapel).ToListAsync();

            var bHarian = bobots.FirstOrDefault(b => b.IdKomponen == kompHarian.IdKomponen);
            if (bHarian != null) bHarian.Persentase = model.BobotHarian;
            else _context.BobotNilai.Add(new BobotNilai { IdMapel = model.IdMapel, IdKomponen = kompHarian.IdKomponen, Persentase = model.BobotHarian });

            var bUTS = bobots.FirstOrDefault(b => b.IdKomponen == kompUTS.IdKomponen);
            if (bUTS != null) bUTS.Persentase = model.BobotUTS;
            else _context.BobotNilai.Add(new BobotNilai { IdMapel = model.IdMapel, IdKomponen = kompUTS.IdKomponen, Persentase = model.BobotUTS });

            var bUAS = bobots.FirstOrDefault(b => b.IdKomponen == kompUAS.IdKomponen);
            if (bUAS != null) bUAS.Persentase = model.BobotUAS;
            else _context.BobotNilai.Add(new BobotNilai { IdMapel = model.IdMapel, IdKomponen = kompUAS.IdKomponen, Persentase = model.BobotUAS });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bobot penilaian mapel {model.NamaMapel} berhasil diperbarui (Harian {model.BobotHarian}%, UTS {model.BobotUTS}%, UAS {model.BobotUAS}%).";
            return RedirectToAction(nameof(Bobot));
        }

        // ==================== PENUGASAN MENGAJAR ====================
        public async Task<IActionResult> Penugasan(int? kelasId, int? guruId)
        {
            var query = _context.PenugasanMengajar
                .Include(p => p.Guru)
                .Include(p => p.Mapel)
                .Include(p => p.Kelas)
                .Include(p => p.Periode)
                .AsQueryable();

            if (kelasId.HasValue && kelasId.Value > 0)
                query = query.Where(p => p.IdKelas == kelasId.Value);

            if (guruId.HasValue && guruId.Value > 0)
                query = query.Where(p => p.IdGuru == guruId.Value);

            ViewBag.KelasList = new SelectList(await _context.Kelas.OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", kelasId);
            ViewBag.GuruList = new SelectList(await _context.Guru.OrderBy(g => g.Nama).ToListAsync(), "IdGuru", "Nama", guruId);

            var list = await query.OrderBy(p => p.Kelas!.NamaKelas).ThenBy(p => p.Mapel!.KodeMapel).ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePenugasan()
        {
            var periodeAktif = await _context.PeriodeAkademik.FirstOrDefaultAsync(p => p.Status == "Aktif")
                               ?? await _context.PeriodeAkademik.OrderByDescending(p => p.IdPeriode).FirstOrDefaultAsync();
            var model = new PenugasanFormViewModel
            {
                GuruList = new SelectList(await _context.Guru.Where(g => g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync(), "IdGuru", "Nama"),
                MapelList = new SelectList(await _context.Mapel.Where(m => m.Status == "Aktif").OrderBy(m => m.NamaMapel).ToListAsync(), "IdMapel", "NamaMapel"),
                KelasList = new SelectList(await _context.Kelas.Where(k => k.Status == "Aktif").OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas"),
                IdPeriode = periodeAktif?.IdPeriode ?? 0,
                PeriodeList = new SelectList(await _context.PeriodeAkademik.OrderByDescending(p => p.IdPeriode).ToListAsync(), "IdPeriode", "NamaPeriode", periodeAktif?.IdPeriode)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePenugasan(PenugasanFormViewModel model)
        {
            if (await _context.PenugasanMengajar.AnyAsync(p => p.IdGuru == model.IdGuru && p.IdMapel == model.IdMapel && p.IdKelas == model.IdKelas && p.IdPeriode == model.IdPeriode))
            {
                ModelState.AddModelError(string.Empty, "Penugasan guru ini untuk kelas dan mapel tersebut sudah ada.");
            }

            if (!ModelState.IsValid)
            {
                model.GuruList = new SelectList(await _context.Guru.Where(g => g.Status == "Aktif").OrderBy(g => g.Nama).ToListAsync(), "IdGuru", "Nama", model.IdGuru);
                model.MapelList = new SelectList(await _context.Mapel.Where(m => m.Status == "Aktif").OrderBy(m => m.NamaMapel).ToListAsync(), "IdMapel", "NamaMapel", model.IdMapel);
                model.KelasList = new SelectList(await _context.Kelas.Where(k => k.Status == "Aktif").OrderBy(k => k.NamaKelas).ToListAsync(), "IdKelas", "NamaKelas", model.IdKelas);
                model.PeriodeList = new SelectList(await _context.PeriodeAkademik.OrderByDescending(p => p.IdPeriode).ToListAsync(), "IdPeriode", "NamaPeriode", model.IdPeriode);
                return View(model);
            }

            var penugasan = new PenugasanMengajar
            {
                IdGuru = model.IdGuru,
                IdMapel = model.IdMapel,
                IdKelas = model.IdKelas,
                IdPeriode = model.IdPeriode
            };

            _context.PenugasanMengajar.Add(penugasan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Penugasan mengajar berhasil ditambahkan.";
            return RedirectToAction(nameof(Penugasan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePenugasan(int id)
        {
            var p = await _context.PenugasanMengajar.FindAsync(id);
            if (p != null)
            {
                _context.PenugasanMengajar.Remove(p);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Penugasan berhasil dihapus.";
            }
            return RedirectToAction(nameof(Penugasan));
        }

        // ==================== MANAJEMEN PENGGUNA ====================
        public async Task<IActionResult> Pengguna()
        {
            var list = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                .OrderBy(p => p.Role!.NamaRole)
                .ThenBy(p => p.Nama)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePengguna()
        {
            var model = new PenggunaFormViewModel
            {
                Status = "Aktif"
            };
            await LoadRoleListAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePengguna(PenggunaFormViewModel model)
        {
            model.Username = model.Username?.Trim() ?? string.Empty;
            model.Nama = model.Nama?.Trim() ?? string.Empty;
            model.NipNrg = model.NipNrg?.Trim();

            if (!string.IsNullOrWhiteSpace(model.Username) &&
                await _context.Pengguna.AnyAsync(p => p.Username.ToLower() == model.Username.ToLower()))
            {
                ModelState.AddModelError("Username", "Username sudah digunakan.");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Kata sandi wajib diisi untuk akun baru.");
            }
            else if (model.Password.Length < 8 || model.Password.Length > 100)
            {
                ModelState.AddModelError("Password", "Kata sandi harus terdiri dari 8 sampai 100 karakter.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRole == model.IdRole);
            if (role == null)
            {
                ModelState.AddModelError(nameof(model.IdRole), "Peran yang dipilih tidak valid.");
            }
            else if (RequiresGuruProfile(role.NamaRole) && string.IsNullOrWhiteSpace(model.NipNrg))
            {
                ModelState.AddModelError(nameof(model.NipNrg), "NIP/NRG wajib diisi untuk akun Guru atau Wali Kelas.");
            }

            if (model.Status is not ("Aktif" or "Nonaktif"))
            {
                ModelState.AddModelError(nameof(model.Status), "Status akun tidak valid.");
            }

            if (role != null && RequiresGuruProfile(role.NamaRole) && !string.IsNullOrWhiteSpace(model.NipNrg) &&
                await _context.Guru.AnyAsync(g => g.NipNrg.ToLower() == model.NipNrg.ToLower()))
            {
                ModelState.AddModelError(nameof(model.NipNrg), "NIP/NRG sudah digunakan oleh akun guru lain.");
            }

            if (!ModelState.IsValid)
            {
                await LoadRoleListAsync(model);
                return View(model);
            }

            var pengguna = new Pengguna
            {
                Nama = model.Nama.Trim(),
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password!),
                IdRole = model.IdRole,
                Status = model.Status,
                DibuatPada = DateTime.UtcNow
            };

            _context.Pengguna.Add(pengguna);
            await _context.SaveChangesAsync();

            if (role != null && RequiresGuruProfile(role.NamaRole))
            {
                _context.Guru.Add(new Guru
                {
                    IdPengguna = pengguna.IdPengguna,
                    NipNrg = model.NipNrg!,
                    Nama = pengguna.Nama,
                    Status = pengguna.Status
                });
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Akun {pengguna.Nama} ({pengguna.Username}) berhasil dibuat.";
            return RedirectToAction(nameof(Pengguna));
        }

        [HttpGet]
        public async Task<IActionResult> EditPengguna(int id)
        {
            var pengguna = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                .FirstOrDefaultAsync(p => p.IdPengguna == id);

            if (pengguna == null) return NotFound();

            var model = new PenggunaFormViewModel
            {
                IdPengguna = pengguna.IdPengguna,
                Nama = pengguna.Nama,
                Username = pengguna.Username,
                IdRole = pengguna.IdRole,
                Status = pengguna.Status,
                NipNrg = pengguna.Guru?.NipNrg
            };
            await LoadRoleListAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPengguna(int id, PenggunaFormViewModel model)
        {
            var pengguna = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                .FirstOrDefaultAsync(p => p.IdPengguna == id);

            if (pengguna == null) return NotFound();

            model.IdPengguna = id;
            model.Nama = model.Nama?.Trim() ?? string.Empty;
            model.Username = model.Username?.Trim() ?? string.Empty;
            model.NipNrg = model.NipNrg?.Trim();

            if (!string.IsNullOrWhiteSpace(model.Username) && await _context.Pengguna.AnyAsync(p =>
                    p.IdPengguna != id && p.Username.ToLower() == model.Username.ToLower()))
            {
                ModelState.AddModelError(nameof(model.Username), "Username sudah digunakan oleh akun lain.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRole == model.IdRole);
            if (role == null)
            {
                ModelState.AddModelError(nameof(model.IdRole), "Peran yang dipilih tidak valid.");
            }
            else if (RequiresGuruProfile(role.NamaRole) && string.IsNullOrWhiteSpace(model.NipNrg))
            {
                ModelState.AddModelError(nameof(model.NipNrg), "NIP/NRG wajib diisi untuk akun Guru atau Wali Kelas.");
            }

            if (model.Status is not ("Aktif" or "Nonaktif"))
            {
                ModelState.AddModelError(nameof(model.Status), "Status akun tidak valid.");
            }

            var currentUserId = GetCurrentUserId();
            if (pengguna.IdPengguna == currentUserId && (role?.NamaRole != "Admin" || model.Status != "Aktif"))
            {
                ModelState.AddModelError(string.Empty, "Akun administrator yang sedang digunakan tidak dapat dinonaktifkan atau diubah perannya.");
            }

            var kehilanganAksesAdmin = pengguna.Role?.NamaRole == "Admin" && pengguna.Status == "Aktif" &&
                (role?.NamaRole != "Admin" || model.Status != "Aktif");
            if (kehilanganAksesAdmin && await JumlahAdministratorAktifAsync() <= 1)
            {
                ModelState.AddModelError(string.Empty, "Perubahan ditolak karena harus ada minimal satu administrator aktif.");
            }

            if (role != null && RequiresGuruProfile(role.NamaRole) && !string.IsNullOrWhiteSpace(model.NipNrg) &&
                await _context.Guru.AnyAsync(g => g.NipNrg.ToLower() == model.NipNrg.ToLower() &&
                    (pengguna.Guru == null || g.IdGuru != pengguna.Guru.IdGuru)))
            {
                ModelState.AddModelError(nameof(model.NipNrg), "NIP/NRG sudah digunakan oleh akun guru lain.");
            }

            if (!ModelState.IsValid)
            {
                await LoadRoleListAsync(model);
                return View(model);
            }

            pengguna.Nama = model.Nama;
            pengguna.Username = model.Username;
            pengguna.IdRole = model.IdRole;
            pengguna.Status = model.Status;

            if (pengguna.Guru != null)
            {
                pengguna.Guru.Nama = pengguna.Nama;
                pengguna.Guru.Status = pengguna.Status;
            }

            if (role != null && RequiresGuruProfile(role.NamaRole))
            {
                if (pengguna.Guru == null)
                {
                    pengguna.Guru = new Guru
                    {
                        IdPengguna = pengguna.IdPengguna,
                        NipNrg = model.NipNrg!,
                        Nama = pengguna.Nama,
                        Status = pengguna.Status
                    };
                }
                else
                {
                    pengguna.Guru.NipNrg = model.NipNrg!;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Data akun {pengguna.Username} berhasil diperbarui.";
            return RedirectToAction(nameof(Pengguna));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePengguna(int id)
        {
            var pengguna = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                .FirstOrDefaultAsync(p => p.IdPengguna == id);

            if (pengguna == null) return NotFound();

            if (pengguna.IdPengguna == GetCurrentUserId())
            {
                TempData["ErrorMessage"] = "Akun administrator yang sedang digunakan tidak dapat dihapus.";
                return RedirectToAction(nameof(Pengguna));
            }

            if (pengguna.Role?.NamaRole == "Admin" && pengguna.Status == "Aktif" && await JumlahAdministratorAktifAsync() <= 1)
            {
                TempData["ErrorMessage"] = "Akun administrator aktif terakhir tidak dapat dihapus.";
                return RedirectToAction(nameof(Pengguna));
            }

            if (pengguna.Guru != null)
            {
                var guruId = pengguna.Guru.IdGuru;
                var memilikiRiwayatAkademik = await _context.Kelas.AnyAsync(k => k.IdWaliKelas == guruId) ||
                    await _context.PenugasanMengajar.AnyAsync(p => p.IdGuru == guruId) ||
                    await _context.Nilai.AnyAsync(n => n.IdGuruPenginput == guruId);

                if (memilikiRiwayatAkademik)
                {
                    TempData["ErrorMessage"] = $"Akun {pengguna.Username} memiliki riwayat atau penugasan akademik. Nonaktifkan akun agar data tetap terjaga.";
                    return RedirectToAction(nameof(Pengguna));
                }
            }

            if (await _context.LogPerubahanNilai.AnyAsync(l => l.IdPengguna == id))
            {
                TempData["ErrorMessage"] = $"Akun {pengguna.Username} tercatat pada riwayat perubahan nilai dan tidak dapat dihapus.";
                return RedirectToAction(nameof(Pengguna));
            }

            if (pengguna.Guru != null)
            {
                _context.Guru.Remove(pengguna.Guru);
            }
            _context.Pengguna.Remove(pengguna);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Akun {pengguna.Username} berhasil dihapus.";
            return RedirectToAction(nameof(Pengguna));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusPengguna(int id)
        {
            var pengguna = await _context.Pengguna
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.IdPengguna == id);
            if (pengguna != null)
            {
                if (pengguna.IdPengguna == GetCurrentUserId() && pengguna.Status == "Aktif")
                {
                    TempData["ErrorMessage"] = "Akun administrator yang sedang digunakan tidak dapat dinonaktifkan.";
                    return RedirectToAction(nameof(Pengguna));
                }

                if (pengguna.Role?.NamaRole == "Admin" && pengguna.Status == "Aktif" && await JumlahAdministratorAktifAsync() <= 1)
                {
                    TempData["ErrorMessage"] = "Administrator aktif terakhir tidak dapat dinonaktifkan.";
                    return RedirectToAction(nameof(Pengguna));
                }

                pengguna.Status = pengguna.Status == "Aktif" ? "Nonaktif" : "Aktif";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Status akun {pengguna.Username} diubah menjadi {pengguna.Status}.";
            }
            return RedirectToAction(nameof(Pengguna));
        }

        [HttpGet]
        public async Task<IActionResult> PemulihanKataSandi()
        {
            var users = await _context.Pengguna
                .Include(p => p.Role)
                .OrderBy(p => p.Nama)
                .ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPasswordPengguna(int id)
        {
            var pengguna = await _context.Pengguna.FindAsync(id);
            if (pengguna == null) return NotFound();

            return View(new ResetPasswordViewModel
            {
                IdPengguna = pengguna.IdPengguna,
                Nama = pengguna.Nama,
                Username = pengguna.Username
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordPengguna(ResetPasswordViewModel model)
        {
            var pengguna = await _context.Pengguna.FindAsync(model.IdPengguna);
            if (pengguna == null) return NotFound();

            model.Nama = pengguna.Nama;
            model.Username = pengguna.Username;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            pengguna.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.KataSandiBaru);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Kata sandi akun {pengguna.Username} berhasil diperbarui.";
            return RedirectToAction(nameof(Pengguna));
        }

        private async Task LoadRoleListAsync(PenggunaFormViewModel model)
        {
            var roles = await _context.Roles.OrderBy(r => r.NamaRole).ToListAsync();
            var roleOptions = roles.Select(role => new
            {
                role.IdRole,
                NamaRole = role.NamaRole switch
                {
                    "Admin" => "Administrator",
                    "WaliKelas" => "Wali Kelas",
                    _ => "Guru"
                }
            });
            model.RoleList = new SelectList(roleOptions, "IdRole", "NamaRole", model.IdRole);
        }

        private static bool RequiresGuruProfile(string roleName)
        {
            return roleName is "Guru" or "WaliKelas";
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        }

        private Task<int> JumlahAdministratorAktifAsync()
        {
            return _context.Pengguna.CountAsync(p => p.Status == "Aktif" && p.Role != null && p.Role.NamaRole == "Admin");
        }
    }
}
