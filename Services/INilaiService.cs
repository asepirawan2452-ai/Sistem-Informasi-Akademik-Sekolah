using SistemInformasiNilaiAkademik.Models;
using SistemInformasiNilaiAkademik.ViewModels;

namespace SistemInformasiNilaiAkademik.Services
{
    public interface INilaiService
    {
        decimal HitungNilaiAkhir(decimal harian, decimal uts, decimal uas, decimal bobotHarian = 30m, decimal bobotUTS = 30m, decimal bobotUAS = 40m);
        string KonversiNilaiHuruf(decimal nilaiAkhir);
        Task<RekapNilai> HitungDanSimpanRekapAsync(int idSiswa, int idMapel, int idPeriode);
        Task HitungDanSimpanRekapKelasAsync(int idKelas, int idPeriode);
        Task<RaporViewModel> GetRaporSiswaAsync(int idSiswa, int idPeriode);
        Task<GrafikViewModel> GetGrafikKelasAsync(int idKelas, int idPeriode);
        Task<RekapKelasViewModel> GetRekapKelasAsync(int idKelas, int idPeriode);
    }
}
