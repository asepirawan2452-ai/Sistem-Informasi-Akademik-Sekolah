using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("guru")]
    public class Guru
    {
        [Key]
        [Column("id_guru")]
        public int IdGuru { get; set; }

        [Required]
        [Column("id_pengguna")]
        public int IdPengguna { get; set; }

        [Required]
        [StringLength(30)]
        [Column("nip_nrg")]
        public string NipNrg { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nama")]
        public string Nama { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif";

        [ForeignKey("IdPengguna")]
        public virtual Pengguna? Pengguna { get; set; }

        public virtual Kelas? KelasWali { get; set; }
        public virtual ICollection<PenugasanMengajar> PenugasanList { get; set; } = new List<PenugasanMengajar>();
        public virtual ICollection<Nilai> NilaiDiinputList { get; set; } = new List<Nilai>();
    }
}
