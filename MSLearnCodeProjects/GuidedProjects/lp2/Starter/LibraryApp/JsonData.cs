using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using LibraryApp.Entities;

namespace LibraryApp.ConsoleApp;

public class JsonData
{
    public List<Author>? Authors { get; set; }
    public List<Book>? Books { get; set; }
    public List<BookItem>? BookItems { get; set; }
    public List<Patron>? Patrons { get; set; }
    public List<Loan>? Loans { get; set; }

    private readonly string _authorsPath = Path.Combine("Json", "Authors.json");
    private readonly string _booksPath = Path.Combine("Json", "Books.json");
    private readonly string _bookItemsPath = Path.Combine("Json", "BookItems.json");
    private readonly string _patronsPath = Path.Combine("Json", "Patrons.json");
    private readonly string _loansPath = Path.Combine("Json", "Loans.json");

    public void EnsureDataLoaded()
    {
        if (Patrons == null) {
            LoadData();
        }
    }

    public void LoadData()
    {
        Authors = LoadJson<List<Author>>(_authorsPath);
        Books = LoadJson<List<Book>>(_booksPath);
        BookItems = LoadJson<List<BookItem>>(_bookItemsPath);
        Patrons = LoadJson<List<Patron>>(_patronsPath);
        Loans = LoadJson<List<Loan>>(_loansPath);
    }

    public List<Patron> GetPopulatedPatrons(IEnumerable<Patron> patrons)
    {
        List<Patron> populated = new List<Patron>();
        foreach (Patron patron in patrons)
        {
            populated.Add(GetPopulatedPatron(patron));
        }
        return populated;
    }

    public Patron GetPopulatedPatron(Patron p)
    {
        Patron populated = new Patron
        {
            Id = p.Id,
            Name = p.Name,
            ImageName = p.ImageName,
            MembershipStart = p.MembershipStart,
            MembershipEnd = p.MembershipEnd,
            Loans = new List<Loan>()
        };

        foreach (Loan loan in Loans!)
        {
            if (loan.PatronId == p.Id)
            {
                populated.Loans.Add(GetPopulatedLoan(loan));
            }
        }

        return populated;
    }

    public Loan GetPopulatedLoan(Loan l)
    {
        Loan populated = new Loan
        {
            Id = l.Id,
            BookItemId = l.BookItemId,
            PatronId = l.PatronId,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            ReturnDate = l.ReturnDate
        };

        foreach (BookItem bi in BookItems!)
        {
            if (bi.Id == l.BookItemId)
            {
                populated.BookItem = GetPopulatedBookItem(bi);
                break;
            }
        }

        return populated;
    }

    public BookItem GetPopulatedBookItem(BookItem bi)
    {
        BookItem populated = new BookItem
        {
            Id = bi.Id,
            BookId = bi.BookId,
            AcquisitionDate = bi.AcquisitionDate,
            Condition = bi.Condition
        };

        foreach (Book b in Books!)
        {
            if (b.Id == bi.BookId)
            {
                populated.Book = GetPopulatedBook(b);
                break;
            }
        }

        return populated;
    }

    public Book GetPopulatedBook(Book b)
    {
        Book populated = new Book
        {
            Id = b.Id,
            Title = b.Title,
            AuthorId = b.AuthorId,
            Genre = b.Genre,
            ISBN = b.ISBN,
            ImageName = b.ImageName
        };

        foreach (Author a in Authors!)
        {
            if (a.Id == b.AuthorId)
            {
                populated.Author = new Author
                {
                    Id = a.Id,
                    Name = a.Name
                };
                break;
            }
        }

        return populated;
    }

    public void SaveLoans(IEnumerable<Loan> loans)
    {
        List<Loan> loanList = new List<Loan>();
        foreach (var l in loans)
        {
            Loan loan = new Loan
            {
                // making sure only a subset of properties is set and saved
                Id = l.Id,
                BookItemId = l.BookItemId,
                PatronId = l.PatronId,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate
            };
            loanList.Add(loan);
        }
        SaveJson(_loansPath, loanList);
    }

    private void SaveJson<T>(string filePath, T data)
    {
        using (FileStream jsonStream = File.Create(filePath))
        {
            JsonSerializer.Serialize(jsonStream, data);
        }
    }

    private T? LoadJson<T>(string filePath)
    {
        using (FileStream jsonStream = File.OpenRead(filePath))
        {
            return JsonSerializer.Deserialize<T>(jsonStream);
        }
    }
}