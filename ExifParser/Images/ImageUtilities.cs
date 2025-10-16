namespace ExifParser.Images
{
    public static class ImageUtilities
    {
        private const string DefaultImageDownloadPath = @"C:/Downloads/Image";

        private static void EnsureDirectoryExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static async Task DownloadFileAsync(IImage image, string? destinationPath = null)
        {
            destinationPath ??= DefaultImageDownloadPath;
            EnsureDirectoryExists(destinationPath);

            using var httpClient = new HttpClient();

            using var response = await httpClient.GetAsync(image.Url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }

        public static async Task OpenDownloadedFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                }
            };
            process.Start();
        }
    }
}
