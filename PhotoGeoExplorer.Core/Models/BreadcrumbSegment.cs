using System;

namespace PhotoGeoExplorer.Models;

internal sealed class BreadcrumbSegment
{
    public BreadcrumbSegment(string title, string fullPath, IReadOnlyList<BreadcrumbChild> children)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
        Children = children ?? throw new ArgumentNullException(nameof(children));
    }

    public string Title { get; }
    public string FullPath { get; }
    public IReadOnlyList<BreadcrumbChild> Children { get; }
    public bool HasChildren => Children.Count > 0;
}
