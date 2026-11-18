using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.Models;
using SistemInformasiNilaiAkademik.ViewModels;

namespace SistemInformasiNilaiAkademik.Services
{
    public class NilaiService : INilaiService
    {
        private readonly AppDbContext _context;

        public NilaiService(AppDbContext context)
        {
            _context = context;
        }

        public decimal HitungNilaiAkhir(decimal harian, decimal uts, decimal uas, decimal bobotHarian = 30m, decimal bobotUTS = 30m, decimal bobotUAS = 40m)
        {
            decimal totalBobot = bobotHarian + bobotUTS + bobotUAS;
            if (totalBobot != 100m)
            {
                throw new InvalidOperationException($"Total bobot harus 100%, saat ini: {totalBobot}%");
            }

            decimal hasil = (harian * (bobotHarian / 100m)) +
                            (uts * (bobotUTS / 100m)) +
                            (uas * (bobotUAS / 100m));

            return Math.Round(hasil, 2, MidpointRounding.AwayFromZero);
        }

        public string KonversiNilaiHuruf(decimal nilaiAkhir)
        {
            // Rentang sesuai SRS FR-09: A 85–100, B 75–<85, C 60–<75, D 0–<60
            if (nilaiAkhir >= 85.00m) return "A";
            if (nilaiAkhir >= 75.00m) return "B";
            if (nilaiAkhir >= 60.00m) return "C";
            return "D";
        }

