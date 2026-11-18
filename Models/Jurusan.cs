using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("jurusan")]
    public class Jurusan
    {
        [Key]
        [Column("id_jurusan")]
        public int IdJurusan { get; set; }

        [Required]
        [StringLength(20)]
        [Column("kode")]
        public string Kode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nama")]
        public string Nama { get; set; } = string.Empty;

        public virtual ICollection<Kelas> KelasList { get; set; } = new List<Kelas>();
        public virtual ICollection<Mapel> MapelList { get; set; } = new List<Mapel>();
    }
}
