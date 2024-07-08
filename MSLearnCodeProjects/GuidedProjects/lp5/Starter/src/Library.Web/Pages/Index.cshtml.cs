using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Web.Pages;

public class IndexModel : PageModel
{
    private const int pageSize = 20;
    private IPatronRepository _patronRepository;

    #region Public Properties

    public string? SearchInput { get; set; }
    public List<Patron> Patrons { get; set; } = new List<Patron>();
    public int PatronsCount { get; set; } = 0;
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int PageCount => (int)Math.Ceiling((double) PatronsCount / pageSize);

    #endregion

    public IndexModel(IPatronRepository patronRepository)
    {
        _patronRepository = patronRepository;
        Patrons = new List<Patron>();
        SearchInput = null;
    }

    public async Task OnGetAsync(string? searchInput)
    {
        SearchInput = searchInput;
        PatronsCount = await _patronRepository.GetPatronsCount(searchInput);
        Patrons = await _patronRepository.SearchPatronsPaged(searchInput, PageNumber, pageSize);
    }
}

