using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.Models;
using SistemInformasiNilaiAkademik.ViewModels;
using SistemInformasiNilaiAkademik.Services;

namespace SistemInformasiNilaiAkademik.Controllers
{
    [Authorize(Roles = "Guru,WaliKelas")]
    public class GuruController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INilaiService _nilaiService;

        public GuruController(AppDbContext context, INilaiService nilaiService)
        {
            _context = context;
            _nilaiService = nilaiService;
        }

        private int GetCurrentUserId()
        {
            var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(str, out int id);
            return id;
        }

        private async Task<Guru?> GetCurrentGuruAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Guru.FirstOrDefaultAsync(g => g.IdPengguna == userId);
        }

        private async Task<PeriodeAkademik?> GetPeriodeAktifAsync()
        {
            return await _context.PeriodeAkademik.FirstOrDefaultAsync(p => p.Status == "Aktif")
                   ?? await _context.PeriodeAkademik.OrderByDescending(p => p.IdPeriode).FirstOrDefaultAsync();
        }

        // ==================== DASHBOARD GURU ====================
        public async Task<IActionResult> Index()
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null)
            {
                TempData["ErrorMessage"] = "Data profil guru tidak ditemukan.";
                return RedirectToAction("Login", "Account");
            }

            var periodeAktif = await GetPeriodeAktifAsync();
            int idPeriodeAktif = periodeAktif?.IdPeriode ?? 0;

            var penugasan = await _context.PenugasanMengajar
                .Include(p => p.Kelas)
                .Include(p => p.Mapel)
                .Include(p => p.Periode)
                .Where(p => p.IdGuru == guru.IdGuru && p.IdPeriode == idPeriodeAktif)
                .ToListAsync();

            var kelasIds = penugasan.Select(p => p.IdKelas).Distinct().ToList();
            var totalSiswa = await _context.Siswa.CountAsync(s => kelasIds.Contains(s.IdKelas) && s.Status == "Aktif");
            var totalNilai = await _context.Nilai.CountAsync(n => n.IdGuruPenginput == guru.IdGuru && n.IdPeriode == idPeriodeAktif);

            var userId = GetCurrentUserId();
            var riwayat = await _context.LogPerubahanNilai
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Siswa)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Mapel)
                .Where(l => l.IdPengguna == userId && l.Nilai != null && l.Nilai.IdPeriode == idPeriodeAktif)
                .OrderByDescending(l => l.WaktuPerubahan)
                .Take(5)
                .ToListAsync();

            var vm = new GuruDashboardViewModel
            {
                NamaGuru = guru.Nama,
                Nip = guru.NipNrg,
                NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif",
                TotalKelasDiampu = kelasIds.Count,
                TotalMapelDiampu = penugasan.Select(p => p.IdMapel).Distinct().Count(),
                TotalSiswaDiampu = totalSiswa,
                TotalNilaiDiinput = totalNilai,
                PenugasanList = penugasan,
                RiwayatKoreksi = riwayat
            };

            return View(vm);
        }

        // ==================== FR-06: INPUT NILAI (Mendukung Tugas 1, 2, 3...) ====================
        [HttpGet]
        public async Task<IActionResult> InputNilai(int? idKelas, int? idMapel, int? idKomponen, string? namaTugas, string? tugasBaru)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;
            ViewBag.NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif";

            // Input hanya menampilkan penugasan di semester yang diaktifkan administrator.
            var penugasanList = await _context.PenugasanMengajar
                .Include(p => p.Kelas)
                .Include(p => p.Mapel)
                .Where(p => p.IdGuru == guru.IdGuru && p.IdPeriode == selectedPeriode)
                .ToListAsync();

            var distinctKelas = penugasanList.Select(p => p.Kelas!).DistinctBy(k => k.IdKelas).OrderBy(k => k.NamaKelas).ToList();
            int selectedKelas = idKelas ?? (distinctKelas.FirstOrDefault()?.IdKelas ?? 0);

            var availableMapel = penugasanList
                .Where(p => p.IdKelas == selectedKelas)
                .Select(p => p.Mapel!)
                .DistinctBy(m => m.IdMapel)
                .OrderBy(m => m.KodeMapel)
                .ToList();

            int selectedMapel = idMapel ?? (availableMapel.FirstOrDefault()?.IdMapel ?? 0);

            var komponenList = await _context.KomponenNilai.OrderBy(k => k.IdKomponen).ToListAsync();
            int selectedKomponen = idKomponen ?? (komponenList.FirstOrDefault()?.IdKomponen ?? 0);
            var selectedKompObj = komponenList.FirstOrDefault(k => k.IdKomponen == selectedKomponen);

            // Tentukan Nama Tugas (untuk Harian bisa Tugas 1, Tugas 2, dsb. Untuk UTS/UAS bernilai "UTS" / "UAS")
            string taskName = "Tugas 1";
            var existingTasks = new List<string>();

            if (selectedKelas > 0 && selectedMapel > 0 && selectedKomponen > 0)
            {
                if (selectedKompObj != null && selectedKompObj.NamaKomponen.Equals("Harian", StringComparison.OrdinalIgnoreCase))
                {
                    existingTasks = await _context.Nilai
                        .Where(n => n.IdMapel == selectedMapel && n.IdPeriode == selectedPeriode && n.IdKomponen == selectedKomponen && n.Siswa!.IdKelas == selectedKelas)
                        .Select(n => n.NamaTugas)
                        .Distinct()
                        .OrderBy(t => t)
                        .ToListAsync();

                    if (!existingTasks.Any())
                    {
                        existingTasks.Add("Tugas 1");
                    }

                    if (!string.IsNullOrWhiteSpace(tugasBaru))
                    {
                        taskName = tugasBaru.Trim();
                        if (!existingTasks.Contains(taskName))
                        {
                            existingTasks.Add(taskName);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(namaTugas))
                    {
                        taskName = namaTugas.Trim();
                    }
                    else
                    {
                        taskName = existingTasks.First();
                    }
                }
                else
                {
                    taskName = selectedKompObj?.NamaKomponen ?? "Nilai";
                }
            }

            var vm = new InputNilaiFilterViewModel
            {
                IdKelas = selectedKelas,
                IdMapel = selectedMapel,
                IdPeriode = selectedPeriode,
                IdKomponen = selectedKomponen,
                NamaTugas = taskName,
                DaftarTugasAda = existingTasks,
                NamaKelas = distinctKelas.FirstOrDefault(k => k.IdKelas == selectedKelas)?.NamaKelas ?? "-",
                NamaMapel = availableMapel.FirstOrDefault(m => m.IdMapel == selectedMapel)?.NamaMapel ?? "-",
                NamaKomponen = selectedKompObj?.NamaKomponen ?? "-",
                NamaPeriode = periodeAktif?.NamaPeriode ?? "-"
            };

            ViewBag.KelasList = new SelectList(distinctKelas, "IdKelas", "NamaKelas", selectedKelas);
            ViewBag.MapelList = new SelectList(availableMapel, "IdMapel", "NamaMapel", selectedMapel);
            ViewBag.KomponenList = new SelectList(komponenList, "IdKomponen", "NamaKomponen", selectedKomponen);
            if (selectedKelas > 0 && selectedMapel > 0 && selectedKomponen > 0)
            {
                bool isAssigned = penugasanList.Any(p => p.IdKelas == selectedKelas && p.IdMapel == selectedMapel);
                if (!isAssigned)
                {
                    TempData["ErrorMessage"] = "Anda tidak memiliki penugasan untuk mengajar kelas dan mapel ini.";
                    return View(vm);
                }

                var siswaList = await _context.Siswa
                    .Where(s => s.IdKelas == selectedKelas && s.Status == "Aktif")
                    .OrderBy(s => s.NamaSiswa)
                    .ToListAsync();

                // Nilai tersimpan untuk tugas spesifik ini
                var nilaiTersimpan = await _context.Nilai
                    .Where(n => n.IdMapel == selectedMapel && n.IdPeriode == selectedPeriode && n.IdKomponen == selectedKomponen && n.NamaTugas == taskName)
                    .ToListAsync();

                foreach (var s in siswaList)
                {
                    var n = nilaiTersimpan.FirstOrDefault(x => x.IdSiswa == s.IdSiswa);
                    vm.SiswaNilaiList.Add(new SiswaNilaiItem
                    {
                        IdSiswa = s.IdSiswa,
                        Nis = s.Nis,
                        NamaSiswa = s.NamaSiswa,
                        IdNilai = n?.IdNilai,
                        NamaTugas = taskName,
                        NilaiAngka = n?.NilaiAngka,
                        DiubahPada = n?.DiubahPada
                    });
                }

                // Jika komponen Harian, buat rekap komparasi seluruh tugas harian
                if (selectedKompObj != null && selectedKompObj.NamaKomponen.Equals("Harian", StringComparison.OrdinalIgnoreCase))
                {
                    var allHarian = await _context.Nilai
                        .Where(n => n.IdMapel == selectedMapel && n.IdPeriode == selectedPeriode && n.IdKomponen == selectedKomponen && n.Siswa!.IdKelas == selectedKelas)
                        .ToListAsync();

                    foreach (var s in siswaList)
                    {
                        var row = new BarisRekapTugasHarian
                        {
                            IdSiswa = s.IdSiswa,
                            Nis = s.Nis,
                            NamaSiswa = s.NamaSiswa
                        };

                        var siswaHarian = allHarian.Where(x => x.IdSiswa == s.IdSiswa).ToList();
                        foreach (var task in existingTasks)
                        {
                            var match = siswaHarian.FirstOrDefault(x => x.NamaTugas == task);
                            if (match != null)
                            {
                                row.NilaiPerTugas[task] = match.NilaiAngka;
                            }
                        }

                        row.RataRataHarian = siswaHarian.Any() ? Math.Round(siswaHarian.Average(x => x.NilaiAngka), 2) : 0m;
                        vm.RingkasanTugasHarian.Add(row);
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimpanNilai(InputNilaiFilterViewModel model, Dictionary<int, decimal?> nilaiInputs)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            if (periodeAktif == null)
            {
                TempData["ErrorMessage"] = "Belum ada periode akademik. Hubungi Administrator.";
                return RedirectToAction(nameof(InputNilai));
            }

            if (model.IdPeriode != periodeAktif.IdPeriode)
            {
                TempData["ErrorMessage"] = "Semester aktif telah berubah. Muat ulang halaman sebelum menyimpan nilai.";
                return RedirectToAction(nameof(InputNilai), new { idKelas = model.IdKelas, idMapel = model.IdMapel, idKomponen = model.IdKomponen });
            }

            bool isAssigned = await _context.PenugasanMengajar.AnyAsync(p =>
                p.IdGuru == guru.IdGuru && p.IdKelas == model.IdKelas && p.IdMapel == model.IdMapel && p.IdPeriode == periodeAktif.IdPeriode);

            if (!isAssigned)
            {
                TempData["ErrorMessage"] = "Otorisasi ditolak: Anda tidak ditugaskan untuk kelas dan mata pelajaran ini.";
                return RedirectToAction(nameof(InputNilai));
            }

            int countSaved = 0;
            var userId = GetCurrentUserId();
            string taskName = string.IsNullOrWhiteSpace(model.NamaTugas) ? "Tugas 1" : model.NamaTugas.Trim();

            var requestedStudentIds = nilaiInputs.Keys.ToList();
            var validStudentIds = await _context.Siswa
                .Where(s => requestedStudentIds.Contains(s.IdSiswa) && s.IdKelas == model.IdKelas && s.Status == "Aktif")
                .Select(s => s.IdSiswa)
                .ToListAsync();
            var validStudentIdSet = validStudentIds.ToHashSet();

            foreach (var kvp in nilaiInputs)
            {
                int idSiswa = kvp.Key;
                decimal? angka = kvp.Value;

                if (!angka.HasValue || !validStudentIdSet.Contains(idSiswa)) continue;

                // Sesuai SRS FR-06 & NFR-02: Validasi nilai berada di rentang 0-100
                if (angka.Value < 0 || angka.Value > 100)
                {
                    TempData["ErrorMessage"] = $"Gagal menyimpan: Nilai {angka.Value} berada di luar rentang valid 0–100.";
                    return RedirectToAction(nameof(InputNilai), new { idKelas = model.IdKelas, idMapel = model.IdMapel, idKomponen = model.IdKomponen, idPeriode = model.IdPeriode, namaTugas = taskName });
                }

                var existing = await _context.Nilai.FirstOrDefaultAsync(n =>
                    n.IdSiswa == idSiswa &&
                    n.IdMapel == model.IdMapel &&
                    n.IdPeriode == periodeAktif.IdPeriode &&
                    n.IdKomponen == model.IdKomponen &&
                    n.NamaTugas == taskName);

                if (existing == null)
                {
                    var newNilai = new Nilai
                    {
                        IdSiswa = idSiswa,
                        IdMapel = model.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        IdKomponen = model.IdKomponen,
                        NamaTugas = taskName,
                        NilaiAngka = angka.Value,
                        TanggalPenilaian = DateTime.UtcNow,
                        IdGuruPenginput = guru.IdGuru
                    };
                    _context.Nilai.Add(newNilai);
                }
                else
                {
                    if (existing.NilaiAngka != angka.Value)
                    {
                        _context.LogPerubahanNilai.Add(new LogPerubahanNilai
                        {
                            IdNilai = existing.IdNilai,
                            NilaiLama = existing.NilaiAngka,
                            NilaiBaru = angka.Value,
                            IdPengguna = userId,
                            WaktuPerubahan = DateTime.UtcNow,
                            Alasan = $"Pembaruan nilai {taskName} melalui form input"
                        });

                        existing.NilaiAngka = angka.Value;
                        existing.DiubahPada = DateTime.UtcNow;
                        existing.IdGuruPenginput = guru.IdGuru;
                        _context.Nilai.Update(existing);
                    }
                }
                countSaved++;
            }

            await _context.SaveChangesAsync();

            // Hitung ulang rekap nilai otomatis di background/service
            foreach (var sId in nilaiInputs.Keys)
            {
                if (validStudentIdSet.Contains(sId))
                    await _nilaiService.HitungDanSimpanRekapAsync(sId, model.IdMapel, periodeAktif.IdPeriode);
            }

            TempData["SuccessMessage"] = $"Berhasil menyimpan {countSaved} nilai ({taskName}) dan memperbarui rekap nilai akhir.";
            return RedirectToAction(nameof(InputNilai), new { idKelas = model.IdKelas, idMapel = model.IdMapel, idKomponen = model.IdKomponen, namaTugas = taskName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HapusTugas(int idKelas, int idMapel, int idPeriode, int idKomponen, string namaTugas)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            if (periodeAktif == null || idPeriode != periodeAktif.IdPeriode)
            {
                TempData["ErrorMessage"] = "Semester aktif telah berubah. Muat ulang halaman sebelum menghapus tugas.";
                return RedirectToAction(nameof(InputNilai), new { idKelas, idMapel, idKomponen });
            }

            var isAssigned = await _context.PenugasanMengajar.AnyAsync(p =>
                p.IdGuru == guru.IdGuru && p.IdKelas == idKelas && p.IdMapel == idMapel && p.IdPeriode == periodeAktif.IdPeriode);
            if (!isAssigned) return Forbid();

            var records = await _context.Nilai
                .Where(n => n.IdMapel == idMapel && n.IdPeriode == periodeAktif.IdPeriode && n.IdKomponen == idKomponen && n.NamaTugas == namaTugas && n.Siswa!.IdKelas == idKelas)
                .ToListAsync();

            if (records.Any())
            {
                var siswaIds = records.Select(r => r.IdSiswa).Distinct().ToList();
                _context.Nilai.RemoveRange(records);
                await _context.SaveChangesAsync();

                foreach (var sId in siswaIds)
                {
                    await _nilaiService.HitungDanSimpanRekapAsync(sId, idMapel, periodeAktif.IdPeriode);
                }

                TempData["SuccessMessage"] = $"Tugas '{namaTugas}' berhasil dihapus.";
            }

            return RedirectToAction(nameof(InputNilai), new { idKelas, idMapel, idKomponen });
        }

        // ==================== FR-07: PERUBAHAN / KOREKSI NILAI ====================
        [HttpGet]
        public async Task<IActionResult> KoreksiNilai(int id)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            var nilai = await _context.Nilai
                .Include(n => n.Siswa)
                .Include(n => n.Mapel)
                .Include(n => n.KomponenNilai)
                .FirstOrDefaultAsync(n => n.IdNilai == id);

            if (nilai == null) return NotFound();
            if (periodeAktif == null || nilai.IdPeriode != periodeAktif.IdPeriode || nilai.Siswa == null)
                return Forbid();

            var isAssigned = await _context.PenugasanMengajar.AnyAsync(p =>
                p.IdGuru == guru.IdGuru && p.IdKelas == nilai.Siswa.IdKelas &&
                p.IdMapel == nilai.IdMapel && p.IdPeriode == periodeAktif.IdPeriode);
            if (!isAssigned) return Forbid();

            var vm = new KoreksiNilaiViewModel
            {
                IdNilai = nilai.IdNilai,
                NamaSiswa = nilai.Siswa?.NamaSiswa ?? "-",
                NamaMapel = nilai.Mapel?.NamaMapel ?? "-",
                NamaKomponen = nilai.KomponenNilai?.NamaKomponen ?? "-",
                NamaTugas = nilai.NamaTugas,
                NilaiLama = nilai.NilaiAngka,
                NilaiBaru = nilai.NilaiAngka
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KoreksiNilai(KoreksiNilaiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NilaiBaru < 0 || model.NilaiBaru > 100)
            {
                ModelState.AddModelError("NilaiBaru", "Nilai harus berada pada rentang 0–100.");
                return View(model);
            }

            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            var nilai = await _context.Nilai
                .Include(n => n.Siswa)
                .FirstOrDefaultAsync(n => n.IdNilai == model.IdNilai);
            if (nilai == null) return NotFound();
            if (periodeAktif == null || nilai.IdPeriode != periodeAktif.IdPeriode || nilai.Siswa == null)
                return Forbid();

            var isAssigned = await _context.PenugasanMengajar.AnyAsync(p =>
                p.IdGuru == guru.IdGuru && p.IdKelas == nilai.Siswa.IdKelas &&
                p.IdMapel == nilai.IdMapel && p.IdPeriode == periodeAktif.IdPeriode);
            if (!isAssigned) return Forbid();

            var userId = GetCurrentUserId();

            var log = new LogPerubahanNilai
            {
                IdNilai = nilai.IdNilai,
                NilaiLama = nilai.NilaiAngka,
                NilaiBaru = model.NilaiBaru,
                IdPengguna = userId,
                WaktuPerubahan = DateTime.UtcNow,
                Alasan = model.Alasan.Trim()
            };

            nilai.NilaiAngka = model.NilaiBaru;
            nilai.DiubahPada = DateTime.UtcNow;

            _context.LogPerubahanNilai.Add(log);
            _context.Nilai.Update(nilai);
            await _context.SaveChangesAsync();

            // Hitung ulang Rekap
            await _nilaiService.HitungDanSimpanRekapAsync(nilai.IdSiswa, nilai.IdMapel, nilai.IdPeriode);

            TempData["SuccessMessage"] = $"Perubahan nilai ({nilai.NamaTugas}) berhasil disimpan dan tercatat pada riwayat audit.";
            return RedirectToAction(nameof(RiwayatPerubahan));
        }

        public async Task<IActionResult> RiwayatPerubahan()
        {
            var userId = GetCurrentUserId();
            var periodeAktif = await GetPeriodeAktifAsync();
            int idPeriodeAktif = periodeAktif?.IdPeriode ?? 0;
            ViewBag.NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif";
            var list = await _context.LogPerubahanNilai
                .Include(l => l.Pengguna)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Siswa)
                        .ThenInclude(s => s!.Kelas)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.Mapel)
                .Include(l => l.Nilai)
                    .ThenInclude(n => n!.KomponenNilai)
                .Where(l => l.IdPengguna == userId && l.Nilai != null && l.Nilai.IdPeriode == idPeriodeAktif)
                .OrderByDescending(l => l.WaktuPerubahan)
                .ToListAsync();

            return View(list);
        }

        // ==================== REKAP NILAI GURU (Daftar Tugas Harian Terperinci) ====================
        public async Task<IActionResult> RekapNilai(int? idKelas, int? idMapel)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;
            ViewBag.NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif";

            var penugasanList = await _context.PenugasanMengajar
                .Include(p => p.Kelas)
                .Include(p => p.Mapel)
                .Where(p => p.IdGuru == guru.IdGuru && p.IdPeriode == selectedPeriode)
                .ToListAsync();

            var distinctKelas = penugasanList.Select(p => p.Kelas!).DistinctBy(k => k.IdKelas).OrderBy(k => k.NamaKelas).ToList();
            int selectedKelas = idKelas ?? (distinctKelas.FirstOrDefault()?.IdKelas ?? 0);

            var availableMapel = penugasanList
                .Where(p => p.IdKelas == selectedKelas)
                .Select(p => p.Mapel!)
                .DistinctBy(m => m.IdMapel)
                .OrderBy(m => m.KodeMapel)
                .ToList();

            int selectedMapel = idMapel ?? (availableMapel.FirstOrDefault()?.IdMapel ?? 0);

            ViewBag.KelasList = new SelectList(distinctKelas, "IdKelas", "NamaKelas", selectedKelas);
            ViewBag.MapelList = new SelectList(availableMapel, "IdMapel", "NamaMapel", selectedMapel);

            var siswaList = await _context.Siswa
                .Where(s => s.IdKelas == selectedKelas && s.Status == "Aktif")
                .OrderBy(s => s.NamaSiswa)
                .ToListAsync();

            var nilaiRecords = await _context.Nilai
                .Include(n => n.KomponenNilai)
                .Where(n => n.IdMapel == selectedMapel && n.IdPeriode == selectedPeriode)
                .ToListAsync();

            var rekapRecords = await _context.RekapNilai
                .Where(r => r.IdMapel == selectedMapel && r.IdPeriode == selectedPeriode)
                .ToListAsync();

            // Cari seluruh daftar tugas harian yang pernah diinput untuk mapel ini
            var daftarNamaTugas = nilaiRecords
                .Where(n => n.KomponenNilai?.NamaKomponen == "Harian")
                .Select(n => n.NamaTugas)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            ViewBag.SiswaList = siswaList;
            ViewBag.NilaiRecords = nilaiRecords;
            ViewBag.RekapRecords = rekapRecords;
            ViewBag.DaftarNamaTugas = daftarNamaTugas;

            return View();
        }

        // ==================== FR-11: GRAFIK RATA-RATA KELAS ====================
        public async Task<IActionResult> Grafik(int? idKelas)
        {
            var guru = await GetCurrentGuruAsync();
            if (guru == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;

            var penugasanList = await _context.PenugasanMengajar
                .Include(p => p.Kelas)
                .Where(p => p.IdGuru == guru.IdGuru && p.IdPeriode == selectedPeriode)
                .ToListAsync();

            var distinctKelas = penugasanList.Select(p => p.Kelas!).DistinctBy(k => k.IdKelas).OrderBy(k => k.NamaKelas).ToList();
            int selectedKelas = idKelas ?? (distinctKelas.FirstOrDefault()?.IdKelas ?? 0);

            ViewBag.KelasList = new SelectList(distinctKelas, "IdKelas", "NamaKelas", selectedKelas);

            var grafikModel = await _nilaiService.GetGrafikKelasAsync(selectedKelas, selectedPeriode);
            return View(grafikModel);
        }
    }
}
