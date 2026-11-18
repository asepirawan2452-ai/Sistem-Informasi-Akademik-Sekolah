using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class LogPerubahanNilai
{
    public int Id { get; set; }

    public int IdNilai { get; set; }

    [Range(0, 100, ErrorMessage = "Nilai lama harus antara 0 sampai 100.")]
    public decimal NilaiLama { get; set; }

    [Range(0, 100, ErrorMessage = "Nilai baru harus antara 0 sampai 100.")]
    public decimal NilaiBaru { get; set; }

    public int IdPengguna { get; set; }
    public DateTime WaktuPerubahan { get; set; } = DateTime.Now;

    [StringLength(255, ErrorMessage = "Alasan maksimal 255 karakter.")]
    public string? Alasan { get; set; }

    public Nilai Nilai { get; set; } = null!;
    public Pengguna Pengguna { get; set; } = null!;
}
