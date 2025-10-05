using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;

namespace ExifParser.APIs
{
    public interface IApiRequest
    {
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
        public string CallbackUri { get; }
        public string BaseUrl { get; }
        public string Token { get; }
        public string Endpoint { get; }
        public Dictionary<string, string> Parameters { get; }
        public HttpHeaders Headers { get; }
        public int TimeoutSeconds { get; }

        public int RetryCount { get; }
        public int MaxRetryLimit { get; }
        public Task<string> ExecuteRequest();
        public void AddParameter(string key, string value);
        public void AddParameters(Dictionary<string, string> parameters);
    }
}
