using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("log_perubahan_nilai")]
    public class LogPerubahanNilai
    {
        [Key]
        [Column("id_log")]
        public int IdLog { get; set; }

        [Required]
        [Column("id_nilai")]
        public int IdNilai { get; set; }

        [Column("nilai_lama", TypeName = "decimal(5,2)")]
        public decimal NilaiLama { get; set; }

        [Column("nilai_baru", TypeName = "decimal(5,2)")]
        public decimal NilaiBaru { get; set; }

        [Required]
        [Column("id_pengguna")]
        public int IdPengguna { get; set; }

        [Column("waktu_perubahan")]
        public DateTime WaktuPerubahan { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(255)]
        [Column("alasan")]
        public string Alasan { get; set; } = string.Empty;

        [ForeignKey("IdNilai")]
        public virtual Nilai? Nilai { get; set; }

        [ForeignKey("IdPengguna")]
        public virtual Pengguna? Pengguna { get; set; }
    }
}
