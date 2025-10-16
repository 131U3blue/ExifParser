using static ExifParser.APIs.ApiUtilities;

namespace ExifParser.Images
{
    public class Image
    {
        public string ID { get; set; }
        public ApiSource ApiSource { get; set; }
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Description { get; set; }
    }
}
