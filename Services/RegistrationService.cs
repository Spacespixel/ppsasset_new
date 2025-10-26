using Dapper;
using MySql.Data.MySqlClient;
using PPSAsset.Models;

namespace PPSAsset.Services
{
    public class RegistrationService
    {
        private readonly string _connectionString;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(IConfiguration configuration, ILogger<RegistrationService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not found");
            _logger = logger;
        }

        public async Task SaveAsync(RegistrationInputModel input, HttpRequest request)
        {
            var transactionId = DateTime.UtcNow.ToString("yyMMddHHmmssfff");

            var parameters = new DynamicParameters();
            parameters.Add("@TransactoinID", transactionId);
            parameters.Add("@ProjectID", input.ProjectID);
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
                _logger.LogInformation("Inserted transaction {TransactionId} for project {ProjectId}", transactionId, input.ProjectID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert transaction for project {ProjectId}", input.ProjectID);
                throw;
            }
        }
    }
}
