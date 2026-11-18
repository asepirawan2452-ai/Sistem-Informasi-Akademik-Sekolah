using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class RekapNilai
{
    public int Id { get; set; }

    public int IdSiswa { get; set; }
    public int IdMapel { get; set; }
    public int IdPeriode { get; set; }

    [Required(ErrorMessage = "Nilai akhir wajib diisi.")]
    [Range(0, 100, ErrorMessage = "Nilai akhir harus antara 0 sampai 100.")]
    public decimal NilaiAkhir { get; set; }

    [Required(ErrorMessage = "Nilai huruf wajib diisi.")]
    [StringLength(5, ErrorMessage = "Nilai huruf maksimal 5 karakter.")]
    public string NilaiHuruf { get; set; } = string.Empty;

    public DateTime TanggalPerhitungan { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
    public string Status { get; set; } = "Aktif";

    public Siswa Siswa { get; set; } = null!;
    public Mapel Mapel { get; set; } = null!;
    public PeriodeAkademik PeriodeAkademik { get; set; } = null!;
}
