using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Null hatası almamak için soru işareti (?) koyduk
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // EKSİK OLAN 1: Telefon Alanı
        public string? Phone { get; set; }

        public string? Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? RoleId { get; set; }

        // EKSİK OLAN 2: Ödünçler Listesi (İlişki)
        // Bir kullanıcının birden fazla ödünç işlemi olabilir.
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}