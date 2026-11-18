using System.ComponentModel.DataAnnotations;

namespace SistemNilaiAkademik.Models;

public class Role
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama role wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama role maksimal 50 karakter.")]
    public string NamaRole { get; set; } = string.Empty;

    public ICollection<Pengguna> Pengguna { get; set; } = new List<Pengguna>();
}
