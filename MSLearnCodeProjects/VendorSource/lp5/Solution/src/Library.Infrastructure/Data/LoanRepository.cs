using System.Linq;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryContext _context;

    public LoanRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetLoan(int id)
    {
        return await _context
            .Loans
            .Include(l => l.Patron)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task UpdateLoan(Loan loan)
    {
        _context.Entry(loan).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}