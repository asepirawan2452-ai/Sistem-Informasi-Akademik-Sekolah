using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class PeriodeAkademik
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama periode wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama periode maksimal 100 karakter.")]
    public string NamaPeriode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tahun ajaran wajib diisi.")]
    [StringLength(20, ErrorMessage = "Tahun ajaran maksimal 20 karakter.")]
    public string TahunAjaran { get; set; } = string.Empty;

    [Required(ErrorMessage = "Semester wajib dipilih.")]
    [StringLength(20, ErrorMessage = "Semester maksimal 20 karakter.")]
    public string Semester { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime TanggalMulai { get; set; }

    [DataType(DataType.Date)]
    public DateTime TanggalSelesai { get; set; }

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public ICollection<PenugasanMengajar> PenugasanMengajar { get; set; } = new List<PenugasanMengajar>();
    public ICollection<Nilai> Nilai { get; set; } = new List<Nilai>();
    public ICollection<RekapNilai> RekapNilai { get; set; } = new List<RekapNilai>();
}
