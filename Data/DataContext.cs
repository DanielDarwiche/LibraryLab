using LibraryLab.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryLab.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=INTICOMPUTER;Initial Catalog=BookApiDb;Integrated Security=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd(); // Manually registering Identity Specification for ID
        }
        public DbSet<Book> Books { get; set; }
    }
}
