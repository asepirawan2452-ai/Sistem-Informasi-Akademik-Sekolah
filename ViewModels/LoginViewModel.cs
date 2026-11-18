using SistemInformasiNilaiAkademik.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username atau email wajib diisi")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kata sandi wajib diisi")]
        [DataType(DataType.Password)]
        [Display(Name = "Kata Sandi")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ingat Saya")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Kata sandi lama wajib diisi")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kata sandi baru wajib diisi")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Kata sandi minimal 6 karakter")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Konfirmasi kata sandi tidak cocok")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
