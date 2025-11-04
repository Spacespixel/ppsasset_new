using Dapper;
using MySql.Data.MySqlClient;
using PPSAsset.Models;

namespace PPSAsset.Services
{
    /// <summary>
    /// Implementation of GTM service using database configuration
    /// </summary>
    public class GtmService : IGtmService
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<GtmService> _logger;

        public GtmService(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILogger<GtmService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Get GTM ID for a specific project
        /// </summary>
        public async Task<string?> GetGtmIdAsync(string projectId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
                    SELECT GtmId
                    FROM sy_project
                    WHERE ProjectID = @ProjectId
                    LIMIT 1";

                var gtmId = await connection.QueryFirstOrDefaultAsync<string?>(
                    sql, new { ProjectId = projectId });

                if (string.IsNullOrEmpty(gtmId))
                {
                    _logger.LogInformation("No GTM ID configured for project: {ProjectId}", projectId);
                }

                return gtmId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GTM ID for project: {ProjectId}", projectId);
                return null;
            }
        }

        /// <summary>
        /// Get global GTM ID (from first available project or global setting)
        /// </summary>
        public async Task<string?> GetGlobalGtmIdAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                // First try to get a global GTM setting (you can create a specific project for this)
                const string globalSql = @"
                    SELECT GtmId
                    FROM sy_project
                    WHERE ProjectID = 'global'
                    LIMIT 1";

                var globalResult = await connection.QueryFirstOrDefaultAsync<string>(
                    globalSql);

                if (!string.IsNullOrEmpty(globalResult))
                {
                    return globalResult;
                }

                // Fallback: get from any project that has GTM configured
                const string fallbackSql = @"
                    SELECT GtmId
                    FROM sy_project
                    WHERE GtmId IS NOT NULL AND GtmId != ''
                    LIMIT 1";

                var fallbackResult = await connection.QueryFirstOrDefaultAsync<string>(
                    fallbackSql);

                if (!string.IsNullOrEmpty(fallbackResult))
                {
                    return fallbackResult;
                }

                _logger.LogWarning("No global GTM configuration found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving global GTM ID");
                return null;
            }
        }

        /// <summary>
        /// Update GTM configuration for a project
        /// </summary>
        public async Task<bool> UpdateProjectGtmAsync(string projectId, string? gtmId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
                    UPDATE sy_project
                    SET GtmId = @GtmId,
                        ModifiedDate = CURRENT_TIMESTAMP
                    WHERE ProjectID = @ProjectId";

                var rowsAffected = await connection.ExecuteAsync(sql, new {
                    ProjectId = projectId,
                    GtmId = gtmId
                });

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated GTM configuration for project: {ProjectId}", projectId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No project found with ID: {ProjectId}", projectId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating GTM configuration for project: {ProjectId}", projectId);
                return false;
            }
        }
    }
}