using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty; // Varsayılan değer atadık

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Varsayılan değer atadık

        [DataType(DataType.Password)]
        [Display(Name = "Şifreyi Onayla")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty; // Varsayılan değer atadık
    }
}