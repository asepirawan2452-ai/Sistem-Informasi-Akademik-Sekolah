using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class BobotNilai
{
    public int Id { get; set; }

    public int IdMapel { get; set; }
    public int IdKomponen { get; set; }

    [Required(ErrorMessage = "Persentase wajib diisi.")]
    [Range(0, 100, ErrorMessage = "Persentase harus antara 0 sampai 100.")]
    public decimal Persentase { get; set; }

    public Mapel Mapel { get; set; } = null!;
    public KomponenNilai KomponenNilai { get; set; } = null!;
}
