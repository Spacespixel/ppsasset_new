using Dapper;
using MySql.Data.MySqlClient;
using PPSAsset.Models;

namespace PPSAsset.Services
{
    public class RegistrationService
    {
        private readonly string _connectionString;
        private readonly ILogger<RegistrationService> _logger;
        private readonly IProjectMappingService _projectMappingService;

        public RegistrationService(IConfiguration configuration, ILogger<RegistrationService> logger, IProjectMappingService projectMappingService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _logger = logger;
            _projectMappingService = projectMappingService;
        }


        public async Task<List<string>> CheckDuplicatesAsync(RegistrationInputModel input)
        {
            var errors = new List<string>();
            
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // Check duplicate name only if both first and last name are provided
                if (!string.IsNullOrWhiteSpace(input.FirstName) && !string.IsNullOrWhiteSpace(input.LastName))
                {
                    var nameCheckSql = "SELECT COUNT(*) FROM tr_transaction WHERE FirstName = @FirstName AND LastName = @LastName";
                    var nameCount = await connection.QuerySingleAsync<int>(nameCheckSql, new { 
                        FirstName = input.FirstName.Trim(), 
                        LastName = input.LastName.Trim() 
                    });
                    
                    if (nameCount > 0)
                    {
                        errors.Add($"พบข้อมูลชื่อ {input.FirstName} {input.LastName} ในระบบแล้ว");
                    }
                }
                
                // Check duplicate phone number if provided
                if (!string.IsNullOrWhiteSpace(input.TelNo))
                {
                    var phoneCheckSql = "SELECT COUNT(*) FROM tr_transaction WHERE TelNo = @TelNo";
                    var phoneCount = await connection.QuerySingleAsync<int>(phoneCheckSql, new { 
                        TelNo = input.TelNo.Trim() 
                    });
                    
                    if (phoneCount > 0)
                    {
                        errors.Add($"พบหมายเลขโทรศัพท์ {input.TelNo} ในระบบแล้ว");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking duplicates for {FirstName} {LastName}", input.FirstName, input.LastName);
                // Return empty list on error - don't block registration for database issues
            }
            
            return errors;
        }

        public async Task SaveAsync(RegistrationInputModel input, HttpRequest request)
        {
            var transactionId = DateTime.UtcNow.ToString("yyMMddHHmmssfff");

            // Get the MappedProjectID (legacy code like SG06, TH01) from the string ProjectID for backward compatibility
            var mappedProjectId = await _projectMappingService.GetMappedProjectIdAsync(input.ProjectID);

            var parameters = new DynamicParameters();
            parameters.Add("@TransactoinID", transactionId);
            // Use MappedProjectID (legacy code) instead of string ProjectID for backward compatibility with old data
            parameters.Add("@ProjectID", mappedProjectId ?? string.Empty);
            parameters.Add("@ProjectName", input.ProjectName);
            parameters.Add("@FirstName", input.FirstName);
            parameters.Add("@LastName", input.LastName);
            parameters.Add("@Budget", input.Budget);
            parameters.Add("@Province", input.Province);
            parameters.Add("@Distric", input.District);
            parameters.Add("@TelNo", input.TelNo);
            parameters.Add("@EMail", input.Email);
            parameters.Add("@ClientFrom", input.ClientFrom);
            parameters.Add("@TransactionDate", DateTime.UtcNow);
            parameters.Add("@Remark", input.Remark);
            parameters.Add("@AppointmentDate", input.AppointmentDate);
            parameters.Add("@AppointmentTime", input.AppointmentTime);
            parameters.Add("@ConsentMarketing", input.ConsentMarketing ? 1 : 0);
            parameters.Add("@utm_source", input.UtmSource);
            parameters.Add("@utm_medium", input.UtmMedium);
            parameters.Add("@utm_campaign", input.UtmCampaign);
            parameters.Add("@utm_term", input.UtmTerm);
            parameters.Add("@utm_content", input.UtmContent);

            const string sql = @"
                INSERT INTO tr_transaction (
                    TransactoinID,
                    ProjectID,
                    ProjectName,
                    FirstName,
                    LastName,
                    Budget,
                    Province,
                    Distric,
                    TelNo,
                    EMail,
                    ClientFrom,
                    TransactionDate,
                    Remark,
                    AppointmentDate,
                    AppointmentTime,
                    ConsentMarketing,
                    utm_source,
                    utm_medium,
                    utm_campaign,
                    utm_term,
                    utm_content
                ) VALUES (
                    @TransactoinID,
                    @ProjectID,
                    @ProjectName,
                    @FirstName,
                    @LastName,
                    @Budget,
                    @Province,
                    @Distric,
                    @TelNo,
                    @EMail,
                    @ClientFrom,
                    @TransactionDate,
                    @Remark,
                    @AppointmentDate,
                    @AppointmentTime,
                    @ConsentMarketing,
                    @utm_source,
                    @utm_medium,
                    @utm_campaign,
                    @utm_term,
                    @utm_content
                );";

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.ExecuteAsync(sql, parameters);
                _logger.LogInformation("Inserted transaction {TransactionId} for project {ProjectId} (MappedProjectID: {MappedProjectId})",
                    transactionId, input.ProjectID, mappedProjectId ?? "NOT_FOUND");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert transaction for project {ProjectId} (MappedProjectID: {MappedProjectId})",
                    input.ProjectID, mappedProjectId ?? "NOT_FOUND");
                throw;
            }
        }
    }
}
