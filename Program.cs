using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Repositories;
using SoundScape.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ������ �������� ���������� �� �������
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // ������������ ReferenceHandler.Preserve ��� ��������� �������� �������
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// ����������� ���������� �� ���� �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ������ ������ ��� �����
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<ISongService, SongService>();

// ������ ������ ��� ���������
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>(); // ������ ����� ��� ���������

// ������ ��������� ����������� ���� �����
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// ������������ ������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // ������������� ������� ����� ��� ������� �� Uploads

// ������ ��������� ��������� ����� ����� Uploads
app.UseDirectoryBrowser("/Uploads");

app.UseRouting();

app.UseAuthorization();

// ����������� ��������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
