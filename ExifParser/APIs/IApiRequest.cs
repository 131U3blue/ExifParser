using ExifParser.Images;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;

namespace ExifParser.APIs
{

    public interface IApiRequest
    {
        Task<object> ExecuteAndDeserialise();
    }

    public class ApiRequestWrapper<TResponse> : IApiRequest
    {
        private readonly IApiRequest<TResponse> _apiRequest;
        public ApiRequestWrapper(IApiRequest<TResponse> apiRequest)
        {
            _apiRequest = apiRequest;
        }
        public async Task<object> ExecuteAndDeserialise()
        {
            string response = await _apiRequest.ExecuteGetRequest();
            return _apiRequest.DeserialiseResponse(response);
        }
    }

    public interface IApiRequest<TResponse>
    {
        public string ApiName { get; }
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
        public Task GetApiCredentials();
        public Task<string> ExecuteGetRequest();
        public Task<string> ExecutePostRequest();
        public Task<string> ExecutePutRequest();
        public Task<string> ExecuteDeleteRequest();
        public void AddParameter(string key, string value);
        public void AddParameters(Dictionary<string, string> parameters);
        public TResponse DeserialiseResponse(string response);
    }
}
