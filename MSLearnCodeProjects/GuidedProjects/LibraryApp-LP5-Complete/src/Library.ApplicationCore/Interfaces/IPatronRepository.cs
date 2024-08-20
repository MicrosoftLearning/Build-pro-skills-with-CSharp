using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IPatronRepository {
    Task<Patron?> GetPatron(int patronId);
    Task<List<Patron>> SearchPatronsPaged(string? searchInput, int pageNumber, int pageSize);
    Task<int> GetPatronsCount(string? searchInput);
    Task UpdatePatron(Patron patron);
}