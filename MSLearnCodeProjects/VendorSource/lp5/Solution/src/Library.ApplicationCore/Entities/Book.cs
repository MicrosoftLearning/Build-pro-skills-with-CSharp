namespace Library.ApplicationCore.Entities;
public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public required string Genre { get; set; }
    public string? ImageName { get; set; }
    public required string ISBN { get; set; }
}
