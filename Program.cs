using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape;

var builder = WebApplication.CreateBuilder(args);

// Підключення до PostgreSQL
builder.Services.AddDbContext<SoundScapeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додаємо підтримку MVC (Views + API)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Налаштування для роботи з HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles(); // Додає підтримку статичних файлів (CSS, JS)
app.UseRouting();
app.UseAuthorization();

// Маршрутизація
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();  // Додає підтримку Razor Pages
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"); // Головна сторінка
});

app.Run();
