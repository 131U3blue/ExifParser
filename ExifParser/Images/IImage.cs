using ExifParser.Services;
using static ExifParser.APIs.ApiUtilities;

namespace ExifParser.Images
{
    public interface IImage
    {
        string Id { get; }
        string Description { get; }
        string Url { get; }
        ApiSource ApiSource { get; }
        int Width { get; }
        int Height { get; }

        public Task<Image> RequestImageDetails(ApiCredentialService credentialService);

        public void DownloadImageUsingUrl(ApiCredentialService credentialService);
    }
}
