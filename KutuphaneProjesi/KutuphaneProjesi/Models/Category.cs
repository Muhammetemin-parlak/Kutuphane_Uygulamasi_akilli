using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        // string? yaparak veritabanındaki eski NULL kayıtlara izin veriyoruz
        public string? Name { get; set; }

        // İlişki: Bir kategoride birden fazla kitap olabilir
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}