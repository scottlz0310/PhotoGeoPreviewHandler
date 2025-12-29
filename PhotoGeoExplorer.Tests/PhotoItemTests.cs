using System.Globalization;
using PhotoGeoExplorer.Models;

namespace PhotoGeoExplorer.Tests;

public sealed class PhotoItemTests
{
    [Fact]
    public void SizeTextReturnsEmptyForFolder()
    {
        using var _ = new CultureScope(CultureInfo.InvariantCulture);
        var item = new PhotoItem("C:\\temp\\folder", 2048, DateTimeOffset.UtcNow, isFolder: true);

        Assert.Equal(string.Empty, item.SizeText);
    }

    [Fact]
    public void SizeTextFormatsBytes()
    {
        using var _ = new CultureScope(CultureInfo.InvariantCulture);
        var item = new PhotoItem("C:\\temp\\file.txt", 1024, DateTimeOffset.UtcNow, isFolder: false);

        Assert.Equal("1 KB", item.SizeText);
    }

    [Fact]
    public void ResolutionTextReturnsEmptyWhenMissingDimensions()
    {
        var item = new PhotoItem("C:\\temp\\file.jpg", 100, DateTimeOffset.UtcNow, isFolder: false);

        Assert.Equal(string.Empty, item.ResolutionText);
    }

    [Fact]
    public void ResolutionTextFormatsDimensions()
    {
        using var _ = new CultureScope(CultureInfo.InvariantCulture);
        var item = new PhotoItem("C:\\temp\\file.jpg", 100, DateTimeOffset.UtcNow, isFolder: false, pixelWidth: 1920, pixelHeight: 1080);

        Assert.Equal("1920 x 1080", item.ResolutionText);
    }
}
