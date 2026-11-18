using SistemInformasiNilaiAkademik.Models;
namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalSiswa { get; set; }
        public int TotalGuru { get; set; }
        public int TotalKelas { get; set; }
        public int TotalMapel { get; set; }
        public int TotalPengguna { get; set; }
        public int TotalNilaiTercatat { get; set; }
        public string NamaPeriodeAktif { get; set; } = "Belum ada periode aktif";
        public List<Siswa> SiswaTerbaru { get; set; } = new();
        public List<LogPerubahanNilai> LogTerbaru { get; set; } = new();
        public Dictionary<string, int> DistribusiJurusan { get; set; } = new();
    }

    public class GuruDashboardViewModel
    {
        public string NamaGuru { get; set; } = string.Empty;
        public string Nip { get; set; } = string.Empty;
        public string NamaPeriodeAktif { get; set; } = "Belum ada periode aktif";
        public int TotalKelasDiampu { get; set; }
        public int TotalMapelDiampu { get; set; }
        public int TotalSiswaDiampu { get; set; }
        public int TotalNilaiDiinput { get; set; }
        public List<PenugasanMengajar> PenugasanList { get; set; } = new();
        public List<LogPerubahanNilai> RiwayatKoreksi { get; set; } = new();
    }

    public class WaliKelasDashboardViewModel
    {
        public string NamaWali { get; set; } = string.Empty;
        public string NamaKelas { get; set; } = string.Empty;
        public string NamaPeriodeAktif { get; set; } = "Belum ada periode aktif";
        public int IdKelas { get; set; }
        public int TotalSiswa { get; set; }
        public int TotalMapel { get; set; }
        public decimal RataRataKelas { get; set; }
        public int NilaiTuntasCount { get; set; }
        public int NilaiRemidiCount { get; set; }
        public List<Siswa> SiswaList { get; set; } = new();
        public Dictionary<string, int> DistribusiHuruf { get; set; } = new();
    }
}
