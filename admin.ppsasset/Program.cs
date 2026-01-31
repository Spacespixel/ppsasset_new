using Microsoft.EntityFrameworkCore;
using PPSAssetAdmin.Data;
using PPSAssetAdmin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<AdminService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "PPSAssetAdminAuth";
        options.LoginPath = "/Dashboard/Auth/Login";
        options.AccessDeniedPath = "/Dashboard/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allow HTTP
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Fix Antiforgery for HTTP
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // context.Database.EnsureCreated(); // Or Migrate
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// app.UseHttpsRedirection(); // Disabled for local dev to avoid issues
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

// Admin Area Route
app.MapControllerRoute(
    name: "admin_dashboard_root",
    pattern: "Dashboard",
    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });

app.MapControllerRoute(
    name: "admin_dashboard",
    pattern: "Dashboard/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
