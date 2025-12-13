using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPSAsset.Services
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyTokenAsync(string token);
    }

    public class RecaptchaService : IRecaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        private const float ScoreThreshold = 0.5f; // Minimum score for valid submission

        public RecaptchaService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                var secretKey = _configuration.GetSection("RecaptchaSettings")["SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    // If secret key is not configured, allow submission for development
                    return true;
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
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<RecaptchaResponse>(content, jsonOptions);

                // Check if the response was successful and score is above threshold
                return result?.Success == true && result.Score >= ScoreThreshold;
            }
            catch
            {
                return false;
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
