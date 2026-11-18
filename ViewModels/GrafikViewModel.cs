using SistemInformasiNilaiAkademik.Models;
namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class GrafikViewModel
    {
        public int IdKelas { get; set; }
        public string NamaKelas { get; set; } = string.Empty;
        public int IdPeriode { get; set; }
        public string NamaPeriode { get; set; } = string.Empty;

        public List<string> LabelsMapel { get; set; } = new();
        public List<decimal> DataRataRata { get; set; } = new();
        public List<decimal> DataTertinggi { get; set; } = new();
        public List<decimal> DataTerendah { get; set; } = new();

        public decimal RataRataKeseluruhan { get; set; }
        public string MapelTertinggi { get; set; } = "-";
        public string MapelTerendah { get; set; } = "-";
        public int TotalSiswa { get; set; }
    }
}
