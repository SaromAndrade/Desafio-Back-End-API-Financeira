using Infrastructure.Data;
using JwtAuthenticationManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<JwtTokenHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
     b => b.MigrationsAssembly("Infrastructure")   
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.

// Aplicar Migrations Automaticamente
// Inicializa os dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Aplica as migrations pendentes
        DbInitializer.Initialize(context);
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
        throw;
    }
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