        public async Task<RekapNilai> HitungDanSimpanRekapAsync(int idSiswa, int idMapel, int idPeriode)
        {
            var bobotList = await _context.BobotNilai
                .Include(b => b.KomponenNilai)
                .Where(b => b.IdMapel == idMapel)
                .ToListAsync();

            decimal bobotHarian = 30m, bobotUTS = 30m, bobotUAS = 40m;
            foreach (var b in bobotList)
            {
                if (b.KomponenNilai != null)
                {
                    if (b.KomponenNilai.NamaKomponen.Equals("Harian", StringComparison.OrdinalIgnoreCase))
                        bobotHarian = b.Persentase;
                    else if (b.KomponenNilai.NamaKomponen.Equals("UTS", StringComparison.OrdinalIgnoreCase))
                        bobotUTS = b.Persentase;
                    else if (b.KomponenNilai.NamaKomponen.Equals("UAS", StringComparison.OrdinalIgnoreCase))
                        bobotUAS = b.Persentase;
                }
            }

            var nilaiRecords = await _context.Nilai
                .Include(n => n.KomponenNilai)
                .Where(n => n.IdSiswa == idSiswa && n.IdMapel == idMapel && n.IdPeriode == idPeriode)
                .ToListAsync();

            var harianRecords = nilaiRecords
                .Where(n => n.KomponenNilai != null && n.KomponenNilai.NamaKomponen.Equals("Harian", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Sesuai kebutuhan tugas harian 1, 2, 3 dst: Rata-rata dari seluruh tugas harian
            decimal harian = harianRecords.Any() ? Math.Round(harianRecords.Average(n => n.NilaiAngka), 2, MidpointRounding.AwayFromZero) : 0m;
            decimal uts = nilaiRecords.FirstOrDefault(n => n.KomponenNilai != null && n.KomponenNilai.NamaKomponen.Equals("UTS", StringComparison.OrdinalIgnoreCase))?.NilaiAngka ?? 0m;
            decimal uas = nilaiRecords.FirstOrDefault(n => n.KomponenNilai != null && n.KomponenNilai.NamaKomponen.Equals("UAS", StringComparison.OrdinalIgnoreCase))?.NilaiAngka ?? 0m;

            decimal akhir = HitungNilaiAkhir(harian, uts, uas, bobotHarian, bobotUTS, bobotUAS);
            string huruf = KonversiNilaiHuruf(akhir);

            var rekap = await _context.RekapNilai
                .FirstOrDefaultAsync(r => r.IdSiswa == idSiswa && r.IdMapel == idMapel && r.IdPeriode == idPeriode);

            if (rekap == null)
            {
                rekap = new RekapNilai
                {
                    IdSiswa = idSiswa,
                    IdMapel = idMapel,
                    IdPeriode = idPeriode,
                    NilaiAkhir = akhir,
                    NilaiHuruf = huruf,
                    TanggalPerhitungan = DateTime.UtcNow,
                    Status = "Final"
                };
                _context.RekapNilai.Add(rekap);
            }
            else
            {
                rekap.NilaiAkhir = akhir;
                rekap.NilaiHuruf = huruf;
                rekap.TanggalPerhitungan = DateTime.UtcNow;
                _context.RekapNilai.Update(rekap);
            }

            await _context.SaveChangesAsync();
            return rekap;
        }

        public async Task HitungDanSimpanRekapKelasAsync(int idKelas, int idPeriode)
        {
            var siswaList = await _context.Siswa
                .Where(s => s.IdKelas == idKelas && s.Status == "Aktif")
                .Select(s => s.IdSiswa)
                .ToListAsync();

            var mapelList = await _context.PenugasanMengajar
                .Where(p => p.IdKelas == idKelas && p.IdPeriode == idPeriode)
                .Select(p => p.IdMapel)
                .Distinct()
                .ToListAsync();

            foreach (var sId in siswaList)
            {
                foreach (var mId in mapelList)
                {
                    await HitungDanSimpanRekapAsync(sId, mId, idPeriode);
                }
            }
        }

        public async Task<RaporViewModel> GetRaporSiswaAsync(int idSiswa, int idPeriode)
        {
            var siswa = await _context.Siswa
                .Include(s => s.Kelas)
                    .ThenInclude(k => k!.Jurusan)
                .Include(s => s.Kelas)
                    .ThenInclude(k => k!.WaliKelas)
                .FirstOrDefaultAsync(s => s.IdSiswa == idSiswa);

            if (siswa == null) throw new ArgumentException("Siswa tidak ditemukan.");

            var periode = await _context.PeriodeAkademik.FindAsync(idPeriode);
            if (periode == null) throw new ArgumentException("Periode akademik tidak ditemukan.");

            // Get all mapel taken in this class
            var assignedMapels = await _context.PenugasanMengajar
                .Include(p => p.Mapel)
                .Where(p => p.IdKelas == siswa.IdKelas && p.IdPeriode == idPeriode)
                .Select(p => p.Mapel!)
                .Distinct()
                .OrderBy(m => m.KodeMapel)
                .ToListAsync();

            // Get grades
            var nilaiList = await _context.Nilai
                .Include(n => n.KomponenNilai)
                .Where(n => n.IdSiswa == idSiswa && n.IdPeriode == idPeriode)
                .ToListAsync();

            // Get or calculate rekap
            var rekapList = await _context.RekapNilai
                .Where(r => r.IdSiswa == idSiswa && r.IdPeriode == idPeriode)
                .ToListAsync();

            var viewModel = new RaporViewModel
            {
                IdSiswa = siswa.IdSiswa,
                NamaSiswa = siswa.NamaSiswa,
                Nis = siswa.Nis,
                Nisn = siswa.Nisn,
                NamaKelas = siswa.Kelas?.NamaKelas ?? "-",
                Tingkat = siswa.Kelas?.Tingkat ?? "-",
                Jurusan = siswa.Kelas?.Jurusan?.Nama ?? "-",
                TahunAjaran = periode.TahunAjaran,
                Semester = periode.Semester,
                NamaWaliKelas = siswa.Kelas?.WaliKelas?.Nama ?? "-",
                NipWaliKelas = siswa.Kelas?.WaliKelas?.NipNrg ?? "-",
                TanggalCetak = DateTime.Today
            };

            int umumNo = 1;
            int kejuruanNo = 1;
            decimal totalAkhir = 0;
            int countMapel = 0;

            foreach (var m in assignedMapels)
            {
                var harianRecords = nilaiList
                    .Where(n => n.IdMapel == m.IdMapel && n.KomponenNilai?.NamaKomponen == "Harian")
                    .OrderBy(n => n.NamaTugas)
                    .ToList();

                decimal harian = harianRecords.Any() ? Math.Round(harianRecords.Average(n => n.NilaiAngka), 2, MidpointRounding.AwayFromZero) : 0;
                var uts = nilaiList.FirstOrDefault(n => n.IdMapel == m.IdMapel && n.KomponenNilai?.NamaKomponen == "UTS")?.NilaiAngka ?? 0;
                var uas = nilaiList.FirstOrDefault(n => n.IdMapel == m.IdMapel && n.KomponenNilai?.NamaKomponen == "UAS")?.NilaiAngka ?? 0;

                string rincianTugas = harianRecords.Any() 
                    ? string.Join(", ", harianRecords.Select(t => $"{t.NamaTugas}: {t.NilaiAngka:0.##}")) 
                    : "-";

                var rekap = rekapList.FirstOrDefault(r => r.IdMapel == m.IdMapel);
                decimal akhir;
                string huruf;

                if (rekap != null)
                {
                    akhir = rekap.NilaiAkhir;
                    huruf = rekap.NilaiHuruf;
                }
                else
                {
                    akhir = HitungNilaiAkhir(harian, uts, uas);
                    huruf = KonversiNilaiHuruf(akhir);
                }

                totalAkhir += akhir;
                countMapel++;

                string ket = huruf switch
                {
                    "A" => "Sangat Baik (Lulus)",
                    "B" => "Baik (Lulus)",
                    "C" => "Cukup (Lulus)",
                    _ => "Kurang (Remidial)"
                };

                var item = new ItemNilaiRapor
                {
                    KodeMapel = m.KodeMapel,
                    NamaMapel = m.NamaMapel,
                    NilaiHarian = harian,
                    JumlahTugas = harianRecords.Count,
                    RincianTugas = rincianTugas,
                    NilaiUTS = uts,
                    NilaiUAS = uas,
                    NilaiAkhir = akhir,
                    NilaiHuruf = huruf,
                    Keterangan = ket
                };

                if (m.Kelompok.Contains("Kejuruan", StringComparison.OrdinalIgnoreCase))
                {
                    item.No = kejuruanNo++;
                    viewModel.NilaiMuatanKejuruan.Add(item);
                }
                else
                {
                    item.No = umumNo++;
                    viewModel.NilaiMuatanUmum.Add(item);
                }
            }

            viewModel.RataRataNilai = countMapel > 0 ? Math.Round(totalAkhir / countMapel, 2) : 0;
            viewModel.PredikatKeseluruhan = KonversiNilaiHuruf(viewModel.RataRataNilai);
            viewModel.CatatanWaliKelas = viewModel.RataRataNilai >= 75
                ? "Pertahankan prestasimu dan terus giat belajar dalam mengasah kompetensi keahlian."
                : "Tingkatkan intensitas belajar dan keaktifan di kelas serta lakukan pengayaan/remidial untuk mapel yang belum tuntas.";

            return viewModel;
        }

        public async Task<GrafikViewModel> GetGrafikKelasAsync(int idKelas, int idPeriode)
        {
            var kelas = await _context.Kelas.FindAsync(idKelas);
            var periode = await _context.PeriodeAkademik.FindAsync(idPeriode);

            var viewModel = new GrafikViewModel
            {
                IdKelas = idKelas,
                NamaKelas = kelas?.NamaKelas ?? "-",
                IdPeriode = idPeriode,
                NamaPeriode = periode?.NamaPeriode ?? "-"
            };

            var siswaIds = await _context.Siswa
                .Where(s => s.IdKelas == idKelas && s.Status == "Aktif")
                .Select(s => s.IdSiswa)
                .ToListAsync();

            viewModel.TotalSiswa = siswaIds.Count;

            var mapelList = await _context.PenugasanMengajar
                .Include(p => p.Mapel)
                .Where(p => p.IdKelas == idKelas && p.IdPeriode == idPeriode)
                .Select(p => p.Mapel!)
                .Distinct()
                .OrderBy(m => m.KodeMapel)
                .ToListAsync();

            var rekapList = await _context.RekapNilai
                .Where(r => r.IdPeriode == idPeriode && siswaIds.Contains(r.IdSiswa))
                .ToListAsync();

            decimal sumAll = 0;
            int totalEntries = 0;
            decimal highestAvg = -1;
            decimal lowestAvg = 101;

            foreach (var m in mapelList)
            {
                var gradesForMapel = rekapList.Where(r => r.IdMapel == m.IdMapel).Select(r => r.NilaiAkhir).ToList();
                if (gradesForMapel.Count > 0)
                {
                    decimal avg = Math.Round(gradesForMapel.Average(), 2);
                    decimal max = gradesForMapel.Max();
                    decimal min = gradesForMapel.Min();

                    viewModel.LabelsMapel.Add(m.NamaMapel.Length > 20 ? m.KodeMapel : m.NamaMapel);
                    viewModel.DataRataRata.Add(avg);
                    viewModel.DataTertinggi.Add(max);
                    viewModel.DataTerendah.Add(min);

                    sumAll += avg;
                    totalEntries++;

                    if (avg > highestAvg)
                    {
                        highestAvg = avg;
                        viewModel.MapelTertinggi = $"{m.NamaMapel} ({avg})";
                    }
                    if (avg < lowestAvg)
                    {
                        lowestAvg = avg;
                        viewModel.MapelTerendah = $"{m.NamaMapel} ({avg})";
                    }
                }
            }

            viewModel.RataRataKeseluruhan = totalEntries > 0 ? Math.Round(sumAll / totalEntries, 2) : 0;
            return viewModel;
        }

        public async Task<RekapKelasViewModel> GetRekapKelasAsync(int idKelas, int idPeriode)
        {
            var kelas = await _context.Kelas
                .Include(k => k.WaliKelas)
                .FirstOrDefaultAsync(k => k.IdKelas == idKelas);

            var periode = await _context.PeriodeAkademik.FindAsync(idPeriode);

            var viewModel = new RekapKelasViewModel
            {
                IdKelas = idKelas,
                NamaKelas = kelas?.NamaKelas ?? "-",
                IdPeriode = idPeriode,
                NamaPeriode = periode?.NamaPeriode ?? "-",
                NamaWaliKelas = kelas?.WaliKelas?.Nama ?? "-"
            };

            viewModel.DaftarMapel = await _context.PenugasanMengajar
                .Include(p => p.Mapel)
                .Where(p => p.IdKelas == idKelas && p.IdPeriode == idPeriode)
                .Select(p => p.Mapel!)
                .Distinct()
                .OrderBy(m => m.KodeMapel)
                .ToListAsync();

            var siswaList = await _context.Siswa
                .Where(s => s.IdKelas == idKelas && s.Status == "Aktif")
                .OrderBy(s => s.NamaSiswa)
                .ToListAsync();

            var siswaIds = siswaList.Select(s => s.IdSiswa).ToList();

            var rekapRecords = await _context.RekapNilai
                .Where(r => r.IdPeriode == idPeriode && siswaIds.Contains(r.IdSiswa))
                .ToListAsync();

            foreach (var s in siswaList)
            {
                var baris = new BarisRekapSiswa
                {
                    IdSiswa = s.IdSiswa,
                    Nis = s.Nis,
                    NamaSiswa = s.NamaSiswa
                };

                decimal total = 0;
                int count = 0;

                foreach (var m in viewModel.DaftarMapel)
                {
                    var r = rekapRecords.FirstOrDefault(x => x.IdSiswa == s.IdSiswa && x.IdMapel == m.IdMapel);
                    if (r != null)
                    {
                        baris.NilaiPerMapel[m.IdMapel] = (r.NilaiAkhir, r.NilaiHuruf);
                        total += r.NilaiAkhir;
                        count++;
                    }
                    else
                    {
                        baris.NilaiPerMapel[m.IdMapel] = (0, "-");
                    }
                }

                baris.RataRata = count > 0 ? Math.Round(total / count, 2) : 0;
                baris.Predikat = KonversiNilaiHuruf(baris.RataRata);
                viewModel.DaftarSiswa.Add(baris);
            }

            return viewModel;
        }
    }
}
