using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;
using Microsoft.Extensions.Logging;

namespace Library.ApplicationCore.Services;
public class PatronService : IPatronService
{
    private readonly IPatronRepository _patronRepository;
    private readonly ILogger<PatronService> _logger;

    public PatronService(IPatronRepository patronRepository, ILogger<PatronService> logger)
    {
        _patronRepository = patronRepository;
        _logger = logger;
    }

    public async Task<MembershipRenewalStatus> RenewMembership(int patronId)
    {
        var patron = await _patronRepository.GetPatron(patronId);
        if (patron == null)
            return MembershipRenewalStatus.PatronNotFound;

        // don't allow to renew till 1 month before expiration
        if (patron.MembershipEnd >= DateTime.Now.AddMonths(1))
            return MembershipRenewalStatus.TooEarlyToRenew;

        // don't allow to renew if patron has overdue loans
        if (patron.Loans.Any(l => (l.ReturnDate == null) && l.DueDate < DateTime.Now))
            return MembershipRenewalStatus.LoanNotReturned;

        patron.MembershipEnd = patron.MembershipEnd.AddYears(1);
        try{
            _logger.LogInformation($"Trying to renew membership for patron {patronId}");
            await _patronRepository.UpdatePatron(patron);
            _logger.LogInformation($"Membership renewed for patron {patronId}");
            return MembershipRenewalStatus.Success;
        } catch (Exception e) {
            _logger.LogError($"Error renewing membership for patron {patronId}: {e.Message}");
            return MembershipRenewalStatus.Error;
        }
    }
}
