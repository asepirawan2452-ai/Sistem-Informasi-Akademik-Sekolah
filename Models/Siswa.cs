using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Siswa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "NIS wajib diisi.")]
    [StringLength(30, ErrorMessage = "NIS maksimal 30 karakter.")]
    public string Nis { get; set; } = string.Empty;

    [Required(ErrorMessage = "NISN wajib diisi.")]
    [StringLength(30, ErrorMessage = "NISN maksimal 30 karakter.")]
    public string Nisn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama siswa wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama siswa maksimal 100 karakter.")]
    public string NamaSiswa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jenis kelamin wajib dipilih.")]
    [StringLength(20, ErrorMessage = "Jenis kelamin maksimal 20 karakter.")]
    public string JenisKelamin { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime TanggalLahir { get; set; }

    [Required(ErrorMessage = "Alamat wajib diisi.")]
    [StringLength(255, ErrorMessage = "Alamat maksimal 255 karakter.")]
    public string Alamat { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kelas wajib dipilih.")]
    public int IdKelas { get; set; }

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public Kelas Kelas { get; set; } = null!;
    public ICollection<Nilai> Nilai { get; set; } = new List<Nilai>();
    public ICollection<RekapNilai> RekapNilai { get; set; } = new List<RekapNilai>();
}
