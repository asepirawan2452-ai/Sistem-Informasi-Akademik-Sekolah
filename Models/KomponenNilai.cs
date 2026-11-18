using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("komponen_nilai")]
    public class KomponenNilai
    {
        [Key]
        [Column("id_komponen")]
        public int IdKomponen { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nama_komponen")]
        public string NamaKomponen { get; set; } = string.Empty; // Harian, UTS, UAS

        [Required]
        [StringLength(100)]
        [Column("jenis_penilaian")]
        public string JenisPenilaian { get; set; } = string.Empty;

        [Column("bobot_default", TypeName = "decimal(5,2)")]
        public decimal BobotDefault { get; set; } = 0;

        public virtual ICollection<BobotNilai> BobotNilaiList { get; set; } = new List<BobotNilai>();
        public virtual ICollection<Nilai> NilaiList { get; set; } = new List<Nilai>();
    }
}
