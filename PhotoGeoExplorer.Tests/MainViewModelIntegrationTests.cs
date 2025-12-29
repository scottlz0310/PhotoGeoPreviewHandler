using PhotoGeoExplorer.Services;
using PhotoGeoExplorer.ViewModels;

namespace PhotoGeoExplorer.Tests;

public sealed class MainViewModelIntegrationTests
{
    [Fact]
    public async Task LoadFolderAsyncLoadsItemsAndBreadcrumbs()
    {
        var root = CreateTempDirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Folder"));
            await File.WriteAllTextAsync(Path.Combine(root, "note.txt"), "note").ConfigureAwait(true);

            var viewModel = new MainViewModel(new FileSystemService())
            {
                ShowImagesOnly = false
            };

            await viewModel.LoadFolderAsync(root).ConfigureAwait(true);

            Assert.Equal(root, viewModel.CurrentFolderPath);
            Assert.Equal(2, viewModel.Items.Count);
            Assert.True(viewModel.Items[0].IsFolder);
            Assert.Equal("Folder", viewModel.Items[0].FileName);
            Assert.False(viewModel.Items[1].IsFolder);
            Assert.Equal("note.txt", viewModel.Items[1].FileName);
            Assert.True(viewModel.BreadcrumbItems.Count > 0);
            Assert.Equal(root, viewModel.BreadcrumbItems.Last().FullPath);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public async Task ToggleSortBySizeReordersFiles()
    {
        var root = CreateTempDirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Folder"));
            await File.WriteAllTextAsync(Path.Combine(root, "small.txt"), "a").ConfigureAwait(true);
            await File.WriteAllTextAsync(Path.Combine(root, "large.txt"), new string('b', 200)).ConfigureAwait(true);

            var viewModel = new MainViewModel(new FileSystemService())
            {
                ShowImagesOnly = false
            };

            await viewModel.LoadFolderAsync(root).ConfigureAwait(true);
            viewModel.ToggleSort(FileSortColumn.Size);

            Assert.Equal("Folder", viewModel.Items[0].FileName);
            Assert.Equal("small.txt", viewModel.Items[1].FileName);
            Assert.Equal("large.txt", viewModel.Items[2].FileName);
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
