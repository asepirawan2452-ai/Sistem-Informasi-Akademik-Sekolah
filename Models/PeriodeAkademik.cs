using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("periode_akademik")]
    public class PeriodeAkademik
    {
        [Key]
        [Column("id_periode")]
        public int IdPeriode { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nama_periode")]
        public string NamaPeriode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("tahun_ajaran")]
        public string TahunAjaran { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Column("semester")]
        public string Semester { get; set; } = "Ganjil"; // Ganjil / Genap

        [Column("tanggal_mulai")]
        public DateTime TanggalMulai { get; set; }

        [Column("tanggal_selesai")]
        public DateTime TanggalSelesai { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif"; // Aktif, Nonaktif

        public virtual ICollection<PenugasanMengajar> PenugasanList { get; set; } = new List<PenugasanMengajar>();
        public virtual ICollection<Nilai> NilaiList { get; set; } = new List<Nilai>();
        public virtual ICollection<RekapNilai> RekapNilaiList { get; set; } = new List<RekapNilai>();
    }
}
