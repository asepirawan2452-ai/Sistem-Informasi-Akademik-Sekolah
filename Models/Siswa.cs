using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemInformasiNilaiAkademik.Models
{
    [Table("siswa")]
    public class Siswa
    {
        [Key]
        [Column("id_siswa")]
        public int IdSiswa { get; set; }

        [Required]
        [StringLength(20)]
        [Column("nis")]
        public string Nis { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("nisn")]
        public string Nisn { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nama_siswa")]
        public string NamaSiswa { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("jenis_kelamin")]
        public string JenisKelamin { get; set; } = "L"; // L / P

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [StringLength(255)]
        [Column("alamat")]
        public string Alamat { get; set; } = string.Empty;

        [Required]
        [Column("id_kelas")]
        public int IdKelas { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Aktif"; // Aktif, Lulus, Pindah

        [ForeignKey("IdKelas")]
        public virtual Kelas? Kelas { get; set; }

        public virtual ICollection<Nilai> NilaiList { get; set; } = new List<Nilai>();
        public virtual ICollection<RekapNilai> RekapNilaiList { get; set; } = new List<RekapNilai>();
    }
}
