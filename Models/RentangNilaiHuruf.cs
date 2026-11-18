using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("rentang_nilai_huruf")]
    public class RentangNilaiHuruf
    {
        [Key]
        [Column("id_rentang")]
        public int IdRentang { get; set; }

        [Required]
        [StringLength(5)]
        [Column("huruf")]
        public string Huruf { get; set; } = string.Empty; // A, B, C, D

        [Column("nilai_min", TypeName = "decimal(5,2)")]
        public decimal NilaiMin { get; set; }

        [Column("nilai_max", TypeName = "decimal(5,2)")]
        public decimal NilaiMax { get; set; }

        [StringLength(100)]
        [Column("keterangan")]
        public string Keterangan { get; set; } = string.Empty; // Sangat Baik, Baik, Cukup, Kurang
    }
}
