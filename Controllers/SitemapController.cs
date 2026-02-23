using Microsoft.AspNetCore.Mvc;
using System.Text;
using PPSAsset.Services;

namespace PPSAsset.Controllers
{
    /// <summary>
    /// Controller for handling SEO sitemap generation
    /// </summary>
    public class SitemapController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<SitemapController> _logger;

        public SitemapController(IProjectService projectService, ILogger<SitemapController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Generates XML sitemap for search engines
        /// </summary>
        [HttpGet("/sitemap.xml")]
        [Produces("application/xml")]
        public IActionResult Index()
        {
            try
            {
                var xml = GenerateSitemap();
                return Content(xml, "application/xml", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sitemap");
                return StatusCode(500, "Error generating sitemap");
            }
        }

        /// <summary>
        /// Generates the XML sitemap with all URLs
        /// </summary>
        private string GenerateSitemap()
        {
            var baseUrl = GetBaseUrl();
            var sb = new StringBuilder();

            // XML declaration
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\">");

            // Static pages with high priority
            var staticPages = new List<(string path, string priority, string changefreq)>
            {
                ("/", "1.0", "daily"),              // Homepage
                ("/About", "0.9", "monthly"),       // About page
                ("/Contact", "0.8", "monthly"),     // Contact page
            };

            foreach (var page in staticPages)
            {
                AddUrl(sb, baseUrl, page.path, page.priority, page.changefreq);
            }

            // Add all projects
            try
            {
                var projects = _projectService.GetAvailableProjects();
                if (projects != null && projects.Count > 0)
                {
                    foreach (var project in projects)
                    {
                        // Add project detail page
                        var projectUrl = $"/Project/{project.Id}";
                        AddUrl(sb, baseUrl, projectUrl, "0.8", "weekly", project.Images?.Hero);

                        // Add PPS Asset format URLs
                        var projectType = GetProjectType(project.Type.ToString());
                        if (!string.IsNullOrEmpty(projectType))
                        {
                            var projectName = ConvertProjectIdToUrl(project.Id);
                            var location = !string.IsNullOrEmpty(project.Location?.District)
                                ? project.Location.District.ToLower().Replace(" ", "-")
                                : "bangkok";

                            var ppsUrl = $"/{projectType}/{projectName}/{location}";
                            AddUrl(sb, baseUrl, ppsUrl, "0.8", "weekly", project.Images?.Hero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding projects to sitemap");
            }

            // Add property type category pages
            var propertyTypes = new List<(string path, string label)>
            {
                ("/singlehouse", "Single Houses"),
                ("/townhome", "Townhomes"),
                ("/twinhouse", "Twin Houses")
            };

            foreach (var type in propertyTypes)
            {
                AddUrl(sb, baseUrl, type.path, "0.7", "weekly");
            }

            sb.AppendLine("</urlset>");

            return sb.ToString();
        }

        /// <summary>
        /// Adds a URL entry to the sitemap XML
        /// </summary>
        private void AddUrl(StringBuilder sb, string baseUrl, string path, string priority, string changefreq, string imageUrl = null)
        {
            var fullUrl = $"{baseUrl.TrimEnd('/')}{path}";

            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{System.Net.WebUtility.HtmlEncode(fullUrl)}</loc>");
            sb.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{priority}</priority>");

            // Add image if provided
            if (!string.IsNullOrEmpty(imageUrl))
            {
                sb.AppendLine("    <image:image>");
                sb.AppendLine($"      <image:loc>{System.Net.WebUtility.HtmlEncode(imageUrl)}</image:loc>");
                sb.AppendLine("    </image:image>");
            }

            sb.AppendLine("  </url>");
        }

        /// <summary>
        /// Gets the base URL from the current request
        /// </summary>
        private string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return baseUrl;
        }

        /// <summary>
        /// Converts project type enum to URL segment
        /// </summary>
        private string GetProjectType(string projectType)
        {
            return projectType?.ToLower() switch
            {
                "singlehouse" => "singlehouse",
                "townhome" => "townhome",
                "twinhouse" => "twinhouse",
                _ => null
            };
        }

        /// <summary>
        /// Converts project ID to URL-friendly format
        /// </summary>
        private string ConvertProjectIdToUrl(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return "project";

            return projectId.ToLower()
                .Replace(" ", "-")
                .Replace("_", "-");
        }
    }
}
