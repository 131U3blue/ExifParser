using System.Net.Http.Headers;

namespace ExifParser.APIs
{
    public interface IAuthentication
    {
        public HttpHeaders GetAuthHeaders();
    }
}
