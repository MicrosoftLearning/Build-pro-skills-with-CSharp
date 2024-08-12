using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data;
public class PatronRepository : IPatronRepository
{
    private readonly LibraryContext _context;

    public PatronRepository(LibraryContext context)
    {
        _context = context;
    }

    private static IQueryable<Patron> AddSearchCondition(IQueryable<Patron> query, string? searchInput)
    {
        if (!string.IsNullOrEmpty(searchInput))
        {
            query = query.Where(p =>
                EF.Functions.Like(p.Name, $"%{searchInput}%")
            );
        }
        return query;
    }

    public async Task<List<Patron>> SearchPatrons(string? searchInput)
    {
        IQueryable<Patron> query = _context.Patrons.AsNoTracking();
        query = AddSearchCondition(query, searchInput);
        query = query.OrderBy(p => p.Name);
        return await query.ToListAsync();
    }

    public async Task<Patron?> GetPatron(int id)
    {
        return await _context.Patrons
          .Include(p => p.Loans)
              .ThenInclude(l => l.BookItem!)
                  .ThenInclude(bi => bi.Book!)
                      .ThenInclude(p => p.Author!)
          .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task UpdatePatron(Patron patron)
    {
        _context.Entry(patron).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}