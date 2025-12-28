using System;

namespace PhotoGeoExplorer.Models;

internal sealed class BreadcrumbChild
{
    public BreadcrumbChild(string name, string fullPath)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
    }

    public string Name { get; }
    public string FullPath { get; }
}
