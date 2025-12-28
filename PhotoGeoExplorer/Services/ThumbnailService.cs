using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PhotoGeoExplorer.Services;

internal static class ThumbnailService
{
    private const int MaxThumbnailSize = 96;
    private static readonly string CacheDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PhotoGeoExplorer",
        "Cache",
        "Thumbnails");

    public static string? GetOrCreateThumbnailPath(string filePath, DateTime lastWriteUtc)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var cacheKey = $"{filePath}|{lastWriteUtc.Ticks}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cacheKey)));
        var thumbnailPath = Path.Combine(CacheDirectory, $"{hash}.jpg");

        if (File.Exists(thumbnailPath))
        {
            return thumbnailPath;
        }

        Directory.CreateDirectory(CacheDirectory);
        var tempPath = Path.Combine(CacheDirectory, $"{hash}.{Guid.NewGuid():N}.tmp");
        try
        {
            using var image = Image.Load(filePath);
            image.Mutate(context => context.Resize(new ResizeOptions
            {
                Size = new Size(MaxThumbnailSize, MaxThumbnailSize),
                Mode = ResizeMode.Max
            }));
            image.Save(tempPath, new JpegEncoder { Quality = 80 });
            File.Move(tempPath, thumbnailPath, overwrite: true);
            return thumbnailPath;
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLog.Error($"Failed to cache thumbnail: {filePath}", ex);
        }
        catch (IOException ex)
        {
            AppLog.Error($"Failed to cache thumbnail: {filePath}", ex);
        }
        catch (NotSupportedException ex)
        {
            AppLog.Error($"Failed to cache thumbnail: {filePath}", ex);
        }
        catch (UnknownImageFormatException ex)
        {
            AppLog.Error($"Failed to cache thumbnail: {filePath}", ex);
        }
        catch (ImageProcessingException ex)
        {
            AppLog.Error($"Failed to cache thumbnail: {filePath}", ex);
        }
        finally
        {
            TryDelete(tempPath);
        }

        return File.Exists(thumbnailPath) ? thumbnailPath : null;
    }

    private static void TryDelete(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }
}
