using ExifParser.Images;

namespace ExifParser.APIs
{
    public interface IImageResponse
    {
        IEnumerable<IImage> Images { get; }
    }
}
