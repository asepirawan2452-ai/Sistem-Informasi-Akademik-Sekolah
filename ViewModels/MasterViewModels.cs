using SistemInformasiNilaiAkademik.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class SiswaFormViewModel
    {
        public int IdSiswa { get; set; }

        [Required(ErrorMessage = "NIS wajib diisi")]
        [StringLength(20)]
        public string Nis { get; set; } = string.Empty;

        [Required(ErrorMessage = "NISN wajib diisi")]
        [StringLength(20)]
        public string Nisn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama siswa wajib diisi")]
        [StringLength(100)]
        public string NamaSiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih jenis kelamin")]
        public string JenisKelamin { get; set; } = "L";

        [Required(ErrorMessage = "Tanggal lahir wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime TanggalLahir { get; set; } = new DateTime(2008, 1, 1);

        [StringLength(255)]
        public string Alamat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih kelas")]
        public int IdKelas { get; set; }

        public string Status { get; set; } = "Aktif";

        public SelectList? KelasList { get; set; }
    }

    public class GuruFormViewModel
    {
        public int IdGuru { get; set; }

        [Required(ErrorMessage = "NIP/NRG wajib diisi")]
        [StringLength(30)]
        public string NipNrg { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama guru wajib diisi")]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        public int IdPengguna { get; set; }

        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }

        public string Status { get; set; } = "Aktif";

        public SelectList? PenggunaList { get; set; }
    }

    public class MapelFormViewModel
    {
        public int IdMapel { get; set; }

        [Required(ErrorMessage = "Kode mapel wajib diisi")]
        [StringLength(30)]
        public string KodeMapel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama mapel wajib diisi")]
        [StringLength(100)]
        public string NamaMapel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih kelompok")]
        public string Kelompok { get; set; } = "Muatan Umum"; // Muatan Umum, Kejuruan

        public int? IdJurusan { get; set; }

        public string? Deskripsi { get; set; }

        public string Status { get; set; } = "Aktif";

        public SelectList? JurusanList { get; set; }
    }

    public class KelasFormViewModel
    {
        public int IdKelas { get; set; }

        [Required(ErrorMessage = "Nama kelas wajib diisi")]
        [StringLength(50)]
        public string NamaKelas { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih tingkat")]
        public string Tingkat { get; set; } = "X"; // X, XI, XII

        [Required(ErrorMessage = "Pilih kejuruan")]
        public int IdJurusan { get; set; }

        public int? IdWaliKelas { get; set; }

        public string Status { get; set; } = "Aktif";

        public SelectList? JurusanList { get; set; }
        public SelectList? GuruList { get; set; }
    }

    public class PenggunaFormViewModel
    {
        public int IdPengguna { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username wajib diisi")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        [StringLength(30)]
        public string? NipNrg { get; set; }

        [Required(ErrorMessage = "Pilih role")]
        public int IdRole { get; set; }

        public string Status { get; set; } = "Aktif";

        public SelectList? RoleList { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int IdPengguna { get; set; }

        public string Nama { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kata sandi baru wajib diisi.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Kata sandi harus terdiri dari 8 sampai 100 karakter.")]
        [DataType(DataType.Password)]
        public string KataSandiBaru { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi kata sandi wajib diisi.")]
        [Compare(nameof(KataSandiBaru), ErrorMessage = "Konfirmasi kata sandi tidak cocok.")]
        [DataType(DataType.Password)]
        public string KonfirmasiKataSandi { get; set; } = string.Empty;
    }

    public class PenugasanFormViewModel
    {
        public int IdPenugasan { get; set; }

        [Required(ErrorMessage = "Pilih guru")]
        public int IdGuru { get; set; }

        [Required(ErrorMessage = "Pilih mapel")]
        public int IdMapel { get; set; }

        [Required(ErrorMessage = "Pilih kelas")]
        public int IdKelas { get; set; }

        [Required(ErrorMessage = "Pilih periode")]
        public int IdPeriode { get; set; }

        public SelectList? GuruList { get; set; }
        public SelectList? MapelList { get; set; }
        public SelectList? KelasList { get; set; }
        public SelectList? PeriodeList { get; set; }
    }

    public class BobotMapelViewModel
    {
        public int IdMapel { get; set; }
        public string KodeMapel { get; set; } = string.Empty;
        public string NamaMapel { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Bobot Harian 0-100%")]
        public decimal BobotHarian { get; set; }

        [Range(0, 100, ErrorMessage = "Bobot UTS 0-100%")]
        public decimal BobotUTS { get; set; }

        [Range(0, 100, ErrorMessage = "Bobot UAS 0-100%")]
        public decimal BobotUAS { get; set; }
    }
}
