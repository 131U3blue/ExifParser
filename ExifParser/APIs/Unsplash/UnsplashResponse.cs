using System.Text.Json.Serialization;

namespace ExifParser.APIs.Unsplash
{
    using ExifParser.Images;
    using ExifParser.Services;
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using static ExifParser.APIs.ApiUtilities;

    public class UnsplashResponse : IImageResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("results")]
        public List<UnsplashImage> Results { get; set; } = new();

        public IEnumerable<IImage> Images => Results;
    }

    public class UnsplashImage : IImage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("blur_hash")]
        public string BlurHash { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("downloads")]
        public int? Downloads { get; set; }

        [JsonPropertyName("liked_by_user")]
        public bool LikedByUser { get; set; }

        [JsonPropertyName("public_domain")]
        public bool? PublicDomain { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("tags")]
        public List<UnsplashTag> Tags { get; set; } = new();

        [JsonPropertyName("exif")]
        public UnsplashExif Exif { get; set; }

        [JsonPropertyName("location")]
        public UnsplashLocation Location { get; set; }

        [JsonPropertyName("current_user_collections")]
        public List<UnsplashCollection> CurrentUserCollections { get; set; } = new();

        [JsonPropertyName("urls")]
        public UnsplashUrls Urls { get; set; }

        //IImage implementation
        public string Url => Urls.Regular;
        public ApiSource ApiSource => ApiSource.Unsplash;

        [JsonPropertyName("links")]
        public UnsplashLinks Links { get; set; }

        [JsonPropertyName("user")]
        public UnsplashUser User { get; set; }

        public void DownloadImageUsingUrl(ApiCredentialService credentialService)
        {
            throw new NotImplementedException();
        }

        public async Task<Image> RequestImageDetails(ApiCredentialService credentialService)
        {
            var request = new UnsplashRequest<UnsplashResponse>(credentialService, $"/photos/{this.Id}");
            var response = await request.ExecuteGetRequest();

            var responseImage = request.DeserialiseResponse(response);
            return new Image();
        }
    }

    public class UnsplashTag
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    public class UnsplashExif
    {
        [JsonPropertyName("make")]
        public string Make { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("exposure_time")]
        public string ExposureTime { get; set; }

        [JsonPropertyName("aperture")]
        public string Aperture { get; set; }

        [JsonPropertyName("focal_length")]
        public string FocalLength { get; set; }

        [JsonPropertyName("iso")]
        public int? Iso { get; set; }
    }

    public class UnsplashLocation
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("position")]
        public UnsplashPosition Position { get; set; }
    }

    public class UnsplashPosition
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }

    public class UnsplashCollection
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime? PublishedAt { get; set; }

        [JsonPropertyName("last_collected_at")]
        public DateTime? LastCollectedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("cover_photo")]
        public object CoverPhoto { get; set; }

        [JsonPropertyName("user")]
        public object User { get; set; }
    }

    public class UnsplashUrls
    {
        [JsonPropertyName("raw")]
        public string Raw { get; set; }

        [JsonPropertyName("full")]
        public string Full { get; set; }

        [JsonPropertyName("regular")]
        public string Regular { get; set; }

        [JsonPropertyName("small")]
        public string Small { get; set; }

        [JsonPropertyName("thumb")]
        public string Thumb { get; set; }
    }

    public class UnsplashLinks
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("html")]
        public string Html { get; set; }

        [JsonPropertyName("download")]
        public string Download { get; set; }

        [JsonPropertyName("download_location")]
        public string DownloadLocation { get; set; }
    }

    public class UnsplashUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("portfolio_url")]
        public string PortfolioUrl { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("total_likes")]
        public int? TotalLikes { get; set; }

        [JsonPropertyName("total_photos")]
        public int? TotalPhotos { get; set; }

        [JsonPropertyName("total_collections")]
        public int? TotalCollections { get; set; }

        [JsonPropertyName("profile_image")]
        public UnsplashProfileImage ProfileImage { get; set; }

        [JsonPropertyName("links")]
        public UnsplashUserLinks Links { get; set; }
    }

    public class UnsplashProfileImage
    {
        [JsonPropertyName("small")]
        public string Small { get; set; }

        [JsonPropertyName("medium")]
        public string Medium { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }
    }

    public class UnsplashUserLinks
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("html")]
        public string Html { get; set; }

        [JsonPropertyName("photos")]
        public string Photos { get; set; }

        [JsonPropertyName("likes")]
        public string Likes { get; set; }

        [JsonPropertyName("portfolio")]
        public string Portfolio { get; set; }
    }


}