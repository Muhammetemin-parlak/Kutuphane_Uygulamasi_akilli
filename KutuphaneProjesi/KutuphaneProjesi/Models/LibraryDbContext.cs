using Microsoft.EntityFrameworkCore;

namespace KutuphaneProjesi.Models
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Veritabanı tablolarının isimlerini belirliyoruz
            modelBuilder.Entity<Book>().ToTable("books");
            modelBuilder.Entity<Author>().ToTable("authors");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Loan>().ToTable("loans");

            // ÖNEMLİ: İlişkileri tanımlıyoruz ama "IsRequired" kısıtlamalarını kaldırdık.
            // Artık Model dosyalarındaki (Book.cs vb.) soru işaretleri (?) geçerli olacak.

            // Kitap - Yazar İlişkisi
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            // Kitap - Kategori İlişkisi
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

            // Ödünç - Kitap İlişkisi
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);

            // Ödünç - Üye İlişkisi
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId);
        }
    }
}