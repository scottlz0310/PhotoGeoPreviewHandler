using PhotoGeoExplorer.Models;
using PhotoGeoExplorer.Services;
using PhotoGeoExplorer.ViewModels;

namespace PhotoGeoExplorer.Tests;

public sealed class SettingsServiceTests
{
    [Fact]
    public async Task ExportImportRoundTripsSettings()
    {
        var root = CreateTempDirectory();
        var path = Path.Combine(root, "settings.json");
        try
        {
            var settings = new AppSettings
            {
                LastFolderPath = "C:\\Photos",
                ShowImagesOnly = false,
                FileViewMode = FileViewMode.Icon,
                Language = "en-US",
                Theme = ThemePreference.Dark
            };

            await SettingsService.ExportAsync(settings, path).ConfigureAwait(true);
            var imported = await SettingsService.ImportAsync(path).ConfigureAwait(true);

            Assert.NotNull(imported);
            Assert.Equal(settings.LastFolderPath, imported!.LastFolderPath);
            Assert.Equal(settings.ShowImagesOnly, imported.ShowImagesOnly);
            Assert.Equal(settings.FileViewMode, imported.FileViewMode);
            Assert.Equal(settings.Language, imported.Language);
            Assert.Equal(settings.Theme, imported.Theme);
        }
        finally
        {
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
}
