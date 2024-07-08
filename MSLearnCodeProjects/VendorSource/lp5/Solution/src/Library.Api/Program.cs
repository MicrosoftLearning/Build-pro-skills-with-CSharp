using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;
using Library.ApplicationCore.Services;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IPatronRepository, PatronRepository>();
services.AddScoped<ILoanRepository, LoanRepository>();
services.AddScoped<ILoanService, LoanService>();
services.AddScoped<IPatronService, PatronService>();
services.AddSingleton<JsonData>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<LibraryContext>();
        JsonUtility.InitializeDbFromJsonAsync(app.Configuration, context).Wait();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
    {
        var apiKey = app.Configuration["ApiKey"];

        if (apiKey != extractedApiKey)
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }
    }
    else
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("API Key is missing");
        return;
    }

    await next.Invoke();
});

# region Patrons

app.MapGet("/patrons", async (string? searchInput, int pageNumber, int pageSize, IPatronRepository patronRepository) =>
{
    var patrons = await patronRepository.SearchPatronsPaged(searchInput, pageNumber, pageSize);
    return patrons;
})
.WithName("SearchPatronsPaged")
.WithOpenApi();

app.MapGet("/patrons/count", async (string? searchInput, IPatronRepository patronRepository) =>
{
    var count = await patronRepository.GetPatronsCount(searchInput);
    return count;
})
.WithName("GetPatronsCount")
.WithOpenApi();


app.MapGet("/patrons/{patronId}", async (int patronId, IPatronRepository patronRepository) =>
{
    var patron = await patronRepository.GetPatron(patronId);
    if (patron == null)
        return Results.NotFound();

    return Results.Ok(new Patron {
        Id = patron.Id,
        Name = patron.Name,
        MembershipStart = patron.MembershipStart,
        MembershipEnd = patron.MembershipEnd,
        ImageName = patron.ImageName,
        Loans = patron.Loans.Select(l => new Loan {
            Id = l.Id,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            ReturnDate = l.ReturnDate,
            BookItem = new BookItem {
                // we rely on GetPatron setting BookItem, Book, and Author
                Id = l.BookItem!.Id,
                Book = new Book {
                    Id = l.BookItem!.Book!.Id,
                    Title = l.BookItem.Book.Title,
                    ImageName = l.BookItem.Book.ImageName,
                    Author = new Author {
                        Id = l.BookItem!.Book!.Author!.Id,
                        Name = l.BookItem.Book.Author.Name
                    },
                    ISBN = l.BookItem.Book.ISBN,
                    Genre = l.BookItem.Book.Genre
                }
            }
        }).ToList()
    });
})
.WithName("GetPatron")
.WithOpenApi();

app.MapPost("/patrons/{patronId}/renew", async (int patronId, IPatronService patronService) =>
{
    var status = await patronService.RenewMembership(patronId);
    if (status == MembershipRenewalStatus.PatronNotFound)
        return Results.NotFound();

    return Results.Ok(status);
})
.WithName("RenewMembership")
.WithOpenApi();

#endregion

#region Loans

app.MapGet("/loans/{loanId}", async (int loanId, ILoanService loanService, ILoanRepository loanRepository) =>
{
    var loan = await loanRepository.GetLoan(loanId);
    if (loan == null)
        return Results.NotFound();

    return Results.Ok(new Loan {
        Id = loan.Id,
        LoanDate = loan.LoanDate,
        DueDate = loan.DueDate,
        ReturnDate = loan.ReturnDate,
        PatronId = loan.PatronId,
        Patron = new Patron {
            // we rely on GetLoan setting Patron
            Id = loan.Patron!.Id,
            Name = loan.Patron.Name,
            MembershipStart = loan.Patron.MembershipStart,
            MembershipEnd = loan.Patron.MembershipEnd,
            ImageName = loan.Patron.ImageName
        }
    });
})
.WithName("GetLoan")
.WithOpenApi();

app.MapPost("/loans/{loanId}/return", async (int loanId, ILoanService loanService) =>
{
    var status = await loanService.ReturnLoan(loanId);
    if (status == LoanReturnStatus.LoanNotFound)
        return Results.NotFound();

    return Results.Ok(status);
})
.WithName("ReturnLoan")
.WithOpenApi();

app.MapPost("/loans/{loanId}/extend", async (int loanId, ILoanService loanService) =>
{
    var status = await loanService.ExtendLoan(loanId);
    if (status == LoanExtensionStatus.LoanNotFound)
        return Results.NotFound();

    return Results.Ok(status);
})
.WithName("ExtendLoan")
.WithOpenApi();


#endregion

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
