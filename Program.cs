using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add support for controllers with JSON serialization settings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Set ReferenceHandler.Preserve to avoid cyclical errors in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configure database connection (PostgreSQL in this case)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enable support for static files and directory browsing
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Configure the request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Enable static files (e.g., for Uploads)

// Enable directory browsing for the /Uploads folder
app.UseDirectoryBrowser("/Uploads");

app.UseRouting();
app.UseAuthorization();

// Configure API controller routes
app.MapControllers();

app.Run();
