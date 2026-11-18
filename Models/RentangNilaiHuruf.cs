using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class RentangNilaiHuruf
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Huruf wajib diisi.")]
    [StringLength(5, ErrorMessage = "Huruf maksimal 5 karakter.")]
    public string Huruf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nilai minimum wajib diisi.")]
    [Range(0, 100, ErrorMessage = "Nilai minimum harus antara 0 sampai 100.")]
    public decimal NilaiMin { get; set; }
}
