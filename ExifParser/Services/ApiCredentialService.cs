using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExifParser.Services
{
    public class ApiCredentialService
    {
        private readonly string _jsonPath;
        public List<ApiCredentials>? CachedCredentials { get; private set; }

        public ApiCredentialService(string jsonPath)
        {
            _jsonPath = jsonPath;
        }

        public async Task<List<ApiCredentials>> LoadApiCredentialsFromJsonAsync()
        {
            //As Blazor Renders pages twice, we cache the credentials here to prevent having to read from the file twice
            if (CachedCredentials is not null)
                return CachedCredentials;

            using var reader = new StreamReader(_jsonPath);
            string json = await reader.ReadToEndAsync();
            CachedCredentials = JsonSerializer.Deserialize<List<ApiCredentials>>(json);
            return CachedCredentials;
        }
    }

    public class ApiCredentials
    {
        [JsonPropertyName("apiName")]
        public string ApiName { get; set; }

        [JsonPropertyName("consumerKey")]
        public string ConsumerKey { get; set; }

        [JsonPropertyName("consumerSecret")]
        public string ConsumerSecret { get; set; }
    }
}
