using FeatureFlag.Data;
using FeatureFlag.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Serilog;

// Logs dizini proje kökünde (dotnet run yapılan dizinde) oluşturulur
var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
Directory.CreateDirectory(logsPath);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(logsPath, "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 31,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Uygulama başlatılıyor...");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var featureModel = new FeatureManagementModel();
builder.Configuration.GetSection("FeatureManagement").Bind(featureModel);
builder.Services.AddFeatureManagement(builder.Configuration.GetSection("FeatureManagement"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context);
}

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmeyen bir hata ile sonlandı.");
    throw;
}
finally
{
    Log.Information("Uygulama kapatılıyor...");
    Log.CloseAndFlush();
}