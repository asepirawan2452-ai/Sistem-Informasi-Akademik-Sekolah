using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Helpers;

EnvFile.Load(Directory.GetCurrentDirectory());

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var server = builder.Configuration["DB_SERVER"] ?? "localhost";
var port = builder.Configuration["DB_PORT"] ?? "3306";
var database = builder.Configuration["DB_NAME"] ?? "db_nilai_akademik";
var user = builder.Configuration["DB_USER"] ?? "root";
var password = builder.Configuration["DB_PASSWORD"] ?? string.Empty;
var connectionString = $"Server={server};Port={port};Database={database};User={user};Password={password};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db, builder.Environment.ContentRootPath);
    var jumlah = DbSeeder.Hitung(db);
    app.Logger.LogInformation("SEED_SIAKAD " + string.Join(", ", jumlah.Select(x => $"{x.Key}={x.Value}")));
}

app.Run();
