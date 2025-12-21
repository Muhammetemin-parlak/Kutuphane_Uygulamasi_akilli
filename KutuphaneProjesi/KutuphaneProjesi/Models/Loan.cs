using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int BookId { get; set; }

        // LoanDate genelde zorunludur, ama en güvenli haliyle bırakalım.
        public DateTime LoanDate { get; set; } = DateTime.Now;

        // --- KRİTİK DÜZELTME BURADA ---
        // DuDate'e de ? koyarak NULL gelmesine izin verdik.
        public DateTime? DueDate { get; set; }

        public DateTime? ReturnDate { get; set; } // İade tarihi boş olabilir.

        public decimal? FineAmount { get; set; } // Ceza boş olabilir.

        public virtual User? User { get; set; }
        public virtual Book? Book { get; set; }
        public string? Status { get; set; }
    }
}