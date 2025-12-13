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
// Direct database service - NO FALLBACK to expose database connection issues
builder.Services.AddScoped<IProjectService, DatabaseProjectService>();
// ProjectMappingService - maps string project IDs to numeric MappedProjectID for backward compatibility
builder.Services.AddScoped<IProjectMappingService, ProjectMappingService>();
builder.Services.AddScoped<RegistrationService>();

// Configure authentication with cookie-only fallback
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
    });

// Only add OAuth if configuration is complete and valid
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

// Check if we have complete Google OAuth configuration
var hasValidGoogleConfig = !string.IsNullOrWhiteSpace(googleClientId) && 
                          !string.IsNullOrWhiteSpace(googleClientSecret) &&
                          googleClientId != "your-google-client-id" &&
                          googleClientSecret != "your-google-client-secret";

// Check if we have complete Facebook OAuth configuration  
var hasValidFacebookConfig = !string.IsNullOrWhiteSpace(facebookAppId) && 
                            !string.IsNullOrWhiteSpace(facebookAppSecret) &&
                            facebookAppId != "your-facebook-app-id" &&
                            facebookAppSecret != "your-facebook-app-secret";

// Only register OAuth providers if configuration is complete
if (hasValidGoogleConfig || hasValidFacebookConfig)
{
    var authBuilder = builder.Services.AddAuthentication();
    
    if (hasValidGoogleConfig)
    {
        authBuilder.AddGoogle(options =>
        {
            options.ClientId = googleClientId!;
            options.ClientSecret = googleClientSecret!;
            options.SaveTokens = true;
        });
        Console.WriteLine("Google authentication enabled");
    }
    else
    {
        Console.WriteLine("Google authentication disabled - incomplete configuration");
    }

    if (hasValidFacebookConfig)
    {
        authBuilder.AddFacebook(options =>
        {
            options.AppId = facebookAppId!;
            options.AppSecret = facebookAppSecret!;
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
        Console.WriteLine("Facebook authentication enabled");
    }
    else
    {
        Console.WriteLine("Facebook authentication disabled - incomplete configuration");
    }
}
else
{
    Console.WriteLine("OAuth authentication disabled - using cookie-only authentication");
}

// ThemeService - handles static theme configuration (no database needed)
builder.Services.AddSingleton<IThemeService, ThemeService>();

// Database migration service for data management
builder.Services.AddScoped<DatabaseMigration>();

// GTM service for Google Tag Manager integration
builder.Services.AddScoped<IGtmService, GtmService>();

// SEO service for managing meta tags and structured data
builder.Services.AddScoped<ISeoService, SeoService>();

// reCAPTCHA service for form submission validation
builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();

// Add logging for better debugging and monitoring
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Add request logging middleware for detailed debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var startTime = DateTime.UtcNow;
    
    logger.LogInformation("HTTP {Method} {Path} started. RemoteIP: {RemoteIP}, UserAgent: {UserAgent}",
        context.Request.Method, 
        context.Request.Path, 
        context.Connection.RemoteIpAddress,
        context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown");
    
    try
    {
        await next();
        var duration = DateTime.UtcNow - startTime;
        logger.LogInformation("HTTP {Method} {Path} completed in {Duration}ms with status {StatusCode}",
            context.Request.Method, 
            context.Request.Path, 
            duration.TotalMilliseconds,
            context.Response.StatusCode);
    }
    catch (Exception ex)
    {
        var duration = DateTime.UtcNow - startTime;
        logger.LogError(ex, "HTTP {Method} {Path} failed after {Duration}ms. Exception: {ExceptionType}",
            context.Request.Method, 
            context.Request.Path,
            duration.TotalMilliseconds,
            ex.GetType().Name);
        throw;
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Only use HTTPS redirection if not in production or if HTTPS is properly configured
if (app.Environment.IsDevelopment() || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// PPS Asset style project routes (Preferred format as per CLAUDE.md)
app.MapControllerRoute(
    name: "project-type-route",
    pattern: "{projectType}/{projectName}/{location}",
    defaults: new { controller = "Home", action = "Project" },
    constraints: new { projectType = @"singlehouse|townhome|twinhouse" });

// Legacy project route (Backward compatibility as per CLAUDE.md)
app.MapControllerRoute(
    name: "project",
    pattern: "Project/{id=ricco-residence-hathairat}",
    defaults: new { controller = "Home", action = "Project" });

// Custom routes for About and Contact pages (without /Home prefix)
app.MapControllerRoute(
    name: "about",
    pattern: "about-us",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "contact",
    pattern: "contact-us",
    defaults: new { controller = "Home", action = "Contact" });

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
