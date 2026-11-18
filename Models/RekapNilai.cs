using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("rekap_nilai")]
    public class RekapNilai
    {
        [Key]
        [Column("id_rekap")]
        public int IdRekap { get; set; }

        [Required]
        [Column("id_siswa")]
        public int IdSiswa { get; set; }

        [Required]
        [Column("id_mapel")]
        public int IdMapel { get; set; }

        [Required]
        [Column("id_periode")]
        public int IdPeriode { get; set; }

        [Column("nilai_akhir", TypeName = "decimal(5,2)")]
        public decimal NilaiAkhir { get; set; }

        [Required]
        [StringLength(5)]
        [Column("nilai_huruf")]
        public string NilaiHuruf { get; set; } = string.Empty; // A, B, C, D

        [Column("tanggal_perhitungan")]
        public DateTime TanggalPerhitungan { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Final"; // Final, Draft

        [ForeignKey("IdSiswa")]
        public virtual Siswa? Siswa { get; set; }

        [ForeignKey("IdMapel")]
        public virtual Mapel? Mapel { get; set; }

        [ForeignKey("IdPeriode")]
        public virtual PeriodeAkademik? Periode { get; set; }
    }
}
