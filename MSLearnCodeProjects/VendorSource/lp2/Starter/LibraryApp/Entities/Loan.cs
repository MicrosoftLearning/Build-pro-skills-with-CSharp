namespace LibraryApp.Entities;

public class Loan
{
    public int Id { get; set; }
    public int BookItemId { get; set; }
    public BookItem? BookItem { get; set; }
    public int PatronId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}