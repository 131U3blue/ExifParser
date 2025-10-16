using ExifParser.Images;
using ExifParser.Services;
using System.Text.Json.Serialization;
using static ExifParser.APIs.ApiUtilities;

namespace ExifParser.APIs.Shutterstock
{
    public class ShutterstockResponse : IImageResponse
    {
        [JsonPropertyName("page")]
        public int PageCount { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("search_id")]
        public string SearchId { get; set; }

        [JsonPropertyName("data")]
        public List<ShutterstockData> ShutterstockData { get; set; }

        public IEnumerable<IImage> Images => ShutterstockData;
    }

    public class ShutterstockData : IImage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("aspect")]
        public decimal Aspect { get; set; }

        [JsonPropertyName("assets")]
        public ShutterstockAssets Assets { get; set; }

        [JsonPropertyName("contributor")]
        public ShutterstockContributor Contributor { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image_type")]
        public string ImageType { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }

        //IImage implementation :)
        public string Url => Assets?.Preview?.Url ?? string.Empty;
        public ApiSource ApiSource => ApiSource.Shutterstock;
        public int Width => Assets?.Preview?.Width ?? 0;
        public int Height => Assets?.Preview?.Height ?? 0;

        public async Task<Image> RequestImageDetails(ApiCredentialService credentialService)
        {
            throw new NotImplementedException();
        }
        public void DownloadImageUsingUrl(ApiCredentialService credentialService)
        {
            throw new NotImplementedException();
        }
    }

    public class ShutterstockAssets
    {
        [JsonPropertyName("preview")]
        public ShutterstockImage Preview { get; set; }

        [JsonPropertyName("small_thumb")]
        public ShutterstockImage SmallThumb { get; set; }

        [JsonPropertyName("large_thumb")]
        public ShutterstockImage LargeThumb { get; set; }

        [JsonPropertyName("huge_thumb")]
        public ShutterstockImage HugeThumb { get; set; }

        [JsonPropertyName("preview_1000")]
        public ShutterstockImage Preview1000 { get; set; }

        [JsonPropertyName("preview_1500")]
        public ShutterstockImage Preview1500 { get; set; }
    }

    public class ShutterstockImage : Image
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class ShutterstockContributor
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
