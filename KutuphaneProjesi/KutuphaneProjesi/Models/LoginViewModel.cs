using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty; // Varsayılan değer eklendi

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Varsayılan değer eklendi

        // Beni Hatırla özelliği istenirse buraya eklenebilir
        // public bool RememberMe { get; set; }
    }
}