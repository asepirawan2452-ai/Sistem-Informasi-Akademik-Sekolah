using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("penugasan_mengajar")]
    public class PenugasanMengajar
    {
        [Key]
        [Column("id_penugasan")]
        public int IdPenugasan { get; set; }

        [Required]
        [Column("id_guru")]
        public int IdGuru { get; set; }

        [Required]
        [Column("id_mapel")]
        public int IdMapel { get; set; }

        [Required]
        [Column("id_kelas")]
        public int IdKelas { get; set; }

        [Required]
        [Column("id_periode")]
        public int IdPeriode { get; set; }

        [ForeignKey("IdGuru")]
        public virtual Guru? Guru { get; set; }

        [ForeignKey("IdMapel")]
        public virtual Mapel? Mapel { get; set; }

        [ForeignKey("IdKelas")]
        public virtual Kelas? Kelas { get; set; }

        [ForeignKey("IdPeriode")]
        public virtual PeriodeAkademik? Periode { get; set; }
    }
}
