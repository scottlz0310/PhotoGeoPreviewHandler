using PhotoGeoExplorer.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoGeoExplorer.Tests;

public sealed class ExifServiceTests
{
    [Fact]
    public async Task UpdateMetadataAsync_UnsupportedFormat_ReturnsFalse()
    {
        var root = CreateTempDirectory();
        try
        {
            var txtPath = Path.Combine(root, "test.txt");
            await File.WriteAllTextAsync(txtPath, "test content").ConfigureAwait(true);

            var result = await ExifService.UpdateMetadataAsync(
                txtPath,
                DateTimeOffset.Now,
                35.6762,
                139.6503,
                updateFileModifiedDate: false,
                CancellationToken.None).ConfigureAwait(true);

            Assert.False(result);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public async Task UpdateMetadataAsync_ValidJpeg_ReturnsTrue()
    {
        var root = CreateTempDirectory();
        try
        {
            var jpgPath = Path.Combine(root, "test.jpg");

            // Create a simple JPEG image
            using (var image = new Image<Rgba32>(100, 100))
            {
                await image.SaveAsync(jpgPath, new JpegEncoder()).ConfigureAwait(true);
            }

            var takenAt = new DateTimeOffset(2024, 1, 15, 12, 30, 0, TimeSpan.Zero);
            var result = await ExifService.UpdateMetadataAsync(
                jpgPath,
                takenAt,
                35.6762,
                139.6503,
                updateFileModifiedDate: false,
                CancellationToken.None).ConfigureAwait(true);

            Assert.True(result);

            // Verify the metadata was written
            var metadata = await ExifService.GetMetadataAsync(jpgPath, CancellationToken.None).ConfigureAwait(true);
            Assert.NotNull(metadata);
            Assert.NotNull(metadata.TakenAt);
            Assert.Equal(takenAt.DateTime.Year, metadata.TakenAt.Value.DateTime.Year);
            Assert.Equal(takenAt.DateTime.Month, metadata.TakenAt.Value.DateTime.Month);
            Assert.Equal(takenAt.DateTime.Day, metadata.TakenAt.Value.DateTime.Day);
            Assert.NotNull(metadata.Latitude);
            Assert.NotNull(metadata.Longitude);
            Assert.True(Math.Abs(metadata.Latitude.Value - 35.6762) < 0.01);
            Assert.True(Math.Abs(metadata.Longitude.Value - 139.6503) < 0.01);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public async Task UpdateMetadataAsync_FileModifiedDateOption_UpdatesFileDate()
    {
        var root = CreateTempDirectory();
        try
        {
            var jpgPath = Path.Combine(root, "test.jpg");

            // Create a simple JPEG image
            using (var image = new Image<Rgba32>(100, 100))
            {
                await image.SaveAsync(jpgPath, new JpegEncoder()).ConfigureAwait(true);
            }

            var originalModifiedTime = File.GetLastWriteTime(jpgPath);
            var takenAt = new DateTimeOffset(2024, 1, 15, 12, 30, 0, TimeSpan.Zero);

            await ExifService.UpdateMetadataAsync(
                jpgPath,
                takenAt,
                null,
                null,
                updateFileModifiedDate: true,
                CancellationToken.None).ConfigureAwait(true);

            var newModifiedTime = File.GetLastWriteTime(jpgPath);
            Assert.NotEqual(originalModifiedTime, newModifiedTime);
            Assert.Equal(takenAt.DateTime, newModifiedTime);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public async Task UpdateMetadataAsync_NonExistentFile_ReturnsFalse()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent.jpg");

        var result = await ExifService.UpdateMetadataAsync(
            nonExistentPath,
            DateTimeOffset.Now,
            35.6762,
            139.6503,
            updateFileModifiedDate: false,
            CancellationToken.None).ConfigureAwait(true);

        Assert.False(result);
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
}
