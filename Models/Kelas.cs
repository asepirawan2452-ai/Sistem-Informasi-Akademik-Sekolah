using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("kelas")]
    public class Kelas
    {
        [Key]
        [Column("id_kelas")]
        public int IdKelas { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nama_kelas")]
        public string NamaKelas { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Column("tingkat")]
        public string Tingkat { get; set; } = string.Empty; // X, XI, XII

        [Required]
        [Column("id_jurusan")]
        public int IdJurusan { get; set; }

        [Column("id_wali_kelas")]
        public int? IdWaliKelas { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif";

        [ForeignKey("IdJurusan")]
        public virtual Jurusan? Jurusan { get; set; }

        [ForeignKey("IdWaliKelas")]
        public virtual Guru? WaliKelas { get; set; }

        public virtual ICollection<Siswa> SiswaList { get; set; } = new List<Siswa>();
        public virtual ICollection<PenugasanMengajar> PenugasanList { get; set; } = new List<PenugasanMengajar>();
    }
}
