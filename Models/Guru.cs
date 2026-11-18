using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Guru
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Akun pengguna wajib dipilih.")]
    public int IdPengguna { get; set; }

    [StringLength(50, ErrorMessage = "NIP/NRG maksimal 50 karakter.")]
    public string? NipNrg { get; set; }

    [Required(ErrorMessage = "Nama guru wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama guru maksimal 100 karakter.")]
    public string Nama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public Pengguna Pengguna { get; set; } = null!;
    public ICollection<Kelas> KelasDiwali { get; set; } = new List<Kelas>();
    public ICollection<PenugasanMengajar> PenugasanMengajar { get; set; } = new List<PenugasanMengajar>();
    public ICollection<Nilai> NilaiDicatat { get; set; } = new List<Nilai>();
}
