using System.Net;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;

public class LibraryApiHttpClient
{
    private readonly HttpClient _client;

    public LibraryApiHttpClient(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("LibraryApiHttpClient");
    }

    #region Patrons

    public async Task<int> GetPatronsCount(string? searchInput)
    {
        var response = await _client.GetAsync($"/patrons/count?searchInput={searchInput}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<int>();
        }
        else
        {
            throw new Exception("Failed to get patrons count");
        }
    }

    public async Task<Patron?> GetPatron(int patronId)
    {
        var response = await _client.GetAsync($"/patrons/{patronId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Patron>();
        }
        else
        {
            throw new Exception($"Failed to get patron with ID {patronId}");
        }
    }

    public async Task<List<Patron>> SearchPatronsPaged(string? searchInput, int pageNumber, int pageSize)
    {
        var response = await _client.GetAsync($"/patrons?searchInput={searchInput}&pageNumber={pageNumber}&pageSize={pageSize}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Patron>>() ?? new List<Patron>();
        }
        else
        {
            throw new Exception("Failed to search patrons");
        }
    }

    public async Task<MembershipRenewalStatus> RenewMembership(int patronId)
    {
        var response = await _client.PostAsync($"/patrons/{patronId}/renew", null);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MembershipRenewalStatus>();
        }
        else
        {
            throw new Exception($"Failed to renew membership for patron with ID {patronId}");
        }
    }

    #endregion

    #region Loans

    public async Task<Loan?> GetLoan(int loanId)
    {
        var response = await _client.GetAsync($"/loans/{loanId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Loan>();
        }
        else if (response.StatusCode == HttpStatusCode.NotFound) {
            return null;
        }
        else
        {
            throw new Exception($"Failed to get loan with ID {loanId}");
        }
    }

    public async Task<LoanReturnStatus> ReturnLoan(int loanId)
    {
        var response = await _client.PostAsync($"/loans/{loanId}/return", null);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoanReturnStatus>();
        }
        else if (response.StatusCode == HttpStatusCode.NotFound) {
            return LoanReturnStatus.LoanNotFound;
        }
        else
        {
            throw new Exception($"Failed to return loan with ID {loanId}");
        }
    }

    public async Task<LoanExtensionStatus> ExtendLoan(int loanId)
    {
        var response = await _client.PostAsync($"/loans/{loanId}/extend", null);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoanExtensionStatus>();
        }
        else if (response.StatusCode == HttpStatusCode.NotFound) {
            return LoanExtensionStatus.LoanNotFound;
        }
        else
        {
            throw new Exception($"Failed to extend loan with ID {loanId}");
        }
    }

    #endregion
}