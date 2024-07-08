using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

public class JsonUtility
{
    public static async Task InitializeDbFromJsonAsync(IConfiguration configuration, LibraryContext context)
    {
        // only populate the database if it's empty
        if (context.Authors.Any())
            return;

        var jsonData = new JsonData(configuration);
        await jsonData.LoadData();

        // IDs in the database after seeding will be different from the ones in the JSON files,
        // so we need to update foreign keys in entities to match the new IDs
        foreach(var author in jsonData.Authors!)
        {
            var newAuthor = new Author { Name = author.Name };
            context.Authors.Add(newAuthor);
            await context.SaveChangesAsync();
            // update all linked entities ids
            foreach(var book in jsonData.Books!)
            {
                if (book.AuthorId == author.Id)
                {
                    book.AuthorId = newAuthor.Id;
                }
            }
            author.Id = newAuthor.Id;
        }

        foreach(var book in jsonData.Books!)
        {
            var newBook = new Book { Title = book.Title, AuthorId = book.AuthorId, Genre = book.Genre, ImageName = book.ImageName, ISBN = book.ISBN };
            context.Books.Add(newBook);
            await context.SaveChangesAsync();
            // update all linked entities ids
            foreach(var bookItem in jsonData.BookItems!)
            {
                if (bookItem.BookId == book.Id)
                {
                    bookItem.BookId = newBook.Id;
                }
            }
            book.Id = newBook.Id;
        }

        foreach(var bookItem in jsonData.BookItems!)
        {
            var newBookItem = new BookItem { BookId = bookItem.BookId, AcquisitionDate = bookItem.AcquisitionDate, Condition = bookItem.Condition };
            context.BookItems.Add(newBookItem);
            await context.SaveChangesAsync();
            // update all linked entities ids
            foreach(var loan in jsonData.Loans!)
            {
                if (loan.BookItemId == bookItem.Id)
                {
                    loan.BookItemId = newBookItem.Id;
                }
            }
            bookItem.Id = newBookItem.Id;
        }

        foreach(var patron in jsonData.Patrons!)
        {
            var newPatron = new Patron { Name = patron.Name, MembershipStart = patron.MembershipStart, MembershipEnd = patron.MembershipEnd, ImageName = patron.ImageName };
            context.Patrons.Add(newPatron);
            await context.SaveChangesAsync();
            // update all linked entities ids
            foreach(var loan in jsonData.Loans!)
            {
                if (loan.PatronId == patron.Id)
                {
                    loan.PatronId = newPatron.Id;
                }
            }
            patron.Id = newPatron.Id;
        }

        foreach(var loan in jsonData.Loans!)
        {
            var newLoan = new Loan { BookItemId = loan.BookItemId, PatronId = loan.PatronId, LoanDate = loan.LoanDate, DueDate = loan.DueDate, ReturnDate = loan.ReturnDate };
            context.Loans.Add(newLoan);
            await context.SaveChangesAsync();
        }
        await context.SaveChangesAsync();
    }
}