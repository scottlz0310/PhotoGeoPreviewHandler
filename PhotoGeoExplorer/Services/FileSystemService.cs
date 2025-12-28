using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoGeoExplorer.Models;

namespace PhotoGeoExplorer.Services;

internal sealed class FileSystemService
{
    private readonly HashSet<string> _imageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".tif",
        ".tiff",
        ".heic",
        ".webp"
    };

    public Task<List<PhotoItem>> GetPhotoItemsAsync(string folderPath, bool imagesOnly, string? searchText)
    {
        ArgumentNullException.ThrowIfNull(folderPath);

        var normalizedSearch = string.IsNullOrWhiteSpace(searchText) ? null : searchText.Trim();
        return Task.Run(() => EnumerateFiles(folderPath, imagesOnly, normalizedSearch));
    }

    private List<PhotoItem> EnumerateFiles(string folderPath, bool imagesOnly, string? searchText)
    {
        var directories = new List<PhotoItem>();
        var files = new List<PhotoItem>();

        foreach (var path in Directory.EnumerateDirectories(folderPath))
        {
            var directoryName = Path.GetFileName(path);
            if (searchText is not null
                && !directoryName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var info = new DirectoryInfo(path);
            directories.Add(new PhotoItem(info.FullName, 0, info.LastWriteTime, isFolder: true));
        }

        foreach (var path in Directory.EnumerateFiles(folderPath))
        {
            if (imagesOnly && !IsImage(path))
            {
                continue;
            }

            if (searchText is not null)
            {
                var fileName = Path.GetFileName(path);
                if (!fileName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            var info = new FileInfo(path);
            string? thumbnailPath = null;
            if (IsImage(info.FullName))
            {
                thumbnailPath = ThumbnailService.GetOrCreateThumbnailPath(info.FullName, info.LastWriteTimeUtc);
            }

            files.Add(new PhotoItem(info.FullName, info.Length, info.LastWriteTime, isFolder: false, thumbnailPath));
        }

        directories.Sort((left, right) => string.Compare(left.FileName, right.FileName, StringComparison.OrdinalIgnoreCase));
        files.Sort((left, right) => string.Compare(left.FileName, right.FileName, StringComparison.OrdinalIgnoreCase));

        var items = new List<PhotoItem>(directories.Count + files.Count);
        items.AddRange(directories);
        items.AddRange(files);
        return items;
    }

    private bool IsImage(string path)
    {
        var extension = Path.GetExtension(path);
        return _imageExtensions.Contains(extension);
    }

    public static List<BreadcrumbChild> GetChildDirectories(string folderPath)
    {
        ArgumentNullException.ThrowIfNull(folderPath);

        var children = new List<BreadcrumbChild>();
        try
        {
            foreach (var path in Directory.EnumerateDirectories(folderPath))
            {
                var name = Path.GetFileName(path);
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                children.Add(new BreadcrumbChild(name, path));
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLog.Error($"Failed to read child folders: {folderPath}", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            AppLog.Error($"Failed to read child folders: {folderPath}", ex);
        }
        catch (PathTooLongException ex)
        {
            AppLog.Error($"Failed to read child folders: {folderPath}", ex);
        }
        catch (IOException ex)
        {
            AppLog.Error($"Failed to read child folders: {folderPath}", ex);
        }

        children.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase));
        return children;
    }
}
