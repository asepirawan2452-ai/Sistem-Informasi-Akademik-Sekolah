using SistemInformasiNilaiAkademik.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class InputNilaiFilterViewModel
    {
        [Required(ErrorMessage = "Pilih kelas")]
        public int IdKelas { get; set; }

        [Required(ErrorMessage = "Pilih mata pelajaran")]
        public int IdMapel { get; set; }

        [Required(ErrorMessage = "Pilih periode")]
        public int IdPeriode { get; set; }

        [Required(ErrorMessage = "Pilih komponen penilaian")]
        public int IdKomponen { get; set; }

        public string NamaTugas { get; set; } = "Tugas 1";

        public string? NamaKelas { get; set; }
        public string? NamaMapel { get; set; }
        public string? NamaPeriode { get; set; }
        public string? NamaKomponen { get; set; }

        // Daftar tugas harian yang sudah ada untuk mapel & kelas ini (misal: Tugas 1, Tugas 2, Kuis 1)
        public List<string> DaftarTugasAda { get; set; } = new();

        public List<SiswaNilaiItem> SiswaNilaiList { get; set; } = new();

        // Ringkasan komparasi seluruh tugas harian (Tugas 1, 2, 3...) & Rata-rata Harian
        public List<BarisRekapTugasHarian> RingkasanTugasHarian { get; set; } = new();
    }

    public class SiswaNilaiItem
    {
        public int IdSiswa { get; set; }
        public string Nis { get; set; } = string.Empty;
        public string NamaSiswa { get; set; } = string.Empty;
        public int? IdNilai { get; set; }
        public string NamaTugas { get; set; } = "Tugas 1";

        [Range(0, 100, ErrorMessage = "Nilai harus berada pada rentang 0–100")]
        public decimal? NilaiAngka { get; set; }
        public DateTime? DiubahPada { get; set; }
    }

    public class BarisRekapTugasHarian
    {
        public int IdSiswa { get; set; }
        public string Nis { get; set; } = string.Empty;
        public string NamaSiswa { get; set; } = string.Empty;
        public Dictionary<string, decimal> NilaiPerTugas { get; set; } = new();
        public decimal RataRataHarian { get; set; }
    }

    public class KoreksiNilaiViewModel
    {
        [Required]
        public int IdNilai { get; set; }

        public string NamaSiswa { get; set; } = string.Empty;
        public string NamaMapel { get; set; } = string.Empty;
        public string NamaKomponen { get; set; } = string.Empty;
        public string NamaTugas { get; set; } = string.Empty;

        public decimal NilaiLama { get; set; }

        [Required(ErrorMessage = "Nilai baru wajib diisi")]
        [Range(0, 100, ErrorMessage = "Nilai baru harus 0–100")]
        public decimal NilaiBaru { get; set; }

        [Required(ErrorMessage = "Alasan perubahan wajib diisi")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Alasan minimal 5 karakter")]
        public string Alasan { get; set; } = string.Empty;
    }
}
