using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.Services;

var builder = WebApplication.CreateBuilder(args);

// Catat log aplikasi ke console.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Gunakan folder persisten untuk kunci cookie saat aplikasi di-host.
var dataProtectionKeysDirectory = builder.Configuration["DataProtection:KeysDirectory"]
    ?? Path.Combine(builder.Environment.ContentRootPath, ".keys");
Directory.CreateDirectory(dataProtectionKeysDirectory);

var dataProtection = builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysDirectory));
if (OperatingSystem.IsWindows())
{
    dataProtection.ProtectKeysWithDpapi();
}

// Daftarkan MVC.
builder.Services.AddControllersWithViews();

// MySQL dipakai pada konfigurasi Development; migrasi yang tersedia memakai SQL Server.
var databaseProvider = (builder.Configuration["Database:Provider"] ?? "SqlServer").Trim();
var applyMigrationsOnStartup = builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");
switch (databaseProvider)
{
    case "MySql":
    {
        var mySqlConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");
        if (string.IsNullOrWhiteSpace(mySqlConnectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:MySqlConnection belum dikonfigurasi.");
        }

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));
        break;
    }
    case "SqlServer":
    {
        var sqlServerConnectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
        if (string.IsNullOrWhiteSpace(sqlServerConnectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:SqlServerConnection belum dikonfigurasi.");
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlServerConnectionString));
        break;
    }
    default:
        throw new InvalidOperationException($"Database:Provider '{databaseProvider}' tidak dikenali. Gunakan 'MySql' atau 'SqlServer'.");
}

// Autentikasi berbasis cookie.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Daftarkan layanan aplikasi.
builder.Services.AddScoped<INilaiService, NilaiService>();
builder.Services.AddScoped<SistemInformasiNilaiAkademik.Services.Testing.ISystemTestSuite, SistemInformasiNilaiAkademik.Services.Testing.SystemTestSuite>();

var app = builder.Build();

// Terapkan perubahan skema hanya jika diaktifkan melalui konfigurasi.
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    if (applyMigrationsOnStartup && databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
    {
        await context.Database.EnsureCreatedAsync();
    }
    else if (applyMigrationsOnStartup)
    {
        await context.Database.MigrateAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        DbInitializer.SeedDevelopmentData(context);
    }
    else
    {
        DbInitializer.EnsureProductionAdmin(context, builder.Configuration);
    }
}

// Susun middleware HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
