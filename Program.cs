using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Repositories;
using SoundScape.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Додаємо підтримку контролерів та виглядів
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Встановлюємо ReferenceHandler.Preserve для уникнення циклічних помилок
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Налаштовуємо підключення до бази даних
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додаємо сервіси для пісень
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<ISongService, SongService>();

// Додаємо сервіси для плейлистів
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>(); // Додано сервіс для плейлистів

// Додаємо можливість переглядати вміст папки
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Конфігурація запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Використовуємо статичні файли для доступу до Uploads

// Додаємо можливість перегляду вмісту папки Uploads
app.UseDirectoryBrowser("/Uploads");

app.UseRouting();

app.UseAuthorization();

// Налаштовуємо маршрути
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
