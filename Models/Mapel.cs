using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("mapel")]
    public class Mapel
    {
        [Key]
        [Column("id_mapel")]
        public int IdMapel { get; set; }

        [Required]
        [StringLength(30)]
        [Column("kode_mapel")]
        public string KodeMapel { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nama_mapel")]
        public string NamaMapel { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("kelompok")]
        public string Kelompok { get; set; } = "Muatan Umum"; // Muatan Umum, Kejuruan

        [Column("id_jurusan")]
        public int? IdJurusan { get; set; }

        [StringLength(255)]
        [Column("deskripsi")]
        public string? Deskripsi { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif";

        [ForeignKey("IdJurusan")]
        public virtual Jurusan? Jurusan { get; set; }

        public virtual ICollection<BobotNilai> BobotNilaiList { get; set; } = new List<BobotNilai>();
        public virtual ICollection<PenugasanMengajar> PenugasanList { get; set; } = new List<PenugasanMengajar>();
        public virtual ICollection<Nilai> NilaiList { get; set; } = new List<Nilai>();
        public virtual ICollection<RekapNilai> RekapNilaiList { get; set; } = new List<RekapNilai>();
    }
}
