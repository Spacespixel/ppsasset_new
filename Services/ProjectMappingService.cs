using Dapper;
using MySql.Data.MySqlClient;

namespace PPSAsset.Services
{
    /// <summary>
    /// Service to handle project ID mapping between internal format and legacy database format
    /// Maps project string IDs (e.g., "ricco-residence-hathairat") to MappedProjectID (e.g., "SG06", "TH01") from sy_project_mapping table
    /// This ensures compatibility with old transaction data that stores legacy project codes
    /// </summary>
    public interface IProjectMappingService
    {
        /// <summary>
        /// Get the MappedProjectID (legacy code like SG06, TH01) for a given project string ID
        /// </summary>
        Task<string?> GetMappedProjectIdAsync(string projectId);

        /// <summary>
        /// Get project string ID from MappedProjectID (legacy code)
        /// </summary>
        Task<string?> GetProjectIdFromMappedIdAsync(string mappedProjectId);
    }

    public class ProjectMappingService : IProjectMappingService
    {
        private readonly string _connectionString;
        private readonly ILogger<ProjectMappingService> _logger;

        public ProjectMappingService(IConfiguration configuration, ILogger<ProjectMappingService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _logger = logger;
        }

        /// <summary>
        /// Get the MappedProjectID (legacy code like SG06, TH01) for a given project string ID
        /// </summary>
        public async Task<string?> GetMappedProjectIdAsync(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                _logger.LogWarning("GetMappedProjectIdAsync called with null or empty projectId");
                return null;
            }

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                // Query sy_project_mapping to get MappedProjectID for the given ProjectID
                const string query = @"
                    SELECT MappedProjectID
                    FROM sy_project_mapping
                    WHERE ProjectID = @ProjectId
                    AND IsActive = 1
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    query,
                    new { ProjectId = projectId });

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogDebug("Found MappedProjectID {MappedProjectId} for ProjectID {ProjectId}", result, projectId);
                    return result;
                }

                _logger.LogWarning("No MappedProjectID found for ProjectID {ProjectId}", projectId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving MappedProjectID for ProjectID {ProjectId}", projectId);
                return null;
            }
        }

        /// <summary>
        /// Get project string ID from MappedProjectID (legacy code)
        /// </summary>
        public async Task<string?> GetProjectIdFromMappedIdAsync(string mappedProjectId)
        {
            if (string.IsNullOrWhiteSpace(mappedProjectId))
            {
                _logger.LogWarning("GetProjectIdFromMappedIdAsync called with null or empty mappedProjectId");
                return null;
            }

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT ProjectID
                    FROM sy_project_mapping
                    WHERE MappedProjectID = @MappedProjectId
                    AND IsActive = 1
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    query,
                    new { MappedProjectId = mappedProjectId });

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogDebug("Found ProjectID {ProjectId} for MappedProjectID {MappedProjectId}", result, mappedProjectId);
                    return result;
                }

                _logger.LogWarning("No ProjectID found for MappedProjectID {MappedProjectId}", mappedProjectId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ProjectID for MappedProjectID {MappedProjectId}", mappedProjectId);
                return null;
            }
        }
    }
}
