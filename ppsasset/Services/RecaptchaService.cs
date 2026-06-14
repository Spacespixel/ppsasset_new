using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPSAsset.Services
{
    public class RecaptchaVerificationResult
    {
        public bool Success { get; set; }
        public float Score { get; set; }
        public string[] ErrorCodes { get; set; }
        public string Hostname { get; set; }
    }

    public interface IRecaptchaService
    {
        Task<RecaptchaVerificationResult> VerifyTokenAsync(string token);
    }

    public class RecaptchaService : IRecaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RecaptchaService> _logger;
        private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        private const float ScoreThreshold = 0.5f; // Minimum score for valid submission

        public RecaptchaService(IConfiguration configuration, HttpClient httpClient, ILogger<RecaptchaService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<RecaptchaVerificationResult> VerifyTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("reCAPTCHA verification failed: Token is missing or empty.");
                    return new RecaptchaVerificationResult { Success = false, ErrorCodes = new[] { "missing-input-response" } };
                }

                var secretKey = _configuration.GetSection("RecaptchaSettings")["SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    // If secret key is not configured, allow submission for development
                    _logger.LogWarning("reCAPTCHA secret key is missing. Bypassing verification (returning true).");
                    return new RecaptchaVerificationResult { Success = true, Score = 1.0f };
                }

                var request = new HttpRequestMessage(HttpMethod.Post, RecaptchaVerifyUrl)
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("secret", secretKey),
                        new KeyValuePair<string, string>("response", token)
                    })
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("reCAPTCHA API request failed. Status Code: {StatusCode}", response.StatusCode);
                    return new RecaptchaVerificationResult { Success = false, ErrorCodes = new[] { $"http-error-{response.StatusCode}" } };
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<RecaptchaResponse>(content, jsonOptions);

                if (result == null)
                {
                    _logger.LogError("reCAPTCHA API response could not be deserialized. Content: {Content}", content);
                    return new RecaptchaVerificationResult { Success = false, ErrorCodes = new[] { "deserialization-failed" } };
                }

                if (!result.Success)
                {
                    _logger.LogWarning("reCAPTCHA verification failed. Success: false. ErrorCodes: {ErrorCodes}", 
                        result.ErrorCodes != null ? string.Join(", ", result.ErrorCodes) : "None");
                    return new RecaptchaVerificationResult { Success = false, ErrorCodes = result.ErrorCodes, Score = result.Score, Hostname = result.Hostname };
                }

                if (result.Score < ScoreThreshold)
                {
                    _logger.LogWarning("reCAPTCHA verification failed due to low score. Score: {Score} (Threshold: {Threshold}). Action: {Action}, Hostname: {Hostname}", 
                        result.Score, ScoreThreshold, result.Action, result.Hostname);
                    return new RecaptchaVerificationResult { Success = false, Score = result.Score, ErrorCodes = new[] { "low-score" }, Hostname = result.Hostname };
                }

                _logger.LogInformation("reCAPTCHA verification successful. Score: {Score}, Action: {Action}, Hostname: {Hostname}", 
                    result.Score, result.Action, result.Hostname);

                return new RecaptchaVerificationResult { Success = true, Score = result.Score, Hostname = result.Hostname };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "reCAPTCHA verification threw an exception.");
                return new RecaptchaVerificationResult { Success = false, ErrorCodes = new[] { "exception" } };
            }
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            public float Score { get; set; }
            public string Action { get; set; }
            public long ChallengeTs { get; set; }
            public string Hostname { get; set; }
            public string[] ErrorCodes { get; set; }
        }
    }
}
