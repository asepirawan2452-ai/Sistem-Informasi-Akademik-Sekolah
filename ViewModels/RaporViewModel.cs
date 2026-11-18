using SistemInformasiNilaiAkademik.Models;
namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class RaporViewModel
    {
        // Info Siswa & Sekolah
        public int IdSiswa { get; set; }
        public string NamaSiswa { get; set; } = string.Empty;
        public string Nis { get; set; } = string.Empty;
        public string Nisn { get; set; } = string.Empty;
        public string NamaKelas { get; set; } = string.Empty;
        public string Tingkat { get; set; } = string.Empty;
        public string Jurusan { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;

        public string NamaWaliKelas { get; set; } = string.Empty;
        public string NipWaliKelas { get; set; } = string.Empty;
        public string NamaKepalaSekolah { get; set; } = "Drs. H. Mulyono, M.Pd.";
        public string NipKepalaSekolah { get; set; } = "196805121994031005";
        public DateTime TanggalCetak { get; set; } = DateTime.Today;

        public List<ItemNilaiRapor> NilaiMuatanUmum { get; set; } = new();
        public List<ItemNilaiRapor> NilaiMuatanKejuruan { get; set; } = new();

        public decimal RataRataNilai { get; set; }
        public string PredikatKeseluruhan { get; set; } = string.Empty;
        public string CatatanWaliKelas { get; set; } = string.Empty;
    }

    public class ItemNilaiRapor
    {
        public int No { get; set; }
        public string KodeMapel { get; set; } = string.Empty;
        public string NamaMapel { get; set; } = string.Empty;
        public decimal NilaiHarian { get; set; }
        public int JumlahTugas { get; set; } = 0;
        public string RincianTugas { get; set; } = string.Empty;
        public decimal NilaiUTS { get; set; }
        public decimal NilaiUAS { get; set; }
        public decimal NilaiAkhir { get; set; }
        public string NilaiHuruf { get; set; } = string.Empty;
        public string Keterangan { get; set; } = string.Empty;
    }
}
