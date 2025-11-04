using PPSAsset.Models;
using PPSAsset.Services;
using Dapper;
using MySql.Data.MySqlClient;

namespace PPSAsset.Data
{
    /// <summary>
    /// Database migration class to populate MySQL database with static project data
    /// This enables seamless transition from static to database-driven content
    /// </summary>
    public class DatabaseMigration
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseMigration> _logger;

        public DatabaseMigration(IConfiguration configuration, ILogger<DatabaseMigration> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _logger = logger;
        }

        /// <summary>
        /// Migrates all static project data to the database
        /// This method is idempotent - can be run multiple times safely
        /// </summary>
        public async Task<MigrationResult> MigrateStaticDataAsync()
        {
            var result = new MigrationResult();

            try
            {
                _logger.LogInformation("Starting database migration of static project data");

                // Get static projects
                var staticService = new StaticProjectService();
                var projects = staticService.GetAllProjects();

                _logger.LogInformation("Found {Count} projects to migrate", projects.Count);

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                // Start transaction for data integrity
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    foreach (var project in projects)
                    {
                        var projectResult = await MigrateProjectAsync(connection, transaction, project);
                        result.ProjectResults.Add(project.Id, projectResult);

                        if (projectResult.Success)
                        {
                            result.SuccessCount++;
                            _logger.LogDebug("Successfully migrated project: {ProjectId}", project.Id);
                        }
                        else
                        {
                            result.ErrorCount++;
                            _logger.LogWarning("Failed to migrate project: {ProjectId} - {Error}", project.Id, projectResult.ErrorMessage);
                        }
                    }

                    await transaction.CommitAsync();
                    result.Success = true;
                    result.Message = $"Migration completed: {result.SuccessCount} succeeded, {result.ErrorCount} failed";

                    _logger.LogInformation("Database migration completed successfully: {SuccessCount} projects migrated, {ErrorCount} errors",
                        result.SuccessCount, result.ErrorCount);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Migration failed: {ex.Message}";
                _logger.LogError(ex, "Database migration failed");
            }

            return result;
        }

        private async Task<ProjectMigrationResult> MigrateProjectAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            var result = new ProjectMigrationResult();

            try
            {
                // 1. Insert/Update main project
                await UpsertMainProjectAsync(connection, transaction, project);

                // 2. Migrate images
                await MigrateProjectImagesAsync(connection, transaction, project);

                // 3. Migrate house types
                await MigrateHouseTypesAsync(connection, transaction, project);

                // 4. Migrate facilities
                await MigrateFacilitiesAsync(connection, transaction, project);

                // 5. Migrate concept features
                await MigrateConceptFeaturesAsync(connection, transaction, project);

                // 6. Migrate location info
                await MigrateLocationInfoAsync(connection, transaction, project);

                // 7. Migrate contact info
                await MigrateContactInfoAsync(connection, transaction, project);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error migrating project {ProjectId}", project.Id);
            }

