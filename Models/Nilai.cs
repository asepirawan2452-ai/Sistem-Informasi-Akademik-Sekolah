using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Nilai
{
    public int Id { get; set; }

    public int IdSiswa { get; set; }
    public int IdMapel { get; set; }
    public int IdPeriode { get; set; }
    public int IdKomponen { get; set; }

    [Required(ErrorMessage = "Nilai wajib diisi.")]
    [Range(0, 100, ErrorMessage = "Nilai harus antara 0 sampai 100.")]
    public decimal NilaiAngka { get; set; }

    [DataType(DataType.Date)]
    public DateTime TanggalPenilaian { get; set; } = DateTime.Today;

    public int IdGuruPenginput { get; set; }
    public DateTime? DiubahPada { get; set; }

    public Siswa Siswa { get; set; } = null!;
    public Mapel Mapel { get; set; } = null!;
    public PeriodeAkademik PeriodeAkademik { get; set; } = null!;
    public KomponenNilai KomponenNilai { get; set; } = null!;
    public Guru GuruPenginput { get; set; } = null!;
    public ICollection<LogPerubahanNilai> LogPerubahanNilai { get; set; } = new List<LogPerubahanNilai>();
}
