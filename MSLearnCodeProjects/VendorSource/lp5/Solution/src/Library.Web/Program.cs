using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

const string key = "Retry";
builder.Services.AddHttpClient("LibraryApiHttpClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiHost"] ?? "");
    client.DefaultRequestHeaders.Add("X-API-KEY", builder.Configuration["ApiKey"]);
})
    .AddResilienceHandler(key, static builder => {
        builder.AddRetry(
            new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 4,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
            }
        );
    });

builder.Services.AddScoped<LibraryApiHttpClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/StatusCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
