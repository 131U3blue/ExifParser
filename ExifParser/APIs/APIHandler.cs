using System.Net.Http.Headers;

namespace ExifParser.APIs
{
    public class APIHandler
    {
        public static async Task<string> CallGetApi(string url, Dictionary<string, string> parameters, IAuthentication authentication, int timeout = 10)
        {
            using (var client = new HttpClient())
            {
                //Register timeout CancellationToken
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                cancellationTokenSource.Token.Register(client.CancelPendingRequests);

                //Add any headers from the IAuthentication implementation
                HttpHeaders headers = authentication.GetAuthHeaders();
                
                foreach (var header in headers)
                {
                    if(client.DefaultRequestHeaders.Contains(header.Key))
                        client.DefaultRequestHeaders.Remove(header.Key);
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                //Add the parameters to the URL
                foreach(var param in parameters)
                {
                    if (url.Contains("?"))
                        url += $"&{param.Key}={param.Value}";
                    else
                        url += $"?{param.Key}={param.Value}";
                }

                //Get the response
                var response = await client.GetAsync(url, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();      
                
                else throw new Exception($"API call failed with status code: {response.StatusCode}");
                
            }
        }
    }
}
