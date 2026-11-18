using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Pengguna
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Role wajib dipilih.")]
    public int IdRole { get; set; }

    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter.")]
    public string Nama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username wajib diisi.")]
    [StringLength(100, ErrorMessage = "Username maksimal 100 karakter.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kata sandi wajib diisi.")]
    [StringLength(255, ErrorMessage = "Hash kata sandi maksimal 255 karakter.")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public DateTime DibuatPada { get; set; } = DateTime.Now;

    public Role Role { get; set; } = null!;
    public Guru? Guru { get; set; }
    public ICollection<LogPerubahanNilai> LogPerubahanNilai { get; set; } = new List<LogPerubahanNilai>();
}
