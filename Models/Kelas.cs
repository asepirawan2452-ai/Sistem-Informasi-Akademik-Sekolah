using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Kelas
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama kelas wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama kelas maksimal 50 karakter.")]
    public string NamaKelas { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tingkat wajib dipilih.")]
    [StringLength(5, ErrorMessage = "Tingkat maksimal 5 karakter.")]
    public string Tingkat { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jurusan wajib dipilih.")]
    public int IdJurusan { get; set; }

    [Required(ErrorMessage = "Wali kelas wajib dipilih.")]
    public int IdWaliKelas { get; set; }

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public Jurusan Jurusan { get; set; } = null!;
    public Guru WaliKelas { get; set; } = null!;
    public ICollection<Siswa> Siswa { get; set; } = new List<Siswa>();
    public ICollection<PenugasanMengajar> PenugasanMengajar { get; set; } = new List<PenugasanMengajar>();
}
