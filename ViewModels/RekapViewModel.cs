using SistemInformasiNilaiAkademik.Models;
namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class RekapKelasViewModel
    {
        public int IdKelas { get; set; }
        public string NamaKelas { get; set; } = string.Empty;
        public int IdPeriode { get; set; }
        public string NamaPeriode { get; set; } = string.Empty;
        public string NamaWaliKelas { get; set; } = string.Empty;

        public List<Mapel> DaftarMapel { get; set; } = new();
        public List<BarisRekapSiswa> DaftarSiswa { get; set; } = new();
    }

    public class BarisRekapSiswa
    {
        public int IdSiswa { get; set; }
        public string Nis { get; set; } = string.Empty;
        public string NamaSiswa { get; set; } = string.Empty;
        // Key: IdMapel, Value: (NilaiAkhir, NilaiHuruf)
        public Dictionary<int, (decimal Akhir, string Huruf)> NilaiPerMapel { get; set; } = new();
        public decimal RataRata { get; set; }
        public string Predikat { get; set; } = string.Empty;
    }
}
