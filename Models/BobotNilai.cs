using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("bobot_nilai")]
    public class BobotNilai
    {
        [Key]
        [Column("id_bobot")]
        public int IdBobot { get; set; }

        [Required]
        [Column("id_mapel")]
        public int IdMapel { get; set; }

        [Required]
        [Column("id_komponen")]
        public int IdKomponen { get; set; }

        [Required]
        [Range(0, 100)]
        [Column("persentase", TypeName = "decimal(5,2)")]
        public decimal Persentase { get; set; }

        [ForeignKey("IdMapel")]
        public virtual Mapel? Mapel { get; set; }

        [ForeignKey("IdKomponen")]
        public virtual KomponenNilai? KomponenNilai { get; set; }
    }
}
