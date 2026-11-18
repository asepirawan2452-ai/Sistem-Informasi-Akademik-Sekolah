namespace SistemNilaiAkademik.Models;

public class PenugasanMengajar
{
    public int Id { get; set; }

    public int IdGuru { get; set; }
    public int IdMapel { get; set; }
    public int IdKelas { get; set; }
    public int IdPeriode { get; set; }

    public Guru Guru { get; set; } = null!;
    public Mapel Mapel { get; set; } = null!;
    public Kelas Kelas { get; set; } = null!;
    public PeriodeAkademik PeriodeAkademik { get; set; } = null!;
}
