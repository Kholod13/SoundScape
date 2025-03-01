using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Додаємо підтримку контролерів з JSON-налаштуваннями
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Налаштування підключення до PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додаємо CORS для підтримки запитів із фронтенду
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Додаємо підтримку статичних файлів та перегляду каталогів
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Налаштування пайплайну запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Дозволяємо сервінг статичних файлів

// Додаємо підтримку перегляду файлів у папці "Uploads"
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/Uploads"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/Uploads"
});

// Включаємо CORS
app.UseCors("AllowAllOrigins");

app.UseRouting();
app.UseAuthorization();

// Логування запитів для дебагу
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

app.MapControllers();

app.Run();
