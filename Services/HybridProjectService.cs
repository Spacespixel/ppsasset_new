using PPSAsset.Models;
using System.IO;

namespace PPSAsset.Services
{
    /// <summary>
    /// Hybrid service that tries database first, falls back to static data for reliability
    /// This ensures zero downtime during database issues or migrations
    /// </summary>
    public class HybridProjectService : IProjectService
    {
        private readonly DatabaseProjectService _databaseService;
        private readonly StaticProjectService _staticService;
        private readonly ILogger<HybridProjectService> _logger;

        public HybridProjectService(
            DatabaseProjectService databaseService,
            StaticProjectService staticService,
            ILogger<HybridProjectService> logger)
        {
            _databaseService = databaseService;
            _staticService = staticService;
            _logger = logger;
        }

        public ProjectViewModel? GetProject(string id)
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve project {ProjectId} from database", id);

                var project = _databaseService.GetProject(id);

                if (project != null)
                {
                    _logger.LogDebug("Successfully retrieved project {ProjectId} from database", id);
                    return EnhanceProjectWithGallery(project);
                }

                _logger.LogInformation("Project {ProjectId} not found in database, falling back to static data", id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving project {ProjectId}, falling back to static data", id);
            }

            // Fallback to static service
            try
            {
                var staticProject = _staticService.GetProject(id);
                if (staticProject != null)
                {
                    _logger.LogInformation("Successfully retrieved project {ProjectId} from static fallback", id);
                    return EnhanceProjectWithGallery(staticProject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId} from static fallback", id);
            }

            _logger.LogWarning("Project {ProjectId} not found in database or static data", id);
            return null;
        }

        public List<ProjectViewModel> GetAllProjects()
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve all projects from database");

                var projects = _databaseService.GetAllProjects();

                if (projects.Any())
                {
                    _logger.LogDebug("Successfully retrieved {Count} projects from database", projects.Count);
                    return projects;
                }

                _logger.LogInformation("No projects found in database, falling back to static data");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving all projects, falling back to static data");
            }

