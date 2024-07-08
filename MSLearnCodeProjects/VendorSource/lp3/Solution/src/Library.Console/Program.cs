using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Library.Infrastructure.Data;
using Library.ApplicationCore;
using Library.ApplicationCore.Services;
using Microsoft.EntityFrameworkCore;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json")
    .Build();

services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddSingleton<IConfiguration>(configuration);
services.AddScoped<IPatronRepository, PatronRepository>();
services.AddScoped<ILoanRepository, LoanRepository>();
services.AddScoped<ILoanService, LoanService>();
services.AddScoped<IPatronService, PatronService>();

services.AddSingleton<JsonData>();
services.AddSingleton<ConsoleApp>();

var servicesProvider = services.BuildServiceProvider();

var context = servicesProvider.GetRequiredService<LibraryContext>();
JsonUtility.InitializeDbFromJsonAsync(configuration, context).Wait();

var consoleApp = servicesProvider.GetRequiredService<ConsoleApp>();
consoleApp.Run().Wait();
