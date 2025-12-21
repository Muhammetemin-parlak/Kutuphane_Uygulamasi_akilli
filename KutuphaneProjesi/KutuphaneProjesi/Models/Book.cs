using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Display(Name = "Kitap Adı")]
        public string? Title { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "ISBN Numarası")]
        public string? Isbn { get; set; }

        [Display(Name = "Sayfa Sayısı")]
        public int? PageCount { get; set; }

        [Display(Name = "Yayın Yılı")]
        public int? PublishYear { get; set; }

        [Display(Name = "Stok Adedi")]
        public int StockQuantity { get; set; } // Stok mantıken null olmaz, 0 olur.

        public bool IsDeleted { get; set; } = false;

        // --- KRİTİK DÜZELTME BURADA ---
        // int yanına ? koyarak "Yazarı veya Kategorisi silinmiş olabilir" diyoruz.
        [Display(Name = "Yazar")]
        public int? AuthorId { get; set; }

        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
        // ------------------------------

        public virtual Author? Author { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}