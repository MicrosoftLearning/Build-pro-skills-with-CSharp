using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;

namespace Library.Web.Pages;

public class PatronDetailsModel : PageModel
{
    LibraryApiHttpClient _apiClient;
    private readonly IConfiguration _configuration;

    #region Public Properties

    public required Patron Patron { get; set; }

    public string PatronImagePath => _configuration["ImagePaths:Patron"] ?? "";

    public string BookCoverImagePath => _configuration["ImagePaths:BookCover"] ?? "";

    public UiOperationStatus? UiStatus { get; set; } = null;

    #endregion

    public PatronDetailsModel(
        LibraryApiHttpClient apiClient,
        IConfiguration configuration)
    {
        _configuration = configuration;
        _apiClient = apiClient;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
          return NotFound();
        }

        var patron = await _apiClient.GetPatron(id.Value);
        if (patron == null)
        {
          return NotFound();
        }
        Patron = patron;
        return Page();
    }

    public async Task<IActionResult> OnPostRenewAsync(int id)
    {
        var status = await _apiClient.RenewMembership(id);
        UiStatus = UiOperationStatusFromMembershipRenewalStatus(status);

        Patron = (await _apiClient.GetPatron(id))!;
        return Page();
    }

    public async Task<IActionResult> OnPostReturnAsync(int loanId)
    {
        var loan = await _apiClient.GetLoan(loanId);
        if(loan == null)
            return NotFound();

        var status = await _apiClient.ReturnLoan(loanId);
        UiStatus = UiOperationStatusFromLoanReturnStatus(status);
        Patron = (await _apiClient.GetPatron(loan.PatronId))!;
        return Page();
    }

    public async Task<IActionResult> OnPostExtendAsync(int loanId)
    {
        var loan = await _apiClient.GetLoan(loanId);
        if(loan == null)
            return NotFound();
        
        var status = await _apiClient.ExtendLoan(loanId);
        UiStatus = UiOperationStatusFromLoanExtensionStatus(status);
        Patron = (await _apiClient.GetPatron(loan.PatronId))!;
        return Page();
    }

    private UiOperationStatus UiOperationStatusFromMembershipRenewalStatus(MembershipRenewalStatus status)
    {
        UiResultType uiStatus = status switch
        {
            MembershipRenewalStatus.Success => UiResultType.Success,
            MembershipRenewalStatus.TooEarlyToRenew => UiResultType.Failure,
            MembershipRenewalStatus.LoanNotReturned => UiResultType.Failure,
            MembershipRenewalStatus.PatronNotFound => UiResultType.Error,
            _ => UiResultType.Error,
        };

        return new UiOperationStatus
        {
            Type = uiStatus,
            Message = EnumHelper.GetDescription(status)
        };
    }

    private UiOperationStatus UiOperationStatusFromLoanReturnStatus(LoanReturnStatus status)
    {
        UiResultType uiStatus = status switch
        {
            LoanReturnStatus.Success => UiResultType.Success,
            LoanReturnStatus.AlreadyReturned => UiResultType.Failure,
            LoanReturnStatus.LoanNotFound => UiResultType.Error,
            _ => UiResultType.Error
        };

        return new UiOperationStatus
        {
            Type = uiStatus,
            Message = EnumHelper.GetDescription(status)
        };
    }

    private UiOperationStatus UiOperationStatusFromLoanExtensionStatus(LoanExtensionStatus status)
    {
        UiResultType uiStatus = status switch
        {
            LoanExtensionStatus.Success => UiResultType.Success,
            LoanExtensionStatus.LoanExpired
                or LoanExtensionStatus.LoanReturned
                or LoanExtensionStatus.MembershipExpired
                    => UiResultType.Failure,
            LoanExtensionStatus.LoanNotFound => UiResultType.Error,
            _ => UiResultType.Error
        };

        return new UiOperationStatus
        {
            Type = uiStatus,
            Message = EnumHelper.GetDescription(status)
        };
    }
}