using System;

namespace PhotoGeoExplorer.Models;

public sealed class PhotoItem
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset? TakenAt { get; set; }
    public int PixelWidth { get; set; }
    public int PixelHeight { get; set; }
    public long FileSizeBytes { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
