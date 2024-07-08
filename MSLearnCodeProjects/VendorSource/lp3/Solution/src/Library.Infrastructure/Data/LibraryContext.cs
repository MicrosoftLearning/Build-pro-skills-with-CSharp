using Microsoft.EntityFrameworkCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class LibraryContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookItem> BookItems { get; set; }
    public DbSet<Patron> Patrons { get; set; }
    public DbSet<Loan> Loans { get; set; }

    // We need this constructor to pass options
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }
}