using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using PhotoGeoExplorer.Models;
using PhotoGeoExplorer.Services;

namespace PhotoGeoExplorer.ViewModels;

internal sealed class MainViewModel : BindableBase
{
    private readonly FileSystemService _fileSystemService;
    private string? _currentFolderPath;
    private string? _statusMessage;
    private Visibility _statusVisibility = Visibility.Collapsed;
    private PhotoItem? _selectedItem;
    private BitmapImage? _selectedPreview;
    private Visibility _previewPlaceholderVisibility = Visibility.Visible;
    private bool _showImagesOnly = true;
    private string? _searchText;

    public MainViewModel(FileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        Items = new ObservableCollection<PhotoItem>();
        BreadcrumbItems = new ObservableCollection<BreadcrumbSegment>();
    }

    public ObservableCollection<PhotoItem> Items { get; }
    public ObservableCollection<BreadcrumbSegment> BreadcrumbItems { get; }

    public string? CurrentFolderPath
    {
        get => _currentFolderPath;
        private set => SetProperty(ref _currentFolderPath, value);
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public Visibility StatusVisibility
    {
        get => _statusVisibility;
        private set => SetProperty(ref _statusVisibility, value);
    }

    public bool ShowImagesOnly
    {
        get => _showImagesOnly;
        set => SetProperty(ref _showImagesOnly, value);
    }

    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public PhotoItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                UpdatePreview(value);
            }
        }
    }

    public BitmapImage? SelectedPreview
    {
        get => _selectedPreview;
        private set => SetProperty(ref _selectedPreview, value);
    }

    public Visibility PreviewPlaceholderVisibility
    {
        get => _previewPlaceholderVisibility;
        private set => SetProperty(ref _previewPlaceholderVisibility, value);
    }

    public async Task InitializeAsync()
    {
        if (!string.IsNullOrWhiteSpace(CurrentFolderPath))
        {
            return;
        }

        await OpenHomeAsync().ConfigureAwait(true);
    }

    public async Task OpenHomeAsync()
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        if (string.IsNullOrWhiteSpace(homePath) || !Directory.Exists(homePath))
        {
            SetStatus("Pictures folder not found.");
            return;
        }

        await LoadFolderAsync(homePath).ConfigureAwait(true);
    }

    public async Task NavigateUpAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFolderPath))
        {
            return;
        }

        var parent = Directory.GetParent(CurrentFolderPath);
        if (parent is null)
        {
            return;
        }

        await LoadFolderAsync(parent.FullName).ConfigureAwait(true);
    }

    public async Task RefreshAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFolderPath))
        {
            return;
        }

        await LoadFolderAsync(CurrentFolderPath).ConfigureAwait(true);
    }

    public async Task LoadFolderAsync(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            SetStatus("Folder path is empty.");
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            SetStatus("Folder not found.");
            return;
        }

        try
        {
            CurrentFolderPath = folderPath;
            UpdateBreadcrumbs(folderPath);
            SetStatus(null);
            SelectedItem = null;

            var items = await _fileSystemService
                .GetPhotoItemsAsync(folderPath, ShowImagesOnly, SearchText)
                .ConfigureAwait(true);
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }

            SetStatus(Items.Count == 0 ? "No files found." : null);
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLog.Error($"Failed to access folder: {folderPath}", ex);
            SetStatus("Access denied. See log.");
        }
        catch (DirectoryNotFoundException ex)
        {
            AppLog.Error($"Folder not found: {folderPath}", ex);
            SetStatus("Folder not found. See log.");
        }
        catch (PathTooLongException ex)
        {
            AppLog.Error($"Folder path too long: {folderPath}", ex);
            SetStatus("Folder path too long. See log.");
        }
        catch (IOException ex)
        {
            AppLog.Error($"Failed to read folder: {folderPath}", ex);
            SetStatus("Failed to read folder. See log.");
        }
    }

    private void UpdatePreview(PhotoItem? item)
    {
        if (item is null)
        {
            SelectedPreview = null;
            PreviewPlaceholderVisibility = Visibility.Visible;
            return;
        }

        try
        {
            SelectedPreview = new BitmapImage(new Uri(item.FilePath));
            PreviewPlaceholderVisibility = Visibility.Collapsed;
        }
        catch (ArgumentException ex)
        {
            AppLog.Error("Failed to load preview image.", ex);
            SelectedPreview = null;
            PreviewPlaceholderVisibility = Visibility.Visible;
        }
        catch (UriFormatException ex)
        {
            AppLog.Error("Failed to load preview image.", ex);
            SelectedPreview = null;
            PreviewPlaceholderVisibility = Visibility.Visible;
        }
    }

    private void UpdateBreadcrumbs(string folderPath)
    {
        BreadcrumbItems.Clear();

        var root = Path.GetPathRoot(folderPath);
        if (string.IsNullOrWhiteSpace(root))
        {
            return;
        }

        var trimmedRoot = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var currentPath = root;
        BreadcrumbItems.Add(new BreadcrumbSegment(trimmedRoot, root));

        var remaining = folderPath[root.Length..];
        if (string.IsNullOrWhiteSpace(remaining))
        {
            return;
        }

        var segments = remaining.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in segments)
        {
            currentPath = Path.Combine(currentPath, segment);
            BreadcrumbItems.Add(new BreadcrumbSegment(segment, currentPath));
        }
    }

    private void SetStatus(string? message)
    {
        StatusMessage = message;
        StatusVisibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
    }
}
