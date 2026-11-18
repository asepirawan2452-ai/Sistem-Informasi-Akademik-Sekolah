using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("nilai")]
    public class Nilai
    {
        [Key]
        [Column("id_nilai")]
        public int IdNilai { get; set; }

        [Required]
        [Column("id_siswa")]
        public int IdSiswa { get; set; }

        [Required]
        [Column("id_mapel")]
        public int IdMapel { get; set; }

        [Required]
        [Column("id_periode")]
        public int IdPeriode { get; set; }

        [Required]
        [Column("id_komponen")]
        public int IdKomponen { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nama_tugas")]
        public string NamaTugas { get; set; } = "Tugas 1"; // e.g. "Tugas 1", "Tugas 2", "Tugas 3", "UTS", "UAS"

        [Required]
        [Range(0, 100)]
        [Column("nilai", TypeName = "decimal(5,2)")]
        public decimal NilaiAngka { get; set; }

        [Column("tanggal_penilaian")]
        public DateTime TanggalPenilaian { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("id_guru_penginput")]
        public int IdGuruPenginput { get; set; }

        [Column("diubah_pada")]
        public DateTime? DiubahPada { get; set; }

        [ForeignKey("IdSiswa")]
        public virtual Siswa? Siswa { get; set; }

        [ForeignKey("IdMapel")]
        public virtual Mapel? Mapel { get; set; }

        [ForeignKey("IdPeriode")]
        public virtual PeriodeAkademik? Periode { get; set; }

        [ForeignKey("IdKomponen")]
        public virtual KomponenNilai? KomponenNilai { get; set; }

        [ForeignKey("IdGuruPenginput")]
        public virtual Guru? GuruPenginput { get; set; }

        public virtual ICollection<LogPerubahanNilai> LogPerubahanList { get; set; } = new List<LogPerubahanNilai>();
    }
}
