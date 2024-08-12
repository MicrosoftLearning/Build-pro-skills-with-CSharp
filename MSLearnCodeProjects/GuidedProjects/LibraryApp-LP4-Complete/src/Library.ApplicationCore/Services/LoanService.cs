using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;
using Microsoft.Extensions.Logging;

namespace Library.ApplicationCore.Services;

public class LoanService : ILoanService
{
    public const int ExtendByDays = 14;
    private ILoanRepository _loanRepository;
    private readonly ILogger<LoanService> _logger;

    public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger)
    {
        _loanRepository = loanRepository;
        _logger = logger;
    }

    public async Task<LoanReturnStatus> ReturnLoan(int loanId)
    {
        Loan? loan = await _loanRepository.GetLoan(loanId);
        if (loan == null)
            return LoanReturnStatus.LoanNotFound;

        // check if already returned
        if (loan.ReturnDate != null)
            return LoanReturnStatus.AlreadyReturned;

        loan.ReturnDate = DateTime.Now;
        try {
            _logger.LogInformation($"Trying to return loan {loanId}");
            await _loanRepository.UpdateLoan(loan);
            _logger.LogInformation($"Loan {loanId} returned");
            return LoanReturnStatus.Success;
        } catch (Exception e) {
            _logger.LogError($"Error returning loan {loanId}: {e.Message}");
            return LoanReturnStatus.Error;
        }
    }

    public async Task<LoanExtensionStatus> ExtendLoan(int loanId)
    {
        var loan = await _loanRepository.GetLoan(loanId);

        if (loan == null)
            return LoanExtensionStatus.LoanNotFound;

        // Check if patron's membership is expired
        if (loan.Patron!.MembershipEnd < DateTime.Now)
            return LoanExtensionStatus.MembershipExpired;

        if (loan.ReturnDate != null)
            return LoanExtensionStatus.LoanReturned;

        if (loan.DueDate < DateTime.Now)
            return LoanExtensionStatus.LoanExpired;

        loan.DueDate = loan.DueDate.AddDays(ExtendByDays);
        try{
            _logger.LogInformation($"Trying to extend loan {loanId}");
            await _loanRepository.UpdateLoan(loan);
            _logger.LogInformation($"Loan {loanId} extended");
            return LoanExtensionStatus.Success;
        } catch (Exception e) {
            _logger.LogError($"Error extending loan {loanId}: {e.Message}");
            return LoanExtensionStatus.Error;
        }
    }
}

