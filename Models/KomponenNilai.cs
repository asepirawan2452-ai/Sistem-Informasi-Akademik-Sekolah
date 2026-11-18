using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class KomponenNilai
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama komponen wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama komponen maksimal 50 karakter.")]
    public string NamaKomponen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jenis penilaian wajib diisi.")]
    [StringLength(20, ErrorMessage = "Jenis penilaian maksimal 20 karakter.")]
    public string JenisPenilaian { get; set; } = string.Empty;

    public ICollection<BobotNilai> BobotNilai { get; set; } = new List<BobotNilai>();
    public ICollection<Nilai> Nilai { get; set; } = new List<Nilai>();
}
