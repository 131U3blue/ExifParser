using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace ExifParser.APIs.Shutterstock
{
    public class ShutterstockApiRequest : IApiRequest
    {
        //private string Token = "v2/cmtLQ1luMDl5bVdBa2IyMklOdkJGd0tPTk10SlF6VVgvMjY1MjcyODU2L2N1c3RvbWVyLzQvUVVSWW9KalNyS2E5Q0c2WlhVV2c2QkZUV2l1OXhkNklwUVQzdWxSdzc2UjQ0UVFLXzBVMUpDb2tQVHZnb016TC1rV3QzVWVXM2dTVmM1MnVaLWVJTTZqSno3LWR0Tndzck9GZmJCVl95TllBVF8zbk5jNE95ejZmaGRWUkNNYVVYX1ZYblRud2w1ZGVaOXVVdnJ4U0RnUE1JTksyWEpPVFA2cWs5LWNFUm51MVBpdW04SnpvajFkS1JsaUFVVWx6dVN6TFJuUVhfRnR4OFJNWEdpQmZVUS8xNXJ4NE5QTnNzelJkODk0TlZIMm5B";
        public string ConsumerKey => "rkKCYn09ymWAkb22INvBFwKONMtJQzUX";
        public string ConsumerSecret => "pvwZ049W3ig8sLuj";
        public string CallbackUri => "localhost";
        public string BaseUrl => "https://api.shutterstock.com/v2";
        public string Token { get; private set; }
        public string Endpoint { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; } = new();
        public HttpHeaders Headers { get; private set; }
        public int TimeoutSeconds { get; private set; } = 10;
        public int RetryCount { get; private set; } = 0;
        public int MaxRetryLimit { get; private set; } = 3;


        private async Task<string> RefreshOAuthToken()
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{BaseUrl}/oauth/authorize");
                client.DefaultRequestHeaders.Add("User-Agent", "EXIF Viewer");


                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", ConsumerKey),
                    new KeyValuePair<string, string>("redirect_uri", CallbackUri),
                    new KeyValuePair<string, string>("response_type", "code"),
                    new KeyValuePair<string, string>("response_type", "user.view, collections.edit, collections.view"),
                });

                var response = await client.GetAsync($"{client.BaseAddress}?{content.ReadAsStringAsync()}");
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

        public async Task<string> ExecuteRequest()
        {
            string result = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                // Build the full URL with parameters
                var url = Endpoint;
                if (Parameters.Count > 0)
                {
                    var queryString = string.Join("&", Parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
                    url += "?" + queryString;
                }
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    if((response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden) && RetryCount < MaxRetryLimit)
                    {
                        // Token might be expired, try to refresh it
                        await RefreshOAuthToken();
                        RetryCount++;
                        return await ExecuteRequest(); // Retry the request
                    }

                    result = $"Error: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }

            return result;
        }

        public ShutterstockApiRequest(string endpoint)
        {
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


    }

}
