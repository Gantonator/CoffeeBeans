using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;
using CoffeeBeanApi.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<CoffeeBeanContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<CoffeeBeanContext>(options =>
        options.UseSqlite("Data Source=coffeebeans.db"));
}


builder.Services.AddScoped<ICoffeeBeanService, CoffeeBeanService>();
builder.Services.AddScoped<ICoffeeBeanRepository, CoffeeBeanRepository>();
builder.Services.AddScoped<IColourService, ColourService>();
builder.Services.AddScoped<IColourRepository, ColourRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddAutoMapper(typeof(CoffeeBeanProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
    var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    await context.Database.EnsureCreatedAsync();
    await DataSeeder.SeedDataAsync(context, environment);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

app.MapControllers();
app.Run();

public partial class Program { }