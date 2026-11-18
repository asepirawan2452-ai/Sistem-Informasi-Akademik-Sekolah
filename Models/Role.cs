using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id_role")]
        public int IdRole { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nama_role")]
        public string NamaRole { get; set; } = string.Empty;

        public virtual ICollection<Pengguna> PenggunaList { get; set; } = new List<Pengguna>();
    }
}
