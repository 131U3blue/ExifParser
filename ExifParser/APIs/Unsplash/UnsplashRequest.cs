using ExifParser.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ExifParser.APIs.Unsplash
{
    public class UnsplashRequest<TResponse> : IApiRequest<TResponse>
    {
        public string ApiName { get; } = "Unsplash";

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }

        public string CallbackUri { get; private set; } = "localhost";

        public string BaseUrl => "https://api.unsplash.com/";

        public string Token { get; set; }

        public string Endpoint { get; private set; }

        public Dictionary<string, string> Parameters { get; private set; } = new();

        public HttpHeaders Headers { get; private set; }

        public int TimeoutSeconds { get; private set; } = 10;

        public int RetryCount { get; private set; } = 0;
        public int MaxRetryLimit { get; private set; } = 3;

        private readonly ApiCredentialService _apiCredentialService;

        public UnsplashRequest(ApiCredentialService apiCredentialService, string endpoint)
        {
            _apiCredentialService = apiCredentialService;

            // Ensure CachedCredentials is not null before accessing it
            if (apiCredentialService.CachedCredentials is null)
                throw new ArgumentNullException(nameof(apiCredentialService.CachedCredentials), "CachedCredentials cannot be null.");

            var credentials = apiCredentialService.CachedCredentials.FirstOrDefault(cred => cred.ApiName == ApiName);
            
            // Ensure credentials are not null before accessing their properties
            if (credentials is null)
                throw new InvalidOperationException($"No credentials found for API: {ApiName}");
            
            ConsumerKey = credentials.ConsumerKey;
            ConsumerSecret = credentials.ConsumerSecret;
            Endpoint = endpoint;
        }

        public void AddParameter(string key, string value)
        {
            Parameters[key] = value;
        }

        public void AddParameters(Dictionary<string, string> newParameters)
        {
            foreach (var param in newParameters)
            {
                Parameters[param.Key] = param.Value;
            }
        }

        public async Task<string> ExecuteGetRequest()
        {
            string result = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", ConsumerKey);

                var uriBuilder = new UriBuilder($"{BaseUrl}{Endpoint}")
                {
                    Query = string.Join("&", Parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"))
                };

                var response = await client.GetAsync(uriBuilder.Uri);
                if(response.IsSuccessStatusCode)
                    result = await response.Content.ReadAsStringAsync();                
                else
                    result = $"Error: {response.StatusCode}, {response.ReasonPhrase}";
                
            }

            return result;
        }

        public Task<string> ExecutePostRequest()
        {
            throw new NotImplementedException();
        }

        public Task<string> ExecutePutRequest()
        {
            throw new NotImplementedException();
        }

        public Task<string> ExecuteDeleteRequest()
        {
            throw new NotImplementedException();
        }

        public async Task GetApiCredentials()
        {
            var allCredentials = await _apiCredentialService.LoadApiCredentialsFromJsonAsync();
            var credentials = allCredentials.Where(cred => cred.ApiName == ApiName);
            if (credentials.Any())
            {
                var cred = credentials.First();
                ConsumerKey = cred.ConsumerKey;
                ConsumerSecret = cred.ConsumerSecret;
            }
            else
            {
                throw new Exception($"No credentials found for API: {ApiName}");
            }
        }

        public TResponse DeserialiseResponse(string response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    throw new ArgumentException("Response cannot be null or empty.", nameof(response));

                var deserializedResponse = JsonSerializer.Deserialize<TResponse>(response);
                if (deserializedResponse is null)
                    throw new InvalidOperationException("Deserialization resulted in a null object.");

                return deserializedResponse;
            }
            catch
            {
                return default;
            }
        }
    }
}
