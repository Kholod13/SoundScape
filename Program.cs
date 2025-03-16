
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoundScape.Data;
using SoundScape.Models;
using SoundScape.Services;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });


var apiUrl = builder.Configuration.GetValue<string>("AppSettings:ApiUrl");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policyBuilder =>
        {
            policyBuilder.WithOrigins(builder.Configuration["AppSettings:ReactAppUrl"])
                         .AllowAnyHeader()
                         .AllowAnyMethod();
        });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));


// Підключення контексту бази даних
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додавання Identity
//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
  //  .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Seeder.SeedAlbum(services);
        Console.WriteLine("Database seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();

app.UseCors("AllowReactApp");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/api/search", async (HttpContext context, ApplicationDbContext db) =>
{
    var query = context.Request.Query["query"].ToString();

    if (string.IsNullOrWhiteSpace(query))
        return Results.BadRequest("Search query is required");

    var tracks = await db.MusicTracks
        .Where(t => t.Title.Contains(query) || t.Artist.Contains(query))
        .ToListAsync();

    return tracks.Any() ? Results.Ok(tracks) : Results.NotFound("No tracks found.");
});

app.Run();

