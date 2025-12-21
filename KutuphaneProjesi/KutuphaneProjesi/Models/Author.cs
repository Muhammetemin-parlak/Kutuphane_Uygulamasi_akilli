using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Display(Name = "Yazar Adı Soyadı")]
        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        // string? yaparak veritabanındaki eski NULL kayıtlara izin veriyoruz
        public string? FullName { get; set; }

        [Display(Name = "Biyografi")]
        public string? Bio { get; set; }

        // İlişki: Bir yazarın birden fazla kitabı olabilir
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}