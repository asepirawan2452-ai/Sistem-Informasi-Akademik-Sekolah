using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.Models;
using SistemInformasiNilaiAkademik.ViewModels;
using SistemInformasiNilaiAkademik.Services;

namespace SistemInformasiNilaiAkademik.Controllers
{
    [Authorize(Roles = "WaliKelas")]
    public class WaliKelasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INilaiService _nilaiService;

        public WaliKelasController(AppDbContext context, INilaiService nilaiService)
        {
            _context = context;
            _nilaiService = nilaiService;
        }

        private async Task<PeriodeAkademik?> GetPeriodeAktifAsync()
        {
            return await _context.PeriodeAkademik.FirstOrDefaultAsync(p => p.Status == "Aktif")
                   ?? await _context.PeriodeAkademik.OrderByDescending(p => p.IdPeriode).FirstOrDefaultAsync();
        }

        private async Task<(Guru? guru, Kelas? kelas)> GetCurrentWaliInfoAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return (null, null);

            var guru = await _context.Guru
                .Include(g => g.KelasWali)
                .FirstOrDefaultAsync(g => g.IdPengguna == userId);

            if (guru == null) return (null, null);

            var kelas = await _context.Kelas
                .Include(k => k.Jurusan)
                .Include(k => k.SiswaList)
                .FirstOrDefaultAsync(k => k.IdWaliKelas == guru.IdGuru);

            return (guru, kelas);
        }

        // ==================== DASHBOARD WALI KELAS ====================
        public async Task<IActionResult> Index()
        {
            var (guru, kelas) = await GetCurrentWaliInfoAsync();
            if (guru == null || kelas == null)
            {
                TempData["ErrorMessage"] = "Anda belum ditugaskan sebagai wali kelas pada kelas manapun.";
                return View(new WaliKelasDashboardViewModel { NamaWali = guru?.Nama ?? User.Identity?.Name ?? "-" });
            }

            var periodeAktif = await GetPeriodeAktifAsync();
            int idPeriode = periodeAktif?.IdPeriode ?? 0;

            var siswaList = await _context.Siswa
                .Where(s => s.IdKelas == kelas.IdKelas && s.Status == "Aktif")
                .OrderBy(s => s.NamaSiswa)
                .ToListAsync();

            var siswaIds = siswaList.Select(s => s.IdSiswa).ToList();

            var rekapList = await _context.RekapNilai
                .Where(r => r.IdPeriode == idPeriode && siswaIds.Contains(r.IdSiswa))
                .ToListAsync();

            decimal rataRata = rekapList.Any() ? Math.Round(rekapList.Average(r => r.NilaiAkhir), 2) : 0;
            int tuntas = rekapList.Count(r => r.NilaiAkhir >= 75);
            int remidi = rekapList.Count(r => r.NilaiAkhir < 75);

            var distHuruf = new Dictionary<string, int>
            {
                { "A", rekapList.Count(r => r.NilaiHuruf == "A") },
                { "B", rekapList.Count(r => r.NilaiHuruf == "B") },
                { "C", rekapList.Count(r => r.NilaiHuruf == "C") },
                { "D", rekapList.Count(r => r.NilaiHuruf == "D") }
            };

            var vm = new WaliKelasDashboardViewModel
            {
                NamaWali = guru.Nama,
                NamaKelas = kelas.NamaKelas,
                NamaPeriodeAktif = periodeAktif?.NamaPeriode ?? "Belum ada periode aktif",
                IdKelas = kelas.IdKelas,
                TotalSiswa = siswaList.Count,
                TotalMapel = await _context.PenugasanMengajar.Where(p => p.IdKelas == kelas.IdKelas && p.IdPeriode == idPeriode).Select(p => p.IdMapel).Distinct().CountAsync(),
                RataRataKelas = rataRata,
                NilaiTuntasCount = tuntas,
                NilaiRemidiCount = remidi,
                SiswaList = siswaList,
                DistribusiHuruf = distHuruf
            };

            return View(vm);
        }

        // ==================== REKAP KELAS BINAAN ====================
        public async Task<IActionResult> RekapKelas()
        {
            var (guru, kelas) = await GetCurrentWaliInfoAsync();
            if (kelas == null)
            {
                TempData["ErrorMessage"] = "Data kelas binaan tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;

            var model = await _nilaiService.GetRekapKelasAsync(kelas.IdKelas, selectedPeriode);
            return View(model);
        }

        // ==================== FR-10: CETAK RAPOR SISWA ====================
        public async Task<IActionResult> CetakRapor(int idSiswa)
        {
            var (guru, kelas) = await GetCurrentWaliInfoAsync();
            if (kelas == null) return Forbid();

            // Sesuai SRS FR-10: Hanya wali kelas terkait yang dapat mencetak
            var siswa = await _context.Siswa.FindAsync(idSiswa);
            if (siswa == null) return NotFound();

            if (siswa.IdKelas != kelas.IdKelas)
            {
                TempData["ErrorMessage"] = "Otorisasi ditolak: Anda hanya berhak mencetak rapor siswa kelas binaan Anda.";
                return RedirectToAction(nameof(Index));
            }

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;

            var raporModel = await _nilaiService.GetRaporSiswaAsync(idSiswa, selectedPeriode);
            return View(raporModel);
        }

        // ==================== FR-11: GRAFIK KELAS BINAAN ====================
        public async Task<IActionResult> GrafikKelas()
        {
            var (guru, kelas) = await GetCurrentWaliInfoAsync();
            if (kelas == null) return Forbid();

            var periodeAktif = await GetPeriodeAktifAsync();
            int selectedPeriode = periodeAktif?.IdPeriode ?? 0;

            var grafikModel = await _nilaiService.GetGrafikKelasAsync(kelas.IdKelas, selectedPeriode);
            return View(grafikModel);
        }
    }
}
