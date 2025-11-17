using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flipkart_search.ViewModel
{
    internal class LoginService
    {
        private readonly Random _rand = new Random();

        // STEP 1: Simulate OTP request
        public async Task<string> RequestOtp(string userId)
        {
            await Task.Delay(500); // Simulated delay
            return Guid.NewGuid().ToString("N"); // Dummy requestId
        }

        // STEP 2: Simulate Login & Token Generation
        public async Task<string> LoginAndGetToken(string userId, string requestId, string otp)
        {
            await Task.Delay(500); // Simulated delay

            // Create dummy JWT payload
            var payload = new
            {
                userId = userId,
                name = "Demo User",
                role = "customer"
            };

            string json = JsonSerializer.Serialize(payload);
            string base64Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            // Simulated token format
            return $"header.{base64Payload}.signature";
        }

        // STEP 3: Simulate profile fetch
        public async Task<string> FetchUserIdFromProfile(string accessToken)
        {
            await Task.Delay(300);

            return ExtractUserIdFromJwt(accessToken);
        }

        // Helper: Extract ID from dummy JWT
        public string ExtractUserIdFromJwt(string jwt)
        {
            string[] parts = jwt.Split('.');

            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT format.");

            string base64 = parts[1];
            byte[] bytes = Convert.FromBase64String(base64);
            string json = System.Text.Encoding.UTF8.GetString(bytes);

             JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("userId", out var id))
                return id.GetString();

            throw new InvalidOperationException("User ID not found in token.");
        }
    }
}
