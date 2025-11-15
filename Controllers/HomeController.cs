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

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IActionResult> Index()
        {
            // Homepage uses default theme from theme service
            var theme = _themeService.GetDefaultTheme();
            ViewBag.ProjectTheme = theme.CssClass;
            ViewBag.ThemePrimaryColor = theme.PrimaryColor;
            ViewBag.ThemeSecondaryColor = theme.SecondaryColor;
            ViewBag.ThemeLightBackground = theme.LightBackground;

            // Set GTM ID for global site
            ViewBag.GtmId = await _gtmService.GetGlobalGtmIdAsync();

            // Set SEO metadata for homepage
            var seoMetadata = _seoService.GetPageMetadata("home");
            ViewBag.SeoTitle = seoMetadata.Title;
            ViewBag.SeoDescription = seoMetadata.Description;
            ViewBag.SeoKeywords = seoMetadata.Keywords;
            ViewBag.SeoCanonical = _seoService.GetCanonicalUrl(Request.Host.ToString(), "/");

            // Set JSON-LD Organization schema
            ViewBag.JsonLdOrganization = _seoService.GetOrganizationSchema();

            // Get available projects using the simplified DatabaseProjectService
            var featuredProjects = _projectService.GetAvailableProjects();

            return View(featuredProjects);
        }

        public async Task<IActionResult> About()
        {
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

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterProject(RegistrationInputModel input)
        {
            var projectId = input.ProjectID;

            if (User.Identity?.IsAuthenticated == true)
            {
                var provider = GetAuthenticatedProvider();
                input.ClientFrom ??= provider;
                input.Email ??= User.FindFirstValue(ClaimTypes.Email);
                input.FirstName ??= User.FindFirstValue(ClaimTypes.GivenName) ?? User.Identity?.Name;
                input.LastName ??= User.FindFirstValue(ClaimTypes.Surname);
            }

            // Verify reCAPTCHA token
            bool recaptchaValid = await _recaptchaService.VerifyTokenAsync(input.RecaptchaToken ?? string.Empty);
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

            if (!ModelState.IsValid)
            {
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
                input.UtmSource ??= Request.Query["utm_source"].ToString();
                input.UtmMedium ??= Request.Query["utm_medium"].ToString();
                input.UtmCampaign ??= Request.Query["utm_campaign"].ToString();
                input.UtmTerm ??= Request.Query["utm_term"].ToString();
                input.UtmContent ??= Request.Query["utm_content"].ToString();

                await _registrationService.SaveAsync(input, Request);

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
                        _logger.LogInformation($"Converted PPS URL to project ID: {urlPattern} -> {result}");
                        return result;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
