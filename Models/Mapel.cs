using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Mapel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kode mata pelajaran wajib diisi.")]
    [StringLength(20, ErrorMessage = "Kode mata pelajaran maksimal 20 karakter.")]
    public string KodeMapel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama mata pelajaran wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama mata pelajaran maksimal 100 karakter.")]
    public string NamaMapel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kelompok wajib dipilih.")]
    [StringLength(20, ErrorMessage = "Kelompok maksimal 20 karakter.")]
    public string Kelompok { get; set; } = string.Empty;

    public int? IdJurusan { get; set; }

    [StringLength(255, ErrorMessage = "Deskripsi maksimal 255 karakter.")]
    public string? Deskripsi { get; set; }

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public Jurusan? Jurusan { get; set; }
    public ICollection<BobotNilai> BobotNilai { get; set; } = new List<BobotNilai>();
    public ICollection<PenugasanMengajar> PenugasanMengajar { get; set; } = new List<PenugasanMengajar>();
    public ICollection<Nilai> Nilai { get; set; } = new List<Nilai>();
    public ICollection<RekapNilai> RekapNilai { get; set; } = new List<RekapNilai>();
}