            return result;
        }

        private async Task UpsertMainProjectAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            const string sql = @"
                INSERT INTO sy_project (
                    ProjectID, ProjectName, ProjectNameEN, ProjectAddress, ProjectType, ProjectEmail,
                    ProjectSubtitle, ProjectDescription, ProjectConcept, ProjectStatus, ProjectSize,
                    TotalUnits, LandSize, UsableArea, Developer, PriceRange, GtmId
                ) VALUES (
                    @ProjectID, @ProjectName, @ProjectNameEN, @ProjectAddress, @ProjectType, @ProjectEmail,
                    @ProjectSubtitle, @ProjectDescription, @ProjectConcept, @ProjectStatus, @ProjectSize,
                    @TotalUnits, @LandSize, @UsableArea, @Developer, @PriceRange, @GtmId
                ) ON DUPLICATE KEY UPDATE
                    ProjectName = VALUES(ProjectName),
                    ProjectNameEN = VALUES(ProjectNameEN),
                    ProjectAddress = VALUES(ProjectAddress),
                    ProjectType = VALUES(ProjectType),
                    ProjectEmail = VALUES(ProjectEmail),
                    ProjectSubtitle = VALUES(ProjectSubtitle),
                    ProjectDescription = VALUES(ProjectDescription),
                    ProjectConcept = VALUES(ProjectConcept),
                    ProjectStatus = VALUES(ProjectStatus),
                    ProjectSize = VALUES(ProjectSize),
                    TotalUnits = VALUES(TotalUnits),
                    LandSize = VALUES(LandSize),
                    UsableArea = VALUES(UsableArea),
                    Developer = VALUES(Developer),
                    PriceRange = VALUES(PriceRange),
                    GtmId = VALUES(GtmId),
                    ModifiedDate = CURRENT_TIMESTAMP";

            var parameters = new
            {
                ProjectID = project.Id,
                ProjectName = project.Name, // Use Name as Thai name
                ProjectNameEN = project.NameEn,
                ProjectAddress = project.Details.Location, // Use location as address
                ProjectType = project.Type.ToString(),
                ProjectEmail = "", // Default empty email
                ProjectSubtitle = project.Subtitle,
                ProjectDescription = project.Description,
                ProjectConcept = project.Concept,
                ProjectStatus = project.Status.ToString(),
                ProjectSize = project.Details.ProjectSize,
                TotalUnits = project.Details.TotalUnits,
                LandSize = project.Details.LandSize,
                UsableArea = project.Details.UsableArea,
                Developer = project.Details.Developer,
                PriceRange = project.Details.PriceRange,
                GtmId = project.GtmId
            };

            await connection.ExecuteAsync(sql, parameters, transaction);
        }

        private async Task MigrateProjectImagesAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            // Delete existing images for this project
            await connection.ExecuteAsync("DELETE FROM sy_project_images WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            var imagesToInsert = new List<object>();

            // Hero image
            if (!string.IsNullOrEmpty(project.Images.Hero))
            {
                imagesToInsert.Add(new
                {
                    ProjectID = project.Id,
                    ImageType = "Hero",
                    ImagePath = project.Images.Hero,
                    SortOrder = 0
                });
            }

            // Logo image
            if (!string.IsNullOrEmpty(project.Images.Logo))
            {
                imagesToInsert.Add(new
                {
                    ProjectID = project.Id,
                    ImageType = "Logo",
                    ImagePath = project.Images.Logo,
                    SortOrder = 0
                });
            }

            // Facility main image
            if (!string.IsNullOrEmpty(project.Images.FacilityMain))
            {
                imagesToInsert.Add(new
                {
                    ProjectID = project.Id,
                    ImageType = "Facility",
                    ImagePath = project.Images.FacilityMain,
                    SortOrder = 0
                });
            }

            // Gallery images
            for (int i = 0; i < project.Images.Gallery.Count; i++)
            {
                imagesToInsert.Add(new
                {
                    ProjectID = project.Id,
                    ImageType = "Gallery",
                    ImagePath = project.Images.Gallery[i],
                    SortOrder = i
                });
            }

            if (imagesToInsert.Any())
            {
                const string sql = @"
                    INSERT INTO sy_project_images (ProjectID, ImageType, ImagePath, SortOrder)
                    VALUES (@ProjectID, @ImageType, @ImagePath, @SortOrder)";

                await connection.ExecuteAsync(sql, imagesToInsert, transaction);
            }
        }

        private async Task MigrateHouseTypesAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            // Delete existing house types for this project (cascade will handle floor plans)
            await connection.ExecuteAsync("DELETE FROM sy_project_house_types WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            foreach (var houseType in project.HouseTypes)
            {
                const string houseTypeSql = @"
                    INSERT INTO sy_project_house_types (
                        ProjectID, HouseTypeCode, HouseTypeName, DisplayName, Description,
                        Bedrooms, Bathrooms, Parking, LandSize, UsableArea
                    ) VALUES (
                        @ProjectID, @HouseTypeCode, @HouseTypeName, @DisplayName, @Description,
                        @Bedrooms, @Bathrooms, @Parking, @LandSize, @UsableArea
                    )";

                var houseTypeId = await connection.QuerySingleAsync<int>(
                    houseTypeSql + "; SELECT LAST_INSERT_ID();",
                    new
                    {
                        ProjectID = project.Id,
                        HouseTypeCode = houseType.Id,
                        HouseTypeName = houseType.Name,
                        DisplayName = houseType.DisplayName,
                        Description = houseType.Description,
                        Bedrooms = houseType.Bedrooms,
                        Bathrooms = houseType.Bathrooms,
                        Parking = houseType.Parking,
                        LandSize = houseType.LandSize,
                        UsableArea = houseType.UsableArea
                    }, transaction);

                // Migrate floor plans for this house type
                await MigrateFloorPlansAsync(connection, transaction, houseTypeId, houseType.FloorPlans);
            }
        }

        private async Task MigrateFloorPlansAsync(MySqlConnection connection, MySqlTransaction transaction, int houseTypeId, List<FloorPlan> floorPlans)
        {
            if (!floorPlans.Any()) return;

            const string sql = @"
                INSERT INTO sy_project_floor_plans (
                    HouseTypeID, FloorPlanCode, FloorPlanName, ImagePath, FloorType, SortOrder
                ) VALUES (
                    @HouseTypeID, @FloorPlanCode, @FloorPlanName, @ImagePath, @FloorType, @SortOrder
                )";

            var floorPlanInserts = floorPlans.Select((fp, index) => new
            {
                HouseTypeID = houseTypeId,
                FloorPlanCode = fp.Id,
                FloorPlanName = fp.Name,
                ImagePath = fp.ImagePath,
                FloorType = fp.Type.ToString(),
                SortOrder = index
            });

            await connection.ExecuteAsync(sql, floorPlanInserts, transaction);
        }

        private async Task MigrateFacilitiesAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            // Delete existing facilities for this project
            await connection.ExecuteAsync("DELETE FROM sy_project_facilities WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            if (!project.Facilities.Any()) return;

            const string sql = @"
                INSERT INTO sy_project_facilities (
                    ProjectID, FacilityCode, FacilityName, Description, Icon, Category
                ) VALUES (
                    @ProjectID, @FacilityCode, @FacilityName, @Description, @Icon, @Category
                )";

            var facilityInserts = project.Facilities.Select(f => new
            {
                ProjectID = project.Id,
                FacilityCode = f.Id,
                FacilityName = f.Name,
                Description = f.Description,
                Icon = f.Icon,
                Category = f.Category.ToString()
            });

            await connection.ExecuteAsync(sql, facilityInserts, transaction);
        }

        private async Task MigrateConceptFeaturesAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            // Delete existing features for this project
            await connection.ExecuteAsync("DELETE FROM sy_project_features WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            if (!project.ConceptFeatures.Any()) return;

            const string sql = @"
                INSERT INTO sy_project_features (
                    ProjectID, FeatureTitle, FeatureDescription, SortOrder
                ) VALUES (
                    @ProjectID, @FeatureTitle, @FeatureDescription, @SortOrder
                )";

            var featureInserts = project.ConceptFeatures.Select((f, index) => new
            {
                ProjectID = project.Id,
                FeatureTitle = f.Title,
                FeatureDescription = f.Description,
                SortOrder = index
            });

            await connection.ExecuteAsync(sql, featureInserts, transaction);
        }

        private async Task MigrateLocationInfoAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            if (project.Location == null) return;

            // Delete existing location for this project (cascade will handle nearby places)
            await connection.ExecuteAsync("DELETE FROM sy_project_locations WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            const string locationSql = @"
                INSERT INTO sy_project_locations (ProjectID, District, Province)
                VALUES (@ProjectID, @District, @Province)";

            var locationId = await connection.QuerySingleAsync<int>(
                locationSql + "; SELECT LAST_INSERT_ID();",
                new
                {
                    ProjectID = project.Id,
                    District = project.Location.District,
                    Province = project.Location.Province
                }, transaction);

            // Migrate nearby places
            if (project.Location.NearbyPlaces.Any())
            {
                const string nearbyPlacesSql = @"
                    INSERT INTO sy_project_nearby_places (
                        LocationID, PlaceName, Category, Distance, TravelTime, SortOrder
                    ) VALUES (
                        @LocationID, @PlaceName, @Category, @Distance, @TravelTime, @SortOrder
                    )";

                var nearbyPlaceInserts = project.Location.NearbyPlaces.Select((np, index) => new
                {
                    LocationID = locationId,
                    PlaceName = np.Name,
                    Category = np.Category,
                    Distance = np.Distance,
                    TravelTime = np.TravelTime,
                    SortOrder = index
                });

                await connection.ExecuteAsync(nearbyPlacesSql, nearbyPlaceInserts, transaction);
            }
        }

        private async Task MigrateContactInfoAsync(MySqlConnection connection, MySqlTransaction transaction, ProjectViewModel project)
        {
            if (project.Contact == null) return;

            // Delete existing contact for this project
            await connection.ExecuteAsync("DELETE FROM sy_project_contacts WHERE ProjectID = @ProjectID",
                new { ProjectID = project.Id }, transaction);

            const string sql = @"
                INSERT INTO sy_project_contacts (
                    ProjectID, Phone, LineId, Facebook,
                    OfficeHoursWeekdays, OfficeHoursWeekends, OfficeHoursHolidays
                ) VALUES (
                    @ProjectID, @Phone, @LineId, @Facebook,
                    @OfficeHoursWeekdays, @OfficeHoursWeekends, @OfficeHoursHolidays
                )";

            await connection.ExecuteAsync(sql, new
            {
                ProjectID = project.Id,
                Phone = project.Contact.Phone,
                LineId = project.Contact.LineId,
                Facebook = project.Contact.Facebook,
                OfficeHoursWeekdays = project.Contact.Hours?.Weekdays,
                OfficeHoursWeekends = project.Contact.Hours?.Weekends,
                OfficeHoursHolidays = project.Contact.Hours?.Holidays
            }, transaction);
        }

        /// <summary>
        /// Verifies the database schema exists and is compatible
        /// </summary>
        public async Task<bool> VerifySchemaAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if all required tables exist
                var requiredTables = new[]
                {
                    "sy_project", "sy_project_images", "sy_project_house_types",
                    "sy_project_facilities", "sy_project_features", "sy_project_locations",
                    "sy_project_nearby_places", "sy_project_contacts", "sy_project_floor_plans"
                };

                foreach (var table in requiredTables)
                {
                    var exists = await connection.QuerySingleAsync<bool>(
                        "SELECT COUNT(*) > 0 FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @TableName",
                        new { TableName = table });

                    if (!exists)
                    {
                        _logger.LogError("Required table {TableName} does not exist", table);
                        return false;
                    }
                }

                _logger.LogInformation("Database schema verification completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database schema verification failed");
                return false;
            }
        }
    }

    public class MigrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public Dictionary<string, ProjectMigrationResult> ProjectResults { get; set; } = new();
    }

    public class ProjectMigrationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}