using PhotoGeoExplorer.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoGeoExplorer.Tests;

public sealed class FileSystemIntegrationTests
{
    [Fact]
    public async Task GetPhotoItemsAsyncCreatesThumbnailForImage()
    {
        var root = CreateTempDirectory();
        string? thumbnailPath = null;
        try
        {
            var imagePath = Path.Combine(root, "image.png");
            using (var image = new Image<Rgba32>(1, 1))
            {
                image[0, 0] = new Rgba32(255, 255, 255, 255);
                await image.SaveAsPngAsync(imagePath).ConfigureAwait(true);
            }

            var service = new FileSystemService();
            var items = await service.GetPhotoItemsAsync(root, imagesOnly: true, searchText: null).ConfigureAwait(true);

            var item = Assert.Single(items);
            Assert.False(item.IsFolder);
            Assert.NotNull(item.ThumbnailPath);
            thumbnailPath = item.ThumbnailPath;
            Assert.True(File.Exists(thumbnailPath));
            Assert.Equal(1, item.PixelWidth);
            Assert.Equal(1, item.PixelHeight);
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(thumbnailPath))
            {
                TryDeleteFile(thumbnailPath);
            }

            TryDeleteDirectory(root);
        }
    }

    private static string CreateTempDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), "PhotoGeoExplorerTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
