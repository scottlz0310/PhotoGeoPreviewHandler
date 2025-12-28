using System;

namespace PhotoGeoExplorer.Models;

internal sealed class BreadcrumbSegment
{
    public BreadcrumbSegment(string title, string fullPath)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
    }

    public string Title { get; }
    public string FullPath { get; }
}
