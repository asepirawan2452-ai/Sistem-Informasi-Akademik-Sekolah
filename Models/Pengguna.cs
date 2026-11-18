using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("pengguna")]
    public class Pengguna
    {
        [Key]
        [Column("id_pengguna")]
        public int IdPengguna { get; set; }

        [Required]
        [Column("id_role")]
        public int IdRole { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nama")]
        public string Nama { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif"; // Aktif, Nonaktif

        [Column("dibuat_pada")]
        public DateTime DibuatPada { get; set; } = DateTime.UtcNow;

        [ForeignKey("IdRole")]
        public virtual Role? Role { get; set; }

        public virtual Guru? Guru { get; set; }
    }
}
