using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;
using PPSAsset.Services;
using PPSAsset.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register clean architecture services
// DatabaseProjectService - handles dynamic project data from database
builder.Services.AddScoped<DatabaseProjectService>();
// StaticProjectService - kept for potential fallback scenarios (but not for floor plans)
builder.Services.AddScoped<StaticProjectService>();
// Use DatabaseProjectService directly as the primary IProjectService (database-only for floor plans)
builder.Services.AddScoped<IProjectService, DatabaseProjectService>();
builder.Services.AddScoped<RegistrationService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
    })
    .AddGoogle(options =>
    {
        builder.Configuration.Bind("Authentication:Google", options);
        options.SaveTokens = true;
    })
    .AddFacebook(options =>
    {
        builder.Configuration.Bind("Authentication:Facebook", options);
        options.Scope.Add("email");
        options.Fields.Add("email");
        options.Fields.Add("name");
        options.Fields.Add("first_name");
        options.Fields.Add("last_name");
        options.SaveTokens = true;
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                AddClaimIfPresent(context, ClaimTypes.GivenName, "first_name");
                AddClaimIfPresent(context, ClaimTypes.Surname, "last_name");
                return Task.CompletedTask;
            }
        };
    });

// ThemeService - handles static theme configuration (no database needed)
builder.Services.AddSingleton<IThemeService, ThemeService>();

// Database migration service for data management
builder.Services.AddScoped<DatabaseMigration>();

// Add logging for better debugging and monitoring
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// PPS Asset style project routes
app.MapControllerRoute(
    name: "project-type-route",
    pattern: "{projectType}/{projectName}/{location}",
    defaults: new { controller = "Home", action = "Project" },
    constraints: new { projectType = @"singlehouse|townhome|twinhouse" });

// Legacy project route (backward compatibility)
app.MapControllerRoute(
    name: "project",
    pattern: "Project/{id=ricco-residence-hathairat}",
    defaults: new { controller = "Home", action = "Project" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void AddClaimIfPresent(OAuthCreatingTicketContext context, string claimType, string jsonKey)
{
    if (context.Identity == null)
    {
        return;
    }

    if (context.User.TryGetProperty(jsonKey, out JsonElement property))
    {
        var value = property.GetString();
        if (!string.IsNullOrWhiteSpace(value))
        {
            context.Identity.AddClaim(new Claim(claimType, value));
        }
    }
}
