using Library_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Loan> Loans { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Loan ----> Member
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(l => l.Memberid)
                .OnDelete(DeleteBehavior.Restrict);

            // Category <----> Book
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Books)
                .WithMany(b => b.Categories);

            // Book <----> Author
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);
        }

    }
}