            // Fallback to static service
            try
            {
                var staticProjects = _staticService.GetAllProjects();
                _logger.LogInformation("Successfully retrieved {Count} projects from static fallback", staticProjects.Count);
                return staticProjects.Select(EnhanceProjectWithGallery).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects from static fallback");
                return new List<ProjectViewModel>();
            }
        }

        public List<ProjectViewModel> GetProjectsByType(ProjectType type)
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve projects of type {ProjectType} from database", type);

                var projects = _databaseService.GetProjectsByType(type);

                if (projects.Any())
                {
                    _logger.LogDebug("Successfully retrieved {Count} projects of type {ProjectType} from database", projects.Count, type);
                    return projects;
                }

                _logger.LogInformation("No projects of type {ProjectType} found in database, falling back to static data", type);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving projects of type {ProjectType}, falling back to static data", type);
            }

            // Fallback to static service
            try
            {
                var staticProjects = _staticService.GetProjectsByType(type);
                _logger.LogInformation("Successfully retrieved {Count} projects of type {ProjectType} from static fallback", staticProjects.Count, type);
                return staticProjects.Select(EnhanceProjectWithGallery).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects of type {ProjectType} from static fallback", type);
                return new List<ProjectViewModel>();
            }
        }

        public List<ProjectViewModel> GetAvailableProjects()
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve available projects from database");

                var projects = _databaseService.GetAvailableProjects();

                if (projects.Any())
                {
                    _logger.LogDebug("Successfully retrieved {Count} available projects from database", projects.Count);
                    return projects;
                }

                _logger.LogInformation("No available projects found in database, falling back to static data");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving available projects, falling back to static data");
            }

            // Fallback to static service
            try
            {
                var staticProjects = _staticService.GetAvailableProjects();
                _logger.LogInformation("Successfully retrieved {Count} available projects from static fallback", staticProjects.Count);
                return staticProjects.Select(EnhanceProjectWithGallery).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available projects from static fallback");
                return new List<ProjectViewModel>();
            }
        }

        public async Task<bool> UpdateProjectStatusAsync(string projectId, ProjectStatus newStatus, string changedBy = "system", string reason = "Status update")
        {
            try
            {
                _logger.LogDebug("Attempting to update project status for {ProjectId} to {NewStatus} in database", projectId, newStatus);

                var result = await _databaseService.UpdateProjectStatusAsync(projectId, newStatus, changedBy, reason);

                if (result)
                {
                    _logger.LogInformation("Successfully updated project status for {ProjectId} to {NewStatus} in database", projectId, newStatus);
                    return true;
                }

                _logger.LogInformation("Project {ProjectId} not found in database, falling back to static service", projectId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error updating project status for {ProjectId}, falling back to static service", projectId);
            }

            // Fallback to static service
            try
            {
                var staticResult = await _staticService.UpdateProjectStatusAsync(projectId, newStatus, changedBy, reason);
                if (staticResult)
                {
                    _logger.LogInformation("Successfully updated project status for {ProjectId} to {NewStatus} in static fallback", projectId, newStatus);
                }
                return staticResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project status for {ProjectId} in static fallback", projectId);
                return false;
            }
        }

        public async Task<List<dynamic>> GetProjectStatusSummaryAsync()
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve project status summary from database");

                var summary = await _databaseService.GetProjectStatusSummaryAsync();

                if (summary.Any())
                {
                    _logger.LogDebug("Successfully retrieved project status summary from database with {Count} entries", summary.Count);
                    return summary;
                }

                _logger.LogInformation("No project status summary found in database, falling back to static data");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving project status summary, falling back to static data");
            }

            // Fallback to static service
            try
            {
                var staticSummary = await _staticService.GetProjectStatusSummaryAsync();
                _logger.LogInformation("Successfully retrieved project status summary from static fallback with {Count} entries", staticSummary.Count);
                return staticSummary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project status summary from static fallback");
                return new List<dynamic>();
            }
        }

        public async Task<string> GetProjectStatusHistoryAsync(string projectId)
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve project status history for {ProjectId} from database", projectId);

                var history = await _databaseService.GetProjectStatusHistoryAsync(projectId);

                if (!string.IsNullOrEmpty(history) && history != "[]")
                {
                    _logger.LogDebug("Successfully retrieved project status history for {ProjectId} from database", projectId);
                    return history;
                }

                _logger.LogInformation("No project status history found for {ProjectId} in database, falling back to static data", projectId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database error retrieving project status history for {ProjectId}, falling back to static data", projectId);
            }

            // Fallback to static service
            try
            {
                var staticHistory = await _staticService.GetProjectStatusHistoryAsync(projectId);
                _logger.LogInformation("Successfully retrieved project status history for {ProjectId} from static fallback", projectId);
                return staticHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project status history for {ProjectId} from static fallback", projectId);
                return "[]";
            }
        }

        private ProjectViewModel EnhanceProjectWithGallery(ProjectViewModel project)
        {
            if (project == null) return project;

            var galleryImages = GetGalleryImages(project.Id);
            project.Images.Gallery = galleryImages;
            return project;
        }

        private List<string> GetGalleryImages(string projectId)
        {
            var galleryImages = new List<string>();
            var projectDir = Path.Combine("wwwroot", "images", "projects", projectId);

            if (Directory.Exists(projectDir))
            {
                var imageExtensions = new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.webp" };
                foreach (var extension in imageExtensions)
                {
                    var files = Directory.GetFiles(projectDir, extension, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var relativePath = file.Replace("wwwroot", "").Replace(Path.DirectorySeparatorChar, '/');
                        galleryImages.Add(relativePath);
                    }
                }
            }

            return galleryImages.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Health check method to verify database connectivity
        /// </summary>
        public bool IsDatabaseHealthy()
        {
            try
            {
                // Simple test - try to get any project from database
                var projects = _databaseService.GetAllProjects();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return false;
            }
        }


        /// <summary>
        /// Get service status information for monitoring
        /// </summary>
        public ServiceStatus GetServiceStatus()
        {
            var isDatabaseHealthy = IsDatabaseHealthy();
            var staticProjectCount = _staticService.GetAllProjects().Count;

            int databaseProjectCount = 0;
            if (isDatabaseHealthy)
            {
                try
                {
                    databaseProjectCount = _databaseService.GetAllProjects().Count;
                }
                catch
                {
                    // Already logged in IsDatabaseHealthy
                }
            }

            return new ServiceStatus
            {
                IsDatabaseAvailable = isDatabaseHealthy,
                DatabaseProjectCount = databaseProjectCount,
                StaticProjectCount = staticProjectCount,
                CurrentSource = isDatabaseHealthy && databaseProjectCount > 0 ? "Database" : "Static",
                LastChecked = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Service status information for monitoring and diagnostics
    /// </summary>
    public class ServiceStatus
    {
        public bool IsDatabaseAvailable { get; set; }
        public int DatabaseProjectCount { get; set; }
        public int StaticProjectCount { get; set; }
        public string CurrentSource { get; set; } = string.Empty;
        public DateTime LastChecked { get; set; }
    }
}