using System.ComponentModel.DataAnnotations;
using SistemInformasiNilaiAkademik.Models;

namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class PeriodeAkademikAdminViewModel
    {
        public PeriodeAkademikFormViewModel Form { get; set; } = new();
        public List<PeriodeAkademik> DaftarPeriode { get; set; } = new();
        public PeriodeAkademik? PeriodeAktif { get; set; }
    }

    public class PeriodeAkademikFormViewModel
    {
        [Required(ErrorMessage = "Tahun ajaran wajib diisi.")]
        [RegularExpression(@"^\d{4}/\d{4}$", ErrorMessage = "Gunakan format tahun ajaran, misalnya 2025/2026.")]
        public string TahunAjaran { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih semester.")]
        [RegularExpression("^(Ganjil|Genap)$", ErrorMessage = "Semester harus Ganjil atau Genap.")]
        public string Semester { get; set; } = "Ganjil";

        [Required(ErrorMessage = "Tanggal mulai wajib diisi.")]
        [DataType(DataType.Date)]
        public DateTime TanggalMulai { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Tanggal selesai wajib diisi.")]
        [DataType(DataType.Date)]
        public DateTime TanggalSelesai { get; set; } = DateTime.Today;

        public bool SalinPenugasanPeriodeAktif { get; set; } = true;
    }
}
