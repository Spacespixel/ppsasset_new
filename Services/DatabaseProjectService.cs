using PPSAsset.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace PPSAsset.Services
{
    public class DatabaseProjectService : IProjectService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseProjectService> _logger;

        public DatabaseProjectService(IConfiguration configuration, ILogger<DatabaseProjectService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _logger = logger;
        }

        public ProjectViewModel? GetProject(string id)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                // Main project query
                const string projectSql = @"
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

                var project = connection.QueryFirstOrDefault<dynamic>(projectSql, new { ProjectId = id });

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found in database", id);
                    return null;
                }

                var projectViewModel = MapToProjectViewModel(project);

                // Load related data
                projectViewModel.Images = LoadProjectImages(connection, id);
                projectViewModel.HouseTypes = LoadHouseTypes(connection, id);
                projectViewModel.Facilities = LoadFacilities(connection, id);
                projectViewModel.ConceptFeatures = LoadConceptFeatures(connection, id);
                projectViewModel.Location = LoadLocationInfo(connection, id);
                projectViewModel.Contact = LoadContactInfo(connection, id);

                return projectViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project with ID {ProjectId} from database", id);
                return null;
            }
        }

        public List<ProjectViewModel> GetAllProjects()
        {
            _logger.LogInformation("DatabaseProjectService: Attempting connection to MySQL server with connection string: {ConnectionString}", 
                _connectionString);
            
            using var connection = new MySqlConnection(_connectionString);
            
            _logger.LogInformation("DatabaseProjectService: Opening MySQL connection...");
            connection.Open();
            _logger.LogInformation("DatabaseProjectService: MySQL connection opened successfully");

                const string sql = @"
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
                    ORDER BY p.SortOrder ASC, p.ModifiedDate DESC";

                var projects = connection.Query<dynamic>(sql).ToList();
                var projectViewModels = new List<ProjectViewModel>();

                foreach (var project in projects)
                {
                    var projectViewModel = MapToProjectViewModel(project);

                    // Load basic images only for list view performance
                    projectViewModel.Images = LoadBasicProjectImages(connection, project.Id);

                    projectViewModels.Add(projectViewModel);
                }

                return projectViewModels;
        }

        public List<ProjectViewModel> GetProjectsByType(ProjectType type)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
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
                    WHERE p.ProjectType = @ProjectType
                    ORDER BY p.SortOrder ASC, p.ModifiedDate DESC";

                var projects = connection.Query<dynamic>(sql, new { ProjectType = type.ToString() }).ToList();
                var projectViewModels = new List<ProjectViewModel>();

                foreach (var project in projects)
                {
                    var projectViewModel = MapToProjectViewModel(project);
                    projectViewModel.Images = LoadBasicProjectImages(connection, project.Id);
                    projectViewModels.Add(projectViewModel);
                }

                return projectViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects by type {ProjectType} from database", type);
                return new List<ProjectViewModel>();
            }
        }

        public List<ProjectViewModel> GetAvailableProjects()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
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
                    WHERE p.ProjectStatus IN ('NewProject', 'Available')
                    ORDER BY p.SortOrder ASC, p.ModifiedDate DESC";

                var projects = connection.Query<dynamic>(sql).ToList();
                var projectViewModels = new List<ProjectViewModel>();

                foreach (var project in projects)
                {
                    var projectViewModel = MapToProjectViewModel(project);
                    projectViewModel.Images = LoadBasicProjectImages(connection, project.Id);
                    projectViewModels.Add(projectViewModel);
                }

                return projectViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available projects from database");
                return new List<ProjectViewModel>();
            }
        }

        public async Task<bool> UpdateProjectStatusAsync(string projectId, ProjectStatus newStatus, string changedBy = "system", string reason = "Status update")
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
                    CALL UpdateProjectStatus(@ProjectID, @NewStatus, @ChangedBy, @ChangeReason)";

                var result = await connection.QueryAsync<dynamic>(sql, new
                {
                    ProjectID = projectId,
                    NewStatus = newStatus.ToString(),
                    ChangedBy = changedBy,
                    ChangeReason = reason
                });

                _logger.LogInformation("Project {ProjectId} status updated to {NewStatus} by {ChangedBy}",
                    projectId, newStatus, changedBy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project status for {ProjectId} to {NewStatus}", projectId, newStatus);
                return false;
            }
        }

        public async Task<List<dynamic>> GetProjectStatusSummaryAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
                    SELECT
                        ProjectID,
                        ProjectName,
                        ProjectNameEN,
                        ProjectType,
                        ProjectStatus,
                        TotalInquiries,
                        LastStatusChange
                    FROM v_project_status_summary";

                var summary = await connection.QueryAsync<dynamic>(sql);
                return summary.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project status summary");
                return new List<dynamic>();
            }
        }

        public async Task<string> GetProjectStatusHistoryAsync(string projectId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"SELECT GetProjectStatusHistory(@ProjectID) as History";

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { ProjectID = projectId });
                return result?.History?.ToString() ?? "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status history for project {ProjectId}", projectId);
                return "[]";
            }
        }

        private ProjectViewModel MapToProjectViewModel(dynamic project)
        {
            return new ProjectViewModel
            {
                Id = project.Id ?? string.Empty,
                Name = project.NameTh ?? string.Empty,
                NameTh = project.NameTh ?? string.Empty,
                NameEn = project.NameEn ?? string.Empty,
                Subtitle = project.Subtitle ?? string.Empty,
                Description = project.Description ?? string.Empty,
                Concept = project.Concept ?? string.Empty,
                Type = Enum.TryParse<ProjectType>(project.Type?.ToString(), out ProjectType type) ? type : ProjectType.SingleHouse,
                Status = Enum.TryParse<ProjectStatus>(project.Status?.ToString(), out ProjectStatus status) ? status : ProjectStatus.Available,
                SortOrder = project.SortOrder ?? 0,
                Details = new ProjectDetails
                {
                    Location = project.Location ?? string.Empty,
                    ProjectSize = project.ProjectSize ?? string.Empty,
                    TotalUnits = project.TotalUnits ?? 0,
                    LandSize = project.LandSize ?? string.Empty,
                    UsableArea = project.UsableArea ?? string.Empty,
                    Developer = project.Developer ?? string.Empty,
                    PriceRange = project.PriceRange ?? string.Empty,
                    PropertyType = MapProjectTypeToPropertyType(project.Type?.ToString())
                }
            };
        }

        private string MapProjectTypeToPropertyType(string? projectType)
        {
            return projectType switch
            {
                nameof(ProjectType.SingleHouse) => "บ้านเดี่ยว 2 ชั้น",
                nameof(ProjectType.Townhouse) => "ทาวน์โฮม 3 ชั้น",
                nameof(ProjectType.TwinHouse) => "บ้านแฝด 2 ชั้น",
                nameof(ProjectType.Condominium) => "คอนโดมิเนียม",
                _ => "บ้านเดี่ยว 2 ชั้น"
            };
        }

        private ProjectImages LoadProjectImages(IDbConnection connection, string projectId)
        {
            const string sql = @"
                SELECT ImageType, ImagePath, SortOrder
                FROM sy_project_images
                WHERE ProjectID = @ProjectId
                ORDER BY ImageType, SortOrder";

            var images = connection.Query<dynamic>(sql, new { ProjectId = projectId }).ToList();

            var projectImages = new ProjectImages();
            var galleryImages = new List<string>();

            foreach (var image in images)
            {
                switch (image.ImageType?.ToString())
                {
                    case "Hero":
                        projectImages.Hero = image.ImagePath ?? string.Empty;
                        break;
                    case "Logo":
                        projectImages.Logo = image.ImagePath ?? string.Empty;
                        break;
                    case "Thumbnail":
                        projectImages.Thumbnail = image.ImagePath ?? string.Empty;
                        break;
                    case "Gallery":
                        galleryImages.Add(image.ImagePath ?? string.Empty);
                        break;
                    case "Facility":
                        if (string.IsNullOrEmpty(projectImages.FacilityMain))
                            projectImages.FacilityMain = image.ImagePath ?? string.Empty;
                        break;
                    case "LocationMap":
                        projectImages.LocationMap = image.ImagePath ?? string.Empty;
                        break;
                }
            }

            projectImages.Gallery = galleryImages;
            return projectImages;
        }

        private ProjectImages LoadBasicProjectImages(IDbConnection connection, string projectId)
        {
            try
            {
                // Try to load from database first
                const string sql = @"
                    SELECT ImageType, ImagePath, SortOrder
                    FROM sy_project_images
                    WHERE ProjectID = @ProjectId AND ImageType IN ('Thumbnail', 'Hero', 'Logo')
                    ORDER BY SortOrder ASC";

                var images = connection.Query<dynamic>(sql, new { ProjectId = projectId }).ToList();
                var projectImages = new ProjectImages();

                foreach (var image in images)
                {
                    switch (image.ImageType?.ToString())
                    {
                        case "Thumbnail":
                            // Store thumbnail separately and also use for Hero fallback
                            projectImages.Thumbnail = image.ImagePath ?? string.Empty;
                            projectImages.Hero = image.ImagePath ?? string.Empty;
                            break;
                        case "Hero":
                            // Only use Hero if no Thumbnail is found
                            if (string.IsNullOrEmpty(projectImages.Hero))
                                projectImages.Hero = image.ImagePath ?? string.Empty;
                            break;
                        case "Logo":
                            projectImages.Logo = image.ImagePath ?? string.Empty;
                            break;
                    }
                }

                // Fallback: Generate dynamic path if no images found in database
                if (string.IsNullOrEmpty(projectImages.Hero) && !string.IsNullOrEmpty(projectId))
                {
                    var imageFolderPath = $"~/images/projects/{projectId}";
                    var projectName = projectId.Replace("-", " ")
                        .Split(' ')
                        .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                        .Aggregate((a, b) => a + "-" + b);
                    projectImages.Hero = $"{imageFolderPath}/{projectName}-Facility-1.png";
                }

                return projectImages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading images for project {ProjectId}", projectId);

                // Fallback: Generate dynamic path
                var projectImages = new ProjectImages();
                if (!string.IsNullOrEmpty(projectId))
                {
                    var imageFolderPath = $"~/images/projects/{projectId}";
                    var projectName = projectId.Replace("-", " ")
                        .Split(' ')
                        .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                        .Aggregate((a, b) => a + "-" + b);
                    projectImages.Hero = $"{imageFolderPath}/{projectName}-Facility-1.png";
                }
                return projectImages;
            }
        }

        private List<HouseType> LoadHouseTypes(IDbConnection connection, string projectId)
        {
            const string houseTypeSql = @"
                SELECT
                    HouseTypeID,
                    HouseTypeCode as Id,
                    HouseTypeName as Name,
                    DisplayName,
                    Description,
                    Bedrooms,
                    Bathrooms,
                    Parking,
                    LandSize,
                    UsableArea
                FROM sy_project_house_types
                WHERE ProjectID = @ProjectId
                ORDER BY HouseTypeID";

            var houseTypes = connection.Query<HouseType>(houseTypeSql, new { ProjectId = projectId }).ToList();

            // Load floor plans for each house type
            foreach (var houseType in houseTypes)
            {
                houseType.FloorPlans = LoadFloorPlans(connection, houseType.HouseTypeID);
            }

            return houseTypes;
        }

        private List<FloorPlan> LoadFloorPlans(IDbConnection connection, int houseTypeId)
        {
            const string sql = @"
                SELECT
                    fp.FloorPlanCode as Id,
                    fp.FloorPlanName as Name,
                    fp.ImagePath,
                    fp.FloorType as Type
                FROM sy_project_floor_plans fp
                WHERE fp.HouseTypeID = @HouseTypeID
                ORDER BY fp.SortOrder";

            var floorPlans = connection.Query<dynamic>(sql, new { HouseTypeID = houseTypeId }).ToList();

            return floorPlans.Select(fp => new FloorPlan
            {
                Id = fp.Id ?? string.Empty,
                Name = fp.Name ?? string.Empty,
                ImagePath = fp.ImagePath ?? string.Empty,
                Type = Enum.TryParse<FloorType>(fp.Type?.ToString(), out FloorType type) ? type : FloorType.Facade
            }).ToList();
        }

        private List<Facility> LoadFacilities(IDbConnection connection, string projectId)
        {
            const string sql = @"
                SELECT
                    FacilityCode as Id,
                    FacilityName as Name,
                    Description,
                    Icon,
                    Category
                FROM sy_project_facilities
                WHERE ProjectID = @ProjectId
                ORDER BY Category, FacilityName";

            var facilities = connection.Query<dynamic>(sql, new { ProjectId = projectId }).ToList();

            return facilities.Select(f => new Facility
            {
                Id = f.Id ?? string.Empty,
                Name = f.Name ?? string.Empty,
                Description = f.Description ?? string.Empty,
                Icon = f.Icon ?? string.Empty,
                Category = Enum.TryParse<FacilityCategory>(f.Category?.ToString(), out FacilityCategory category) ? category : FacilityCategory.Recreation
            }).ToList();
        }

        private List<ConceptFeature> LoadConceptFeatures(IDbConnection connection, string projectId)
        {
            try
            {
                // Get the project concept text
                const string conceptSql = @"
                    SELECT ProjectConcept
                    FROM sy_project
                    WHERE ProjectID = @ProjectId";

                var projectConcept = connection.QueryFirstOrDefault<string>(conceptSql, new { ProjectId = projectId });

                if (string.IsNullOrWhiteSpace(projectConcept))
                {
                    _logger.LogDebug("No concept found for project {ProjectId}", projectId);
                    return new List<ConceptFeature>();
                }

                // Get gallery images for this project (take first 2)
                const string imagesSql = @"
                    SELECT ImagePath
                    FROM sy_project_images
                    WHERE ProjectID = @ProjectId
                    AND ImageType = 'Gallery'
                    ORDER BY SortOrder
                    LIMIT 2";

                var galleryImages = connection.Query<string>(imagesSql, new { ProjectId = projectId }).ToList();

                var features = new List<ConceptFeature>();
                string cleanConcept = projectConcept.Trim();

                // Check if concept contains HTML tags (h3, p)
                if (cleanConcept.Contains("<h3>") || cleanConcept.Contains("<p>"))
                {
                    // Parse HTML-formatted concept data
                    string title = string.Empty;
                    string summary = string.Empty;
                    string details = string.Empty;

                    // Extract h3 title
                    var h3Match = Regex.Match(cleanConcept, @"<h3>(.*?)</h3>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (h3Match.Success)
                    {
                        title = h3Match.Groups[1].Value;
                        cleanConcept = Regex.Replace(cleanConcept, @"<h3>.*?</h3>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    }

                    // Extract all paragraphs
                    var paragraphs = Regex.Matches(cleanConcept, @"<p[^>]*>(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (paragraphs.Count >= 2)
                    {
                        summary = paragraphs[0].Groups[1].Value.Trim();
                        details = paragraphs[1].Groups[1].Value.Trim();
                    }
                    else if (paragraphs.Count == 1)
                    {
                        summary = paragraphs[0].Groups[1].Value.Trim();
                    }
                    else
                    {
                        summary = cleanConcept;
                    }

                    // Add first feature with title and summary
                    if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(summary))
                    {
                        features.Add(new ConceptFeature
                        {
                            Title = title,
                            Description = summary,
                            Image = galleryImages.Count > 0 ? galleryImages[0] : string.Empty,
                            Icon = string.Empty
                        });
                    }

                    // Add second feature with details if available
                    if (!string.IsNullOrEmpty(details))
                    {
                        features.Add(new ConceptFeature
                        {
                            Title = details,
                            Description = string.Empty,
                            Image = galleryImages.Count > 1 ? galleryImages[1] : (galleryImages.Count > 0 ? galleryImages[0] : string.Empty),
                            Icon = string.Empty
                        });
                    }
                }
                else
                {
                    // Original logic for non-HTML formatted text
                    // Find logical split points - look for sentence endings followed by space and capital letter or project name
                    int conceptLength = cleanConcept.Length;
                    int midpoint = conceptLength / 2;

                    // Find nearest sentence break around the midpoint
                    int splitPoint = midpoint;
                    for (int i = midpoint; i < conceptLength - 1; i++)
                    {
                        if (cleanConcept[i] == '.' && i < conceptLength - 1 && cleanConcept[i + 1] == ' ')
                        {
                            splitPoint = i + 2;
                            break;
                        }
                    }

                    string section1Text = cleanConcept.Substring(0, splitPoint).Trim();
                    string section2Text = splitPoint < conceptLength ? cleanConcept.Substring(splitPoint).Trim() : "";

                    // Extract title from section 1 (full first sentence without truncation)
                    int firstSentenceEnd = section1Text.IndexOf('.');
                    string section1Title = firstSentenceEnd > 0
                        ? section1Text.Substring(0, firstSentenceEnd).Trim()
                        : section1Text.Trim();

                    string section1Description = firstSentenceEnd > 0 && firstSentenceEnd < section1Text.Length - 1
                        ? section1Text.Substring(firstSentenceEnd + 1).Trim()
                        : "";

                    // Extract title and description from section 2
                    string section2Title = "";
                    string section2Description = "";

                    if (!string.IsNullOrEmpty(section2Text))
                    {
                        int section2SentenceEnd = section2Text.IndexOf('.');
                        section2Title = section2SentenceEnd > 0
                            ? section2Text.Substring(0, section2SentenceEnd).Trim()
                            : section2Text.Trim();

                        section2Description = section2SentenceEnd > 0 && section2SentenceEnd < section2Text.Length - 1
                            ? section2Text.Substring(section2SentenceEnd + 1).Trim()
                            : "";
                    }

                    // Add first feature section with first gallery image
                    features.Add(new ConceptFeature
                    {
                        Title = section1Title,
                        Description = section1Description,
                        Image = galleryImages.Count > 0 ? galleryImages[0] : string.Empty,
                        Icon = string.Empty
                    });

                    // Add second feature section with second gallery image (if we have content)
                    if (!string.IsNullOrEmpty(section2Title))
                    {
                        features.Add(new ConceptFeature
                        {
                            Title = section2Title,
                            Description = section2Description,
                            Image = galleryImages.Count > 1 ? galleryImages[1] : (galleryImages.Count > 0 ? galleryImages[0] : string.Empty),
                            Icon = string.Empty
                        });
                    }
                }

                if (features.Any())
                {
                    _logger.LogDebug("Loaded {FeatureCount} concept features for project {ProjectId}", features.Count, projectId);
                }

                return features;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading concept features for project {ProjectId}", projectId);
                return new List<ConceptFeature>();
            }
        }

        private LocationInfo LoadLocationInfo(IDbConnection connection, string projectId)
        {
            const string locationSql = @"
                SELECT
                    l.District,
                    l.Province,
                    l.Latitude,
                    l.Longitude
                FROM sy_project_locations l
                WHERE l.ProjectID = @ProjectId";

            const string nearbyPlacesSql = @"
                SELECT
                    np.PlaceName as Name,
                    np.Category,
                    np.Distance,
                    np.TravelTime
                FROM sy_project_nearby_places np
                INNER JOIN sy_project_locations l ON np.LocationID = l.LocationID
                WHERE l.ProjectID = @ProjectId
                ORDER BY np.SortOrder";

            var location = connection.QueryFirstOrDefault<LocationInfo>(locationSql, new { ProjectId = projectId });
            if (location == null)
            {
                return new LocationInfo();
            }

            var nearbyPlaces = connection.Query<NearbyPlace>(nearbyPlacesSql, new { ProjectId = projectId }).ToList();
            location.NearbyPlaces = nearbyPlaces;

            return location;
        }

        private ContactInfo LoadContactInfo(IDbConnection connection, string projectId)
        {
            const string sql = @"
                SELECT
                    Phone,
                    LineId,
                    Facebook,
                    OfficeHoursWeekdays as Weekdays,
                    OfficeHoursWeekends as Weekends,
                    OfficeHoursHolidays as Holidays
                FROM sy_project_contacts
                WHERE ProjectID = @ProjectId";

            var contact = connection.QueryFirstOrDefault<dynamic>(sql, new { ProjectId = projectId });

            if (contact == null)
            {
                return new ContactInfo
                {
                    Email = "sales@ppsasset.com"
                };
            }

            return new ContactInfo
            {
                Phone = contact.Phone ?? string.Empty,
                Email = "sales@ppsasset.com",
                LineId = contact.LineId ?? string.Empty,
                Facebook = contact.Facebook ?? string.Empty,
                Hours = new OfficeHours
                {
                    Weekdays = contact.Weekdays ?? string.Empty,
                    Weekends = contact.Weekends ?? string.Empty,
                    Holidays = contact.Holidays ?? string.Empty
                }
            };
        }
    }
}
