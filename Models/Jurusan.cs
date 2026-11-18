using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Jurusan
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kode jurusan wajib diisi.")]
    [StringLength(20, ErrorMessage = "Kode jurusan maksimal 20 karakter.")]
    public string Kode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama jurusan wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama jurusan maksimal 100 karakter.")]
    public string Nama { get; set; } = string.Empty;

    public ICollection<Kelas> Kelas { get; set; } = new List<Kelas>();
    public ICollection<Mapel> Mapel { get; set; } = new List<Mapel>();
}
