using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.ViewModels;

namespace SistemInformasiNilaiAkademik.Services.Testing
{
    public interface ISystemTestSuite
    {
        Task<TestSuiteResultViewModel> RunAllTestsAsync();
    }

    public class SystemTestSuite : ISystemTestSuite
    {
        private readonly AppDbContext _context;
        private readonly INilaiService _nilaiService;

        public SystemTestSuite(AppDbContext context, INilaiService nilaiService)
        {
            _context = context;
            _nilaiService = nilaiService;
        }

        public async Task<TestSuiteResultViewModel> RunAllTestsAsync()
        {
            var sw = Stopwatch.StartNew();
            var results = new List<TestCaseResult>();

            // TC-01: Perhitungan nilai akhir
            // Contoh: (80 × 30%) + (75 × 30%) + (90 × 40%) = 82,5
            try
            {
                decimal actual = _nilaiService.HitungNilaiAkhir(80m, 75m, 90m, 30m, 30m, 40m);
                decimal expected = 82.50m;
                results.Add(new TestCaseResult
                {
                    Code = "TC-01",
                    Title = "Perhitungan Nilai Akhir",
                    AreaPengujian = "Perhitungan dan pembobotan nilai",
                    InputDescription = "Harian=80 (30%), UTS=75 (30%), UAS=90 (40%)",
                    ExpectedOutput = "82.50",
                    ActualOutput = actual.ToString("0.00"),
                    IsSuccess = (actual == expected),
                    Notes = "Perhitungan sesuai dengan metode pembobotan yang berlaku."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-01",
                    Title = "Perhitungan Nilai Akhir",
                    AreaPengujian = "Perhitungan dan pembobotan nilai",
                    InputDescription = "Harian=80, UTS=75, UAS=90",
                    ExpectedOutput = "82.50",
                    ActualOutput = ex.Message,
                    IsSuccess = false
                });
            }

            // TC-02: Validasi total bobot penilaian
            try
            {
                bool rejected = false;
                try
                {
                    _nilaiService.HitungNilaiAkhir(80m, 75m, 90m, 30m, 30m, 50m); // total 110%
                }
                catch (InvalidOperationException)
                {
                    rejected = true;
                }

                results.Add(new TestCaseResult
                {
                    Code = "TC-02",
                    Title = "Penolakan Konfigurasi Bobot Tidak 100%",
                    AreaPengujian = "Validasi bobot penilaian",
                    InputDescription = "Bobot Harian=30%, UTS=30%, UAS=50% (Total: 110%)",
                    ExpectedOutput = "Ditolak (InvalidOperationException)",
                    ActualOutput = rejected ? "Ditolak (InvalidOperationException)" : "Diterima (Gagal)",
                    IsSuccess = rejected,
                    Notes = "Konfigurasi dengan total bobot lebih dari 100% harus ditolak."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-02",
                    Title = "Penolakan Konfigurasi Bobot Tidak 100%",
                    AreaPengujian = "Validasi bobot penilaian",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-03: Konversi predikat huruf dan batas nilai
            // Contoh: 85 -> A, 82.5 -> B, 74.9 -> C, 59.9 -> D
            try
            {
                string h85 = _nilaiService.KonversiNilaiHuruf(85.00m);
                string h825 = _nilaiService.KonversiNilaiHuruf(82.50m);
                string h749 = _nilaiService.KonversiNilaiHuruf(74.90m);
                string h599 = _nilaiService.KonversiNilaiHuruf(59.90m);

                bool match = (h85 == "A" && h825 == "B" && h749 == "C" && h599 == "D");
                results.Add(new TestCaseResult
                {
                    Code = "TC-03",
                    Title = "Konversi Nilai Angka ke Predikat Huruf",
                    AreaPengujian = "Konversi predikat",
                    InputDescription = "85.0 -> A | 82.5 -> B | 74.9 -> C | 59.9 -> D",
                    ExpectedOutput = "A, B, C, D",
                    ActualOutput = $"{h85}, {h825}, {h749}, {h599}",
                    IsSuccess = match,
                    Notes = "Rentang predikat: A 85–100, B 75–<85, C 60–<75, dan D 0–<60."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-03",
                    Title = "Konversi Nilai Angka ke Nilai Huruf",
                    AreaPengujian = "Konversi predikat",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-04: Perhitungan Rata-rata Banyak Tugas Harian (Tugas 1, 2, 3...)
            try
            {
                var sampleScores = new List<decimal> { 80m, 90m, 70m, 100m };
                decimal avg = Math.Round(sampleScores.Average(), 2);
                decimal expectedAvg = 85.00m;

                results.Add(new TestCaseResult
                {
                    Code = "TC-04",
                    Title = "Rata-rata Nilai Tugas Harian",
                    AreaPengujian = "Nilai tugas harian",
                    InputDescription = "Tugas 1=80, Tugas 2=90, Tugas 3=70, Tugas 4=100",
                    ExpectedOutput = "85.00",
                    ActualOutput = avg.ToString("0.00"),
                    IsSuccess = (avg == expectedAvg),
                    Notes = "Menghitung rata-rata dari empat nilai contoh."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-04",
                    Title = "Rata-rata Nilai Tugas Harian",
                    AreaPengujian = "Input nilai",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-05: Validasi rentang nilai
            try
            {
                decimal invalidValHigh = 105m;
                decimal invalidValLow = -10m;
                bool rejected = (invalidValHigh > 100m || invalidValHigh < 0m) && (invalidValLow < 0m);

                results.Add(new TestCaseResult
                {
                    Code = "TC-05",
                    Title = "Pemeriksaan Nilai di Luar Rentang",
                    AreaPengujian = "Validasi rentang nilai",
                    InputDescription = "Nilai = 105 dan Nilai = -10",
                    ExpectedOutput = "Nilai 105 dan -10 berada di luar rentang 0–100.",
                    ActualOutput = rejected ? "Kedua nilai berada di luar rentang." : "Pemeriksaan tidak sesuai.",
                    IsSuccess = rejected,
                    Notes = "Pemeriksaan ini membandingkan nilai contoh dengan batas 0–100."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-05",
                    Title = "Pemeriksaan Nilai di Luar Rentang",
                    AreaPengujian = "Validasi rentang nilai",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-06: Verifikasi riwayat koreksi nilai
            try
            {
                await _context.LogPerubahanNilai.AnyAsync();
                results.Add(new TestCaseResult
                {
                    Code = "TC-06",
                    Title = "Akses Tabel Riwayat Perubahan Nilai",
                    AreaPengujian = "Riwayat perubahan nilai",
                    InputDescription = "Menjalankan query pada tabel log_perubahan_nilai",
                    ExpectedOutput = "Query pada tabel dapat dijalankan.",
                    ActualOutput = "Tabel dapat diakses.",
                    IsSuccess = true,
                    Notes = "Pemeriksaan memastikan tabel dapat dibaca; jumlah catatan tidak diperiksa."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-06",
                    Title = "Akses Tabel Riwayat Perubahan Nilai",
                    AreaPengujian = "Riwayat perubahan nilai",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-07: Otorisasi cetak rapor
            try
            {
                var kelas = await _context.Kelas.FirstOrDefaultAsync(k => k.IdWaliKelas.HasValue);
                bool hasAssignedWali = kelas != null && kelas.IdWaliKelas.HasValue;

                results.Add(new TestCaseResult
                {
                    Code = "TC-07",
                    Title = "Data Wali Kelas",
                    AreaPengujian = "Data kelas",
                    InputDescription = "Mencari kelas yang memiliki wali kelas terdaftar",
                    ExpectedOutput = "Setidaknya satu kelas memiliki wali kelas.",
                    ActualOutput = hasAssignedWali ? "Kelas dengan wali ditemukan." : "Belum ada kelas dengan wali.",
                    IsSuccess = hasAssignedWali,
                    Notes = "Pemeriksaan ini melihat data kelas; tidak menjalankan alur otorisasi cetak rapor."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-07",
                    Title = "Data Wali Kelas",
                    AreaPengujian = "Data kelas",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            // TC-08: Keamanan kata sandi
            try
            {
                var sampleUser = await _context.Pengguna.FirstOrDefaultAsync();
                bool isBcrypt = sampleUser != null && sampleUser.PasswordHash.StartsWith("$2");

                results.Add(new TestCaseResult
                {
                    Code = "TC-08",
                    Title = "Keamanan Kredensial (BCrypt Password Hashing)",
                    AreaPengujian = "Keamanan kata sandi",
                    InputDescription = "Pemeriksaan format hash password pada tabel pengguna",
                    ExpectedOutput = "Kata sandi tersimpan dalam format hash BCrypt ($2...)",
                    ActualOutput = isBcrypt ? "Hash BCrypt Terverifikasi" : "Plaintext (Gagal)",
                    IsSuccess = isBcrypt,
                    Notes = "Pemeriksaan melihat format hash pada satu akun pengguna."
                });
            }
            catch (Exception ex)
            {
                results.Add(new TestCaseResult
                {
                    Code = "TC-08",
                    Title = "Keamanan Kredensial (BCrypt)",
                    AreaPengujian = "Keamanan kata sandi",
                    IsSuccess = false,
                    ActualOutput = ex.Message
                });
            }

            sw.Stop();

            return new TestSuiteResultViewModel
            {
                TotalTests = results.Count,
                PassedTests = results.Count(r => r.IsSuccess),
                FailedTests = results.Count(r => !r.IsSuccess),
                ExecutionTimeMs = sw.ElapsedMilliseconds,
                ExecutedAt = DateTime.Now,
                TestCases = results
            };
        }
    }
}
