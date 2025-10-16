using ExifParser.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.Json;

namespace ExifParser.APIs.Shutterstock
{
    public class ShutterstockApiRequest<TResponse> : IApiRequest<TResponse>
    {
        //private string Token = "v2/cmtLQ1luMDl5bVdBa2IyMklOdkJGd0tPTk10SlF6VVgvMjY1MjcyODU2L2N1c3RvbWVyLzQvUVVSWW9KalNyS2E5Q0c2WlhVV2c2QkZUV2l1OXhkNklwUVQzdWxSdzc2UjQ0UVFLXzBVMUpDb2tQVHZnb016TC1rV3QzVWVXM2dTVmM1MnVaLWVJTTZqSno3LWR0Tndzck9GZmJCVl95TllBVF8zbk5jNE95ejZmaGRWUkNNYVVYX1ZYblRud2w1ZGVaOXVVdnJ4U0RnUE1JTksyWEpPVFA2cWs5LWNFUm51MVBpdW04SnpvajFkS1JsaUFVVWx6dVN6TFJuUVhfRnR4OFJNWEdpQmZVUS8xNXJ4NE5QTnNzelJkODk0TlZIMm5B";
        public string ApiName { get; } = "Shutterstock";
        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }
        public string CallbackUri { get; private set; } = "localhost";
        public string BaseUrl => "https://api.shutterstock.com/v2";
        public string Token { get; private set; }
        public string Endpoint { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; } = new();
        public HttpHeaders Headers { get; private set; }
        public int TimeoutSeconds { get; private set; } = 10;
        public int RetryCount { get; private set; } = 0;
        public int MaxRetryLimit { get; private set; } = 3;
        private readonly ApiCredentialService _apiCredentialService;

        public ShutterstockApiRequest(ApiCredentialService apiCredentialService, string endpoint)
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

        private async Task<string> RefreshOAuthToken()
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{BaseUrl}/oauth/access_token");
                client.DefaultRequestHeaders.Add("User-Agent", "EXIF Viewer");

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", ConsumerKey),
                    new KeyValuePair<string, string>("client_secret", ConsumerSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await client.PostAsync(client.BaseAddress, content);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response to extract the access token
                    var jsonResponse = System.Text.Json.JsonDocument.Parse(result);
                    if (jsonResponse.RootElement.TryGetProperty("access_token", out var accessTokenElement))
                        Token = accessTokenElement.GetString();
                }
                else
                {
                    result = $"Error: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }
            return result;
        }

        public async Task<string> ExecuteGetRequest()
        {
            string result = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                client.DefaultRequestHeaders.Add("User-Agent", "EXIF Viewer");

                var uriBuilder = new UriBuilder($"{BaseUrl}{Endpoint}")
                {                    
                    Query = string.Join("&", Parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"))
                };

                var response = await client.GetAsync(uriBuilder.Uri);
                if (response.IsSuccessStatusCode)
                    result = await response.Content.ReadAsStringAsync();                
                else
                {
                    if((response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden) && RetryCount < MaxRetryLimit)
                    {
                        // Token might be expired, try to refresh it
                        await RefreshOAuthToken();
                        RetryCount++;
                        
                        return await ExecuteGetRequest(); // Retry the request
                    }

                    result = $"Error: {response.StatusCode}, {response.ReasonPhrase}";
                }
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
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            var deserializedResponse = JsonSerializer.Deserialize<TResponse>(response);
            if (deserializedResponse == null)
                throw new InvalidOperationException("Deserialization resulted in a null object.");

            return deserializedResponse;
        }
    }

}
