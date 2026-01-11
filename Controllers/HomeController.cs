using Microsoft.AspNetCore.Mvc;
using PPSAsset.Models;
using PPSAsset.Services;
using PPSAsset.Data;
using System.Diagnostics;
using Dapper;
using System.Net;
using System.Security.Claims;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PPSAsset.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectService _projectService;
        private readonly IThemeService _themeService;
        private readonly DatabaseMigration _databaseMigration;
        private readonly RegistrationService _registrationService;
        private readonly IConfiguration _configuration;
        private readonly IGtmService _gtmService;
        private readonly ISeoService _seoService;
        private readonly IRecaptchaService _recaptchaService;

        public HomeController(ILogger<HomeController> logger, IProjectService projectService, IThemeService themeService, DatabaseMigration databaseMigration, IConfiguration configuration, RegistrationService registrationService, IGtmService gtmService, ISeoService seoService, IRecaptchaService recaptchaService)
        {
            _logger = logger;
            _projectService = projectService;
            _themeService = themeService;
            _databaseMigration = databaseMigration;
            _registrationService = registrationService;
            _configuration = configuration;
            _gtmService = gtmService;
            _seoService = seoService;
            _recaptchaService = recaptchaService;
        }

        private void SetNavigationData()
        {
            _logger.LogInformation("Attempting to load navigation projects from database...");
            // Get all projects for navigation dropdown - LET IT THROW!
            var allProjects = _projectService.GetAllProjects();
            ViewBag.NavigationProjects = allProjects;
            _logger.LogInformation("Successfully loaded {ProjectCount} projects for navigation", allProjects.Count);
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Index action started. Request path: {Path}, Host: {Host}", Request.Path, Request.Host);
                
                // Set navigation data for dropdown menu
                _logger.LogInformation("Setting navigation data...");
                SetNavigationData();
                _logger.LogInformation("Navigation data set successfully");
                
                // Homepage uses default theme from theme service
                _logger.LogInformation("Getting default theme...");
                var theme = _themeService.GetDefaultTheme();
                ViewBag.ProjectTheme = theme.CssClass;
                ViewBag.ThemePrimaryColor = theme.PrimaryColor;
                ViewBag.ThemeSecondaryColor = theme.SecondaryColor;
                ViewBag.ThemeLightBackground = theme.LightBackground;
                _logger.LogInformation("Theme applied: {ThemeCss}", theme.CssClass);

                // Set GTM ID for global site
                _logger.LogInformation("Getting GTM ID...");
                var gtmId = await _gtmService.GetGlobalGtmIdAsync();
                ViewBag.GtmId = gtmId;
                _logger.LogInformation("GTM ID set: {GtmId}", gtmId ?? "null");

                // Set SEO metadata for homepage
                _logger.LogInformation("Getting SEO metadata...");
                var seoMetadata = _seoService.GetPageMetadata("home");
                ViewBag.SeoTitle = seoMetadata.Title;
                ViewBag.SeoDescription = seoMetadata.Description;
                ViewBag.SeoKeywords = seoMetadata.Keywords;
                ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/");
                _logger.LogInformation("SEO metadata set. Title: {Title}", seoMetadata.Title);

                // Set JSON-LD Organization schema
                _logger.LogInformation("Getting organization schema...");
                ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();
                _logger.LogInformation("Organization schema set");

                // Get available projects using the simplified DatabaseProjectService
                _logger.LogInformation("Getting featured projects...");
                var featuredProjects = _projectService.GetAvailableProjects();
                _logger.LogInformation("Featured projects loaded. Count: {ProjectCount}", featuredProjects?.Count ?? 0);

                _logger.LogInformation("Index action completed successfully. Returning view with {ProjectCount} projects", featuredProjects?.Count ?? 0);
                return View(featuredProjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in Index action. Request: {Method} {Path} from {RemoteIp}. User: {User}", 
                    Request.Method, Request.Path, Request.HttpContext.Connection.RemoteIpAddress, User?.Identity?.Name ?? "Anonymous");
                
                // Return a simple error view or rethrow to let global error handler catch it
                throw; // Let the global error handling middleware deal with it
            }
        }

        public async Task<IActionResult> About()
        {
            // Set navigation data for dropdown menu
            SetNavigationData();
            
            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set SEO metadata for about page
            var seoMetadata = _seoService.GetPageMetadata("about");
            ViewBag.SeoTitle = seoMetadata.Title;
            ViewBag.SeoDescription = seoMetadata.Description;
            ViewBag.SeoKeywords = seoMetadata.Keywords;
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/About");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            return View();
        }

        public async Task<IActionResult> Contact()
        {
            // Set navigation data for dropdown menu
            SetNavigationData();
            
            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set SEO metadata for contact page
            var seoMetadata = _seoService.GetPageMetadata("contact");
            ViewBag.SeoTitle = seoMetadata.Title;
            ViewBag.SeoDescription = seoMetadata.Description;
            ViewBag.SeoKeywords = seoMetadata.Keywords;
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/Contact");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            return View();
        }

        public async Task<IActionResult> CookiePolicy()
        {
            // Set navigation data for dropdown menu
            SetNavigationData();
            
            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set SEO metadata for cookie policy page
            // We can reuse a generic one or just set title manually if no specific metadata exists
            ViewBag.SeoTitle = "นโยบายคุกกี้ (Cookie Policy) | PPS Asset";
            ViewBag.SeoDescription = "นโยบายคุกกี้ของ PPS Asset";
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/CookiePolicy");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            return View();
        }

        [Route("privacy-policy")]
        public async Task<IActionResult> PrivacyPolicy()
        {
            // Set navigation data for dropdown menu
            SetNavigationData();
            
            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set SEO metadata for privacy policy page
            ViewBag.SeoTitle = "นโยบายความเป็นส่วนตัว (Privacy Policy) | PPS Asset";
            ViewBag.SeoDescription = "นโยบายความเป็นส่วนตัวของ PPS Asset";
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/PrivacyPolicy");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            return View();
        }

        public async Task<IActionResult> Project(string id = "ricco-residence-hathairat", string projectType = null, string projectName = null, string location = null)
        {
            string projectId = id;

            if (!string.IsNullOrEmpty(projectType) && !string.IsNullOrEmpty(projectName) && !string.IsNullOrEmpty(location))
            {
                projectId = ConvertPpsUrlToProjectId(projectType, projectName, location);
                _logger.LogInformation("Converted PPS URL to project ID: {ProjectType}/{ProjectName}/{Location} -> {ProjectId}", projectType, projectName, location, projectId);
            }

            var project = _projectService.GetProject(projectId);

            if (project == null)
            {
                _logger.LogWarning("Project with ID '{ProjectId}' not found", projectId);
                return NotFound($"Project '{projectId}' not found");
            }

            // Debug logging for facilities
            _logger.LogInformation("Project '{ProjectId}' loaded. Facilities count: {FacilitiesCount}", 
                projectId, project.Facilities?.Count ?? 0);
            
            if (project.Facilities?.Any() == true)
            {
                foreach (var facility in project.Facilities)
                {
                    _logger.LogInformation("Facility: {FacilityName} (ID: {FacilityId}, Icon: {Icon})", 
                        facility.Name, facility.Id, facility.Icon);
                }
            }

            ApplyTheme(projectId);

            // Set GTM ID - try project-specific first, fallback to global
            var gtmId = await _gtmService.GetGtmIdAsync(projectId);
            if (string.IsNullOrEmpty(gtmId))
            {
                gtmId = await _gtmService.GetGlobalGtmIdAsync();
            }
            ViewBag.GtmId = gtmId;

            // Set SEO metadata for project page
            var seoMetadata = _seoService.GetProjectMetadata(project);
            ViewBag.SeoTitle = seoMetadata.Title;
            ViewBag.SeoDescription = seoMetadata.Description;
            ViewBag.SeoKeywords = seoMetadata.Keywords;
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), $"/Project/{projectId}");
            ViewBag.SeoOgImage = seoMetadata.OgImage;
            ViewBag.SeoOgImageAlt = seoMetadata.OgImageAlt;

            // Set JSON-LD Property schema for this specific project
            var propertySchema = _seoService.GetJsonLdSchema("property", project);
            ViewBag.JsonLdProperty = propertySchema;

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            // Set reCAPTCHA site key
            ViewBag.RecaptchaSiteKey = _configuration.GetSection("RecaptchaSettings")["SiteKey"] ?? string.Empty;

            ViewBag.RegistrationModel = BuildRegistrationModel(project);
            ViewBag.AuthProvider = GetAuthenticatedProvider();
            ViewBag.GetProjectUrl = new Func<string, string>(ConvertProjectIdToPpsUrl);
            ViewBag.HideGlobalNavigation = true;

            return View(project);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // Temporarily disabled for testing
        public async Task<IActionResult> RegisterProject(RegistrationInputModel input)
        {
            _logger.LogInformation("=== RegisterProject called with ProjectID: {ProjectId}, FirstName: {FirstName}, LastName: {LastName}, TelNo: {TelNo} ===", 
                input.ProjectID, input.FirstName, input.LastName, input.TelNo);
            var projectId = input.ProjectID;

            if (User.Identity?.IsAuthenticated == true)
            {
                var provider = GetAuthenticatedProvider();
                input.ClientFrom ??= provider;
                input.Email ??= User.FindFirstValue(ClaimTypes.Email);
                input.FirstName ??= User.FindFirstValue(ClaimTypes.GivenName) ?? User.Identity?.Name;
                input.LastName ??= User.FindFirstValue(ClaimTypes.Surname);
            }

            // Verify reCAPTCHA token (bypass in development)
            _logger.LogInformation("Starting reCAPTCHA verification...");
            bool recaptchaValid = true; // Temporarily bypass for testing
            if (_configuration["Environment"] != "Development") 
            {
                recaptchaValid = await _recaptchaService.VerifyTokenAsync(input.RecaptchaToken ?? string.Empty);
            }
            _logger.LogInformation("reCAPTCHA verification result: {IsValid} (development bypass: {IsBypass})", 
                recaptchaValid, _configuration["Environment"] == "Development");
            if (!recaptchaValid)
            {
                _logger.LogWarning("reCAPTCHA validation failed for project {ProjectId}", projectId);

                // Check if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = false,
                        message = "reCAPTCHA ยืนยันไม่สำเร็จ กรุณาลองใหม่อีกครั้ง"
                    });
                }

                ModelState.AddModelError(string.Empty, "reCAPTCHA ยืนยันไม่สำเร็จ กรุณาลองใหม่อีกครั้ง");
            }

            // Check for duplicates only if reCAPTCHA passed
            if (recaptchaValid)
            {
                try
                {
                    _logger.LogInformation("Starting duplicate validation...");
                    var duplicateErrors = await _registrationService.CheckDuplicatesAsync(input);
                    _logger.LogInformation("Duplicate validation completed. Found {ErrorCount} errors", duplicateErrors.Count);
                    foreach (var error in duplicateErrors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Duplicate check failed for project {ProjectId}, continuing without validation", projectId);
                    // Don't add ModelState error - just log and continue
                }
            }

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model validation failed, returning errors");
                
                // Check if it's an AJAX request - return JSON with errors
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return Json(new {
                        success = false,
                        message = "กรุณาตรวจสอบข้อมูลที่กรอก",
                        errors = errors
                    });
                }

                var project = _projectService.GetProject(projectId ?? string.Empty);
                if (project == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrWhiteSpace(projectId))
                {
                    ApplyTheme(projectId);
                }
                ViewBag.RegistrationModel = input;
                ViewBag.AuthProvider = GetAuthenticatedProvider();
                return View("Project", project);
            }

            try
            {
                _logger.LogInformation("Proceeding to save registration...");
                input.UtmSource ??= Request.Query["utm_source"].ToString();
                input.UtmMedium ??= Request.Query["utm_medium"].ToString();
                input.UtmCampaign ??= Request.Query["utm_campaign"].ToString();
                input.UtmTerm ??= Request.Query["utm_term"].ToString();
                input.UtmContent ??= Request.Query["utm_content"].ToString();

                await _registrationService.SaveAsync(input, Request);
                _logger.LogInformation("Registration saved successfully");

                // Check if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var project = _projectService.GetProject(input.ProjectID ?? string.Empty);
                    return Json(new {
                        success = true,
                        message = "ลงทะเบียนสำเร็จ",
                        projectName = project?.NameTh ?? project?.Name ?? "โครงการของเรา"
                    });
                }

                return RedirectToAction(nameof(RegistrationSuccess), new { projectId = input.ProjectID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register enquiry for project {ProjectId}", projectId);
                ModelState.AddModelError(string.Empty, "ไม่สามารถบันทึกข้อมูลได้ กรุณาลองใหม่อีกครั้ง");

                // Check if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = false,
                        message = "ไม่สามารถบันทึกข้อมูลได้ กรุณาลองใหม่อีกครั้ง"
                    });
                }

                var project = _projectService.GetProject(projectId ?? string.Empty);
                if (project == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrWhiteSpace(projectId))
                {
                    ApplyTheme(projectId);
                }
                ViewBag.RegistrationModel = input;
                ViewBag.AuthProvider = GetAuthenticatedProvider();
                return View("Project", project);
            }
        }

        private string ConvertPpsUrlToProjectId(string projectType, string projectName, string location)
        {
            // Convert URL-friendly format to our project ID format using database mapping
            // Example: singlehouse/thericcoresidence/ramindrachatuchot -> ricco-residence-chatuchot

            try
            {
                var urlPattern = $"{projectType}/{projectName}/{location}";
                _logger.LogInformation("Looking up URL pattern: {UrlPattern}", urlPattern);

                // Query the database for the mapping
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                        SELECT ProjectID
                        FROM sy_project_mapping
                        WHERE UrlPattern = @UrlPattern
                        AND IsActive = 1
                        LIMIT 1";

                    var result = connection.QueryFirstOrDefault<string>(query, new { UrlPattern = urlPattern });

                    if (!string.IsNullOrEmpty(result))
                    {
                        _logger.LogInformation("Successfully mapped URL {UrlPattern} to project ID: {ProjectId}", urlPattern, result);
                        return result;
                    }
                    else
                    {
                        _logger.LogWarning("No mapping found in database for URL pattern: {UrlPattern}", urlPattern);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error converting PPS URL to project ID: {ex.Message}");
            }

            // Fallback to static service if database lookup fails
            _logger.LogWarning($"No mapping found for URL pattern: {projectType}/{projectName}/{location}");
            return null; // Let the caller handle null (instead of wrong default)
        }

        private string ConvertProjectIdToPpsUrl(string projectId)
        {
            // Convert project ID back to PPS Asset format using database mapping
            // Example: ricco-residence-chatuchot -> /singlehouse/thericcoresidence/ramindrachatuchot

            try
            {
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                        SELECT UrlPattern
                        FROM sy_project_mapping
                        WHERE ProjectID = @ProjectId
                        AND IsActive = 1
                        LIMIT 1";

                    var result = connection.QueryFirstOrDefault<string>(query, new { ProjectId = projectId });

                    if (!string.IsNullOrEmpty(result))
                    {
                        return $"/{result}";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error converting project ID to PPS URL: {ex.Message}");
            }

            // Fallback: return null and let caller handle missing mapping
            _logger.LogWarning($"No URL pattern found for project ID: {projectId}");
            return null;
        }

        private void ApplyTheme(string projectId)
        {
            var theme = _themeService.GetProjectTheme(projectId);
            ViewBag.ProjectTheme = theme.CssClass;
            ViewBag.ThemePrimaryColor = theme.PrimaryColor;
            ViewBag.ThemeSecondaryColor = theme.SecondaryColor;
            ViewBag.ThemeLightBackground = theme.LightBackground;
        }

        private RegistrationInputModel BuildRegistrationModel(ProjectViewModel project)
        {
            var provider = GetAuthenticatedProvider();
            var givenName = User.FindFirstValue(ClaimTypes.GivenName);
            var surname = User.FindFirstValue(ClaimTypes.Surname);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return new RegistrationInputModel
            {
                ProjectID = project.Id,
                ProjectName = string.IsNullOrWhiteSpace(project.NameTh) ? project.Name : project.NameTh,
                FirstName = string.IsNullOrWhiteSpace(givenName) ? User.Identity?.Name : givenName,
                LastName = surname,
                Email = email,
                ClientFrom = provider,
                UtmSource = Request.Query["utm_source"].ToString(),
                UtmMedium = Request.Query["utm_medium"].ToString(),
                UtmCampaign = Request.Query["utm_campaign"].ToString(),
                UtmTerm = Request.Query["utm_term"].ToString(),
                UtmContent = Request.Query["utm_content"].ToString()
            };
        }

        private string? GetAuthenticatedProvider()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var claims = User.Claims ?? Enumerable.Empty<Claim>();
            if (claims.Any(c => c.Type.StartsWith("urn:facebook", StringComparison.OrdinalIgnoreCase)))
            {
                return "Facebook";
            }

            if (claims.Any(c => c.Type.StartsWith("urn:google", StringComparison.OrdinalIgnoreCase)))
            {
                return "Google";
            }

            return null;
        }

        

        [HttpGet]
        public async Task<IActionResult> FixProjectNames()
        {
            if (!_configuration.GetValue<bool>("EnableDatabaseMigration"))
            {
                return NotFound();
            }

            try
            {
                _logger.LogInformation("Re-migrating static data to fix corrupted project names");
                
                var result = await _databaseMigration.MigrateStaticDataAsync();

                return Json(new { Success = result.Success, Message = result.Success ? "Project names fixed" : "Migration failed", Error = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing project names");
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public async Task<IActionResult> RegistrationSuccess(string? projectId = null)
        {
            string? backgroundImage = null;
            ProjectViewModel? project = null;

            if (!string.IsNullOrEmpty(projectId))
            {
                project = _projectService.GetProject(projectId);
                if (project != null)
                {
                    _logger.LogInformation("RegistrationSuccess: Project found - {ProjectId}", project.Id);
                    _logger.LogInformation("RegistrationSuccess: Hero={Hero}, Thumbnail={Thumbnail}, GalleryCount={GalleryCount}",
                        project.Images?.Hero, project.Images?.Thumbnail, project.Images?.Gallery?.Count ?? 0);

                    // Try to get Hero image first, then fallback to thumbnail or first gallery image
                    if (!string.IsNullOrEmpty(project.Images?.Hero))
                    {
                        backgroundImage = Url.Content($"~/images/projects/{project.Id}/{project.Images.Hero}");
                        _logger.LogInformation("RegistrationSuccess: Using Hero image - {BackgroundImage}", backgroundImage);
                    }
                    else if (!string.IsNullOrEmpty(project.Images?.Thumbnail))
                    {
                        backgroundImage = Url.Content($"~/images/projects/{project.Id}/{project.Images.Thumbnail}");
                        _logger.LogInformation("RegistrationSuccess: Using Thumbnail image - {BackgroundImage}", backgroundImage);
                    }
                    else if (project.Images?.Gallery?.Any() == true)
                    {
                        backgroundImage = Url.Content($"~/images/projects/{project.Id}/galleries/{project.Images.Gallery.First()}");
                        _logger.LogInformation("RegistrationSuccess: Using Gallery image - {BackgroundImage}", backgroundImage);
                    }
                    else
                    {
                        _logger.LogWarning("RegistrationSuccess: No images found for project {ProjectId}", project.Id);
                    }
                }
                else
                {
                    _logger.LogWarning("RegistrationSuccess: Project not found for projectId {ProjectId}", projectId);
                }
            }

            // Set GTM ID - try project-specific first, fallback to global
            string? gtmId = null;
            if (!string.IsNullOrEmpty(projectId))
            {
                gtmId = await _gtmService.GetGtmIdAsync(projectId);
            }
            if (string.IsNullOrEmpty(gtmId))
            {
                gtmId = await _gtmService.GetGlobalGtmIdAsync();
            }
            ViewBag.GtmId = gtmId;

            ViewBag.BackgroundImage = backgroundImage;
            ViewBag.Project = project;
            return View();
        }

       

        /// <summary>
        /// Service status endpoint for monitoring
        /// </summary>
        public IActionResult ServiceStatus()
        {
            try
            {
                // Check if the service is a DatabaseProjectService
                if (_projectService is DatabaseProjectService databaseService)
                {
                    var projects = databaseService.GetAllProjects();
                    return Ok(new
                    {
                        IsDatabaseAvailable = true,
                        DatabaseProjectCount = projects.Count,
                        StaticProjectCount = 0,
                        CurrentSource = "Database",
                        LastChecked = DateTime.UtcNow
                    });
                }

                // Fallback for other service types (e.g., StaticProjectService)
                var allProjects = _projectService.GetAllProjects();
                return Ok(new
                {
                    IsDatabaseAvailable = false,
                    DatabaseProjectCount = 0,
                    StaticProjectCount = allProjects.Count,
                    CurrentSource = "Static",
                    LastChecked = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service status");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        

        /// <summary>
        /// Get project status summary endpoint
        /// </summary>
        public async Task<IActionResult> ProjectStatusSummary()
        {
            try
            {
                _logger.LogDebug("Retrieving project status summary");

                var summary = await _projectService.GetProjectStatusSummaryAsync();

                return Ok(new
                {
                    success = true,
                    data = summary,
                    count = summary.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project status summary");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error retrieving status summary",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get project status history endpoint
        /// </summary>
        public async Task<IActionResult> ProjectStatusHistory(string projectId)
        {
            try
            {
                if (string.IsNullOrEmpty(projectId))
                {
                    return BadRequest(new { success = false, message = "ProjectId is required" });
                }

                _logger.LogDebug("Retrieving project status history for {ProjectId}", projectId);

                var history = await _projectService.GetProjectStatusHistoryAsync(projectId);

                return Ok(new
                {
                    success = true,
                    projectId = projectId,
                    history = history,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project status history for {ProjectId}", projectId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error retrieving status history",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Debug endpoint to test database connection and investigate issues - TEMPORARY
        /// </summary>
        public IActionResult DebugConnection()
        {
            var results = new List<object>();
            
            try
            {
                var connectionString = GetConnectionString();
                results.Add(new { step = "connection_string", value = connectionString.Replace("Pwd=", "Pwd=***") });

                using (var connection = new MySqlConnection(connectionString))
                {
                    // Test basic connection
                    connection.Open();
                    results.Add(new { step = "connection_open", success = true });

                    // Test database selection
                    var dbName = connection.QueryFirstOrDefault<string>("SELECT DATABASE()");
                    results.Add(new { step = "current_database", value = dbName });

                    // Test server info
                    var serverInfo = connection.QueryFirstOrDefault<string>("SELECT VERSION()");
                    results.Add(new { step = "server_version", value = serverInfo });

                    // Test if tables exist
                    var tables = connection.Query<string>("SHOW TABLES LIKE 'sy_project%'");
                    results.Add(new { step = "available_tables", value = tables.ToArray() });

                    // Test specific project query with full error handling
                    try 
                    {
                        const string fullProjectSql = @"
                            SELECT
                                p.ProjectID as Id,
                                p.ProjectName as NameTh,
                                p.ProjectNameEN as NameEn,
                                p.ProjectSubtitle as Subtitle,
                                p.ProjectDescription as Description,
                                p.ProjectConcept as Concept,
                                p.ProjectType as Type,
                                p.ProjectStatus as Status,
                                p.SortOrder,
                                p.ProjectAddress as Location,
                                p.ProjectSize,
                                p.TotalUnits,
                                p.LandSize,
                                p.UsableArea,
                                p.Developer,
                                p.PriceRange
                            FROM sy_project p
                            WHERE p.ProjectID = @ProjectId";

                        var project = connection.QueryFirstOrDefault<dynamic>(fullProjectSql, new { ProjectId = "ricco-town-phahonyothin-saimai53" });
                        results.Add(new { step = "full_project_query", success = project != null, hasData = project != null });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new { step = "full_project_query", success = false, error = ex.Message, sqlState = ex.Data.Contains("SqlState") ? ex.Data["SqlState"] : null });
                    }

                    // Test if we can access the exact service
                    try
                    {
                        var project = _projectService.GetProject("ricco-town-phahonyothin-saimai53");
                        results.Add(new { step = "service_getproject", success = project != null, projectFound = project != null });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new { step = "service_getproject", success = false, error = ex.Message, innerException = ex.InnerException?.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(new { step = "connection_failed", error = ex.Message, type = ex.GetType().Name, innerException = ex.InnerException?.Message });
            }

            return Json(new
            {
                success = true,
                results = results,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Debug endpoint to test URL mapping - TEMPORARY
        /// </summary>
        public IActionResult DebugUrlMapping(string urlPattern = "townhome/thericcotown/phahonyothin_saimai53")
        {
            try
            {
                _logger.LogInformation("Testing URL mapping for: {UrlPattern}", urlPattern);
                
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                        SELECT ProjectID, MappedProjectID, UrlPattern, IsActive
                        FROM sy_project_mapping
                        WHERE UrlPattern = @UrlPattern
                        LIMIT 1";

                    var result = connection.QueryFirstOrDefault(query, new { UrlPattern = urlPattern });
                    
                    return Json(new
                    {
                        success = true,
                        connectionString = GetConnectionString().Replace("Pwd=", "Pwd=***"),
                        searchPattern = urlPattern,
                        result = result,
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    connectionString = GetConnectionString().Replace("Pwd=", "Pwd=***")
                });
            }
        }

        /// <summary>
        /// Debug endpoint to test each step of project loading - TEMPORARY
        /// </summary>
        public IActionResult DebugProjectSteps(string projectId = "ricco-town-phahonyothin-saimai53")
        {
            try
            {
                var steps = new List<object>();
                
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    steps.Add(new { step = "connection", success = true });

                    // Test main project query
                    const string projectSql = @"
                        SELECT
                            p.ProjectID as Id,
                            p.ProjectName as NameTh,
                            p.ProjectNameEN as NameEn,
                            p.ProjectType as Type,
                            p.ProjectStatus as Status
                        FROM sy_project p
                        WHERE p.ProjectID = @ProjectId";

                    var project = connection.QueryFirstOrDefault<dynamic>(projectSql, new { ProjectId = projectId });
                    steps.Add(new { step = "main_query", success = project != null, data = project });

                    if (project != null)
                    {
                        // Test each related data loading step
                        try
                        {
                            var images = connection.Query("SELECT * FROM sy_project_images WHERE ProjectID = @ProjectId", new { ProjectId = projectId });
                            steps.Add(new { step = "images", success = true, count = images.Count() });
                        }
                        catch (Exception ex)
                        {
                            steps.Add(new { step = "images", success = false, error = ex.Message });
                        }

                        try
                        {
                            var houseTypes = connection.Query("SELECT * FROM sy_project_house_types WHERE ProjectID = @ProjectId", new { ProjectId = projectId });
                            steps.Add(new { step = "house_types", success = true, count = houseTypes.Count() });
                        }
                        catch (Exception ex)
                        {
                            steps.Add(new { step = "house_types", success = false, error = ex.Message });
                        }

                        try
                        {
                            var facilities = connection.Query("SELECT * FROM sy_project_facilities WHERE ProjectID = @ProjectId", new { ProjectId = projectId });
                            steps.Add(new { step = "facilities", success = true, count = facilities.Count() });
                        }
                        catch (Exception ex)
                        {
                            steps.Add(new { step = "facilities", success = false, error = ex.Message });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    projectId = projectId,
                    steps = steps,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    projectId = projectId
                });
            }
        }

        /// <summary>
        /// Debug raw SQL query to see what's in database - TEMPORARY
        /// </summary>
        public IActionResult DebugRawSql(string projectId = "ricco-town-phahonyothin-saimai53")
        {
            try
            {
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    
                    // Test the exact query from DatabaseProjectService
                    const string exactQuery = @"
                        SELECT
                            p.ProjectID as Id,
                            p.ProjectName as NameTh,
                            p.ProjectNameEN as NameEn,
                            p.ProjectSubtitle as Subtitle,
                            p.ProjectDescription as Description,
                            p.ProjectConcept as Concept,
                            p.ProjectType as Type,
                            p.ProjectStatus as Status,
                            p.SortOrder,
                            p.ProjectAddress as Location,
                            p.ProjectSize,
                            p.TotalUnits,
                            p.LandSize,
                            p.UsableArea,
                            p.Developer,
                            p.PriceRange
                        FROM sy_project p
                        WHERE p.ProjectID = @ProjectId";

                    var rawResult = connection.QueryFirstOrDefault<dynamic>(exactQuery, new { ProjectId = projectId });
                    
                    // Also test what service returns
                    var serviceResult = _projectService.GetProject(projectId);
                    
                    return Json(new
                    {
                        success = true,
                        projectId = projectId,
                        rawSqlFound = rawResult != null,
                        rawSqlData = rawResult,
                        serviceFound = serviceResult != null,
                        serviceData = serviceResult != null ? new { serviceResult.Id, serviceResult.Name } : null,
                        connectionString = GetConnectionString().Replace("Pwd=", "Pwd=***"),
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    projectId = projectId
                });
            }
        }

        /// <summary>
        /// Debug endpoint to check if project exists in database - TEMPORARY
        /// </summary>
        public IActionResult DebugProjectExists(string projectId = "ricco-town-phahonyothin-saimai53")
        {
            try
            {
                var project = _projectService.GetProject(projectId);
                
                return Json(new
                {
                    success = true,
                    projectId = projectId,
                    projectFound = project != null,
                    projectData = project != null ? new {
                        project.Id,
                        project.Name,
                        project.NameTh,
                        project.Type,
                        project.Status
                    } : null,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    projectId = projectId
                });
            }
        }

        /// <summary>
        /// Debug endpoint to check project data - development only
        /// </summary>
        public IActionResult DebugProject(string projectId = "ricco-residence-hathairat")
        {
            // Only allow in development environment for security
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                var project = _projectService.GetProject(projectId);

                if (project == null)
                {
                    return Json(new
                    {
                        error = "Project not found",
                        projectId
                    });
                }

                return Json(new
                {
                    projectId,
                    id = project.Id,
                    name = project.Name,
                    nameTh = project.NameTh,
                    nameEn = project.NameEn,
                    subtitle = project.Subtitle,
                    description = project.Description,
                    concept = project.Concept,
                    type = project.Type.ToString(),
                    status = project.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    projectId
                });
            }
        }

        /// <summary>
        /// Debug endpoint to check what gallery images are in the database - development only
        /// </summary>
        public async Task<IActionResult> DebugGalleryImages(string projectId = "ricco-residence-hathairat")
        {
            // Only allow in development environment for security
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                // Create a connection to directly query the database
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ImageType, ImagePath, SortOrder, Category
                    FROM sy_project_images
                    WHERE ProjectID = @ProjectId
                    ORDER BY ImageType, SortOrder";

                var images = await connection.QueryAsync<dynamic>(sql, new { ProjectId = projectId });

                return Json(new
                {
                    projectId,
                    totalImages = images.Count(),
                    allImages = images.Select(img => new
                    {
                        ImageType = img.ImageType?.ToString(),
                        ImagePath = img.ImagePath?.ToString(),
                        SortOrder = img.SortOrder,
                        Category = img.Category?.ToString() // This might be null if column doesn't exist
                    }),
                    galleryImages = images.Where(img => img.ImageType?.ToString() == "Gallery").Select(img => new
                    {
                        ImagePath = img.ImagePath?.ToString(),
                        SortOrder = img.SortOrder,
                        Category = img.Category?.ToString()
                    }),
                    galleryCount = images.Count(img => img.ImageType?.ToString() == "Gallery")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message,
                    projectId
                });
            }
        }

        public async Task<IActionResult> Terms(string id = null, string projectType = null, string projectName = null, string location = null)
        {
            string? projectId = id;

            // Handle PPS URL format conversion
            if (!string.IsNullOrEmpty(projectType) && !string.IsNullOrEmpty(projectName) && !string.IsNullOrEmpty(location))
            {
                projectId = ConvertPpsUrlToProjectId(projectType, projectName, location);
                _logger.LogInformation("Converted PPS URL to project ID for terms: {ProjectType}/{ProjectName}/{Location} -> {ProjectId}", 
                    projectType, projectName, location, projectId);
            }

            // Set navigation data for dropdown menu
            SetNavigationData();

            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set default SEO metadata for terms page
            var seoMetadata = _seoService.GetPageMetadata("terms");
            ViewBag.SeoTitle = seoMetadata.Title;
            ViewBag.SeoDescription = seoMetadata.Description;
            ViewBag.SeoKeywords = seoMetadata.Keywords;
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/terms-and-conditions");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            // Create default general terms if no project specified
            if (string.IsNullOrEmpty(projectId))
            {
                ViewBag.SeoTitle = "ข้อกำหนดและเงื่อนไข โครงการที่อยู่อาศัย | PPS Asset";
                
                // Create general terms model
                var generalTerms = new TermsAndConditions
                {
                    ProjectTitle = "ข้อกำหนดและเงื่อนไข โครงการที่อยู่อาศัย",
                    Company = new CompanyInfo(), // Uses default values from model
                    DisclaimerText = "ภาพทั้งหมดเป็นภาพและบรรยากาศจำลองใช้เพื่อการโฆษณาเท่านั้น",
                    ReservationTerms = "บริษัทฯ ขอสงวนสิทธิ์ในการเปลี่ยนแปลงข้อมูลรายละเอียดและข้อความต่างๆ โดยไม่ต้องแจ้งให้ทราบล่วงหน้า"
                };
                
                ViewBag.Project = null;
                return View(generalTerms);
            }

            // Get specific project for terms
            var project = _projectService.GetProject(projectId);
            if (project == null)
            {
                _logger.LogWarning("Project with ID '{ProjectId}' not found for terms page", projectId);
                
                // Return general terms if project not found
                ViewBag.SeoTitle = "ข้อกำหนดและเงื่อนไข โครงการที่อยู่อาศัย | PPS Asset";
                
                var generalTerms = new TermsAndConditions
                {
                    ProjectTitle = "ข้อกำหนดและเงื่อนไข โครงการที่อยู่อาศัย",
                    Company = new CompanyInfo(),
                    DisclaimerText = "ภาพทั้งหมดเป็นภาพและบรรยากาศจำลองใช้เพื่อการโฆษณาเท่านั้น",
                    ReservationTerms = "บริษัทฯ ขอสงวนสิทธิ์ในการเปลี่ยนแปลงข้อมูลรายละเอียดและข้อความต่างๆ โดยไม่ต้องแจ้งให้ทราบล่วงหน้า"
                };
                
                ViewBag.Project = null;
                return View(generalTerms);
            }

            // Apply project theme
            ApplyTheme(projectId);

            // Get project-specific terms and conditions
            var termsData = GetProjectTermsAndConditions(projectId, project);
            
            // Update SEO metadata for specific project terms
            ViewBag.SeoTitle = $"ข้อกำหนดและเงื่อนไข {project.NameTh ?? project.Name} | PPS Asset";
            ViewBag.SeoDescription = $"ข้อกำหนดและเงื่อนไขสำหรับโครงการ {project.NameTh ?? project.Name} โดยบริษัท พูลผลทรัพย์ จำกัด";

            ViewBag.Project = project;
            ViewBag.ProjectId = projectId; // Set the ProjectId for conditional rendering

            return View(termsData);
        }

        private TermsAndConditions GetProjectTermsAndConditions(string projectId, ProjectViewModel project)
        {
            // This method will retrieve project-specific terms from the database or static data
            // For now, we'll use the extracted data from the HTML file and make it dynamic based on project
            
            var terms = new TermsAndConditions
            {
                ProjectTitle = $"ข้อกำหนดและเงื่อนไข โครงการ {project.NameTh ?? project.Name}",
                Company = new CompanyInfo(), // Uses default values
                DisclaimerText = "ภาพทั้งหมดเป็นภาพและบรรยากาศจำลองใช้เพื่อการโฆษณาเท่านั้น"
            };

            // Set project-specific data based on projectId
            switch (projectId?.ToLower())
            {
                case "ricco-town-wongwaen-lumlukka":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "399", "9593", "9601", "9605", "9606", "151433", "151435" },
                        Address = "ตำบลลาดสวาย อำเภอลำลูกกา จังหวัดปทุมธานี"
                    };
                    terms.ProjectArea = "43 ไร่ 0 งาน 15.43 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 399, 92373, 192374, 192375, 192376, 192377 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย บ้านเดี่ยว 2 ชั้น 3 ยูนิต, บ้านแฝด 2 ชั้น 154 ยูนิต, ทาวน์โฮม 2 ชั้น 174 ยูนิต รวมทั้งหมด 331 ยูนิต";
                    terms.ConstructionStart = new DateTime(2021, 1, 1);
                    terms.ExpectedCompletion = new DateTime(2025, 12, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร เกียรตินาคินภัทร จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 2,490,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้านแบบ 4 ห้องนอน พื้นที่ใช้สอย 119 ตร.ม. มีเพียง 2 ยูนิต คือ แปลง M9, M10";
                    break;

                case "ricco-residence-ramintra-hathairat":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "84056", "120363", "120364" },
                        Address = "ถนนหทัยราษฏร์ แขวงสามวาตะวันตก เขตคลองสามวา กรุงเทพมหานคร"
                    };
                    terms.ProjectArea = "13 ไร่ 2 งาน 58.5 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 124205, 124206, 124207, 124208, 124146 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย บ้านเดี่ยว 2 ชั้น ทั้งหมด 58 ยูนิต";
                    terms.ConstructionStart = new DateTime(2021, 8, 1);
                    terms.ExpectedCompletion = new DateTime(2025, 12, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร ทหารไทยธนชาต จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 5,790,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้านแบบ 3-4 ห้องนอน พื้นที่ใช้สอย 152 ตร.ม. มีเพียง 1 ยูนิต คือ แปลง D4";
                    break;

                case "ricco-residence-prime-wongwaen-chatuchot":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "48916", "48918", "48919", "48920" },
                        Address = "ถนนคู่ขนานเลียบวงแหวนฯกาญจนาภิเษก แขวงออเงิน เขตสายไหม กรุงเทพมหานคร"
                    };
                    terms.ProjectArea = "37 ไร่ 1 งาน 92.3 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 53789, 53880, 53881, 53882, 53722 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย บ้านเดี่ยว 2 ชั้น ทั้งหมด 156 ยูนิต";
                    terms.ConstructionStart = new DateTime(2022, 5, 1);
                    terms.ExpectedCompletion = new DateTime(2026, 12, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร ทหารไทยธนชาติ จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 6,590,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้านแบบ 4 ห้องนอน พื้นที่ใช้สอย 162 ตร.ม. มีเพียง 1 ยูนิต คือ แปลง H1";
                    break;

                case "ricco-town-phahonyothin-saimai53":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "27404", "44213", "44214", "44215", "44244", "44245", "44246" },
                        Address = "ถนนสายไหม แขวงสายไหม เขตบางเขน กรุงเทพมหานคร"
                    };
                    terms.ProjectArea = "22 ไร่ 1 งาน 68.40 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 49451, 49452, 49453, 49454, 49217 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย ทาวน์เฮ้าส์ 2 ชั้น ทั้งหมด 233 ยูนิต";
                    terms.ConstructionStart = new DateTime(2021, 3, 1);
                    terms.ExpectedCompletion = new DateTime(2025, 12, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร กสิกรไทย จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 2,990,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้าน 1 แบบ พื้นที่ใช้สอย 113 ตร.ม. มีเพียง 2 ยูนิต คือ แปลง N04, N05";
                    break;

                case "ricco-residence-ramintra-chatuchot":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "107567", "120411", "120412", "120413", "120414", "120415", "120416", "120417", "120177", "120178", "120188" },
                        Address = "ถนนหนองระแหง แขวงสามวาตะวันตก เขตคลองสามวา กรุงเทพมหานคร"
                    };
                    terms.ProjectArea = "35 ไร่ 1 งาน 74.6 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 123461, 123462, 123463, 123464, 123465, 123317 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย บ้านเดี่ยว 2 ชั้น ทั้งหมด 143 ยูนิต";
                    terms.ConstructionStart = new DateTime(2023, 2, 1);
                    terms.ExpectedCompletion = new DateTime(2025, 12, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร เกียรตินาคินภัทร จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 5,590,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้าน 1 แบบ พื้นที่ใช้สอย 145 ตร.ม. มีเพียง 2 ยูนิต คือ แปลง E01, M06";
                    break;

                case "ricco-residence-prime-wongwaen-hathairat":
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        LandTitleNumbers = new List<string> { "112437", "117324", "117325", "117326", "117327", "117328", "117329", "117330", "117331" },
                        Address = "ถนนไทยรามัญ แขวงสามวาตะวันตก เขตคลองสามวา กรุงเทพมหานคร"
                    };
                    terms.ProjectArea = "24 ไร่ 2 งาน 38.4 ตารางวา";
                    terms.CommonAreaDetails = "โฉนดเลขที่ 119158, 119159, 119160, 119161, 119054 ถือเป็นทรัพย์ส่วนกลางร่วม";
                    terms.ProjectDetails = "ที่พักอาศัย บ้านเดี่ยว 2 ชั้น ทั้งหมด 103 ยูนิต";
                    terms.ConstructionStart = new DateTime(2022, 9, 1);
                    terms.ExpectedCompletion = new DateTime(2026, 5, 31);
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคาร ทหารไทยธนชาต จำกัด" };
                    terms.PromotionalPrice = "ราคาเริ่มต้น 6,190,000 บาท";
                    terms.PromotionalDetails = "เป็นราคาหลังหักโปรโมชั่นแล้ว บ้าน 1 แบบ พื้นที่ใช้สอย 180 ตร.ม. มีเพียง 2 ยูนิต คือ แปลง K05, L06";
                    break;

                default:
                    // Default terms for projects not specifically defined
                    terms.ProjectLocation = new ProjectLocationDetails
                    {
                        Address = project.Location?.Address ?? "กรุงเทพมหานคร"
                    };
                    terms.ProjectDetails = $"โครงการ{project.NameTh ?? project.Name}";
                    terms.BankObligation = new BankObligationInfo { BankName = "ธนาคารพันธมิตร" };
                    break;
            }

            // Add common reservation terms
            terms.ReservationTerms = "บริษัทฯ ขอสงวนสิทธิ์ในการเปลี่ยนแปลงข้อมูลรายละเอียดและข้อความต่างๆ โดยไม่ต้องแจ้งให้ทราบล่วงหน้า และโปรโมชั่นดังกล่าว ไม่สามารถรวมกับโปรโมชั่นส่งเสริมการขายอื่นๆได้อีก";

            return terms;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
