using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace PhotoGeoExplorer;

[SuppressMessage("Design", "CA1515:Consider making public types internal")]
public sealed partial class MainWindow : Window
{
    private bool _mapInitialized;
    private bool _windowSized;

    public MainWindow()
    {
        InitializeComponent();
        AppLog.Info("MainWindow constructed.");
        Activated += OnActivated;
    }

    private async void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_mapInitialized)
        {
            return;
        }

        EnsureWindowSize();
        _mapInitialized = true;
        AppLog.Info("MainWindow activated.");
        await InitializeMapAsync().ConfigureAwait(false);
    }

    private void EnsureWindowSize()
    {
        if (_windowSized)
        {
            return;
        }

        _windowSized = true;

        try
        {
            AppWindow.Resize(new SizeInt32(1200, 800));
        }
        catch (ArgumentException ex)
        {
            AppLog.Error("Failed to set initial window size.", ex);
        }
        catch (InvalidOperationException ex)
        {
            AppLog.Error("Failed to set initial window size.", ex);
        }
        catch (System.Runtime.InteropServices.COMException ex)
        {
            AppLog.Error("Failed to set initial window size.", ex);
        }
    }

    private async Task InitializeMapAsync()
    {
        try
        {
            var indexPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html");
            if (!File.Exists(indexPath))
            {
                AppLog.Error($"Map page not found: {indexPath}");
                return;
            }

            AppLog.Info("Initializing WebView2.");
            await MapWebView.EnsureCoreWebView2Async();
            MapWebView.Source = new Uri(indexPath);
            AppLog.Info("WebView2 initialized.");
        }
        catch (InvalidOperationException ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
        catch (IOException ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
        catch (System.Runtime.InteropServices.COMException ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
        catch (UriFormatException ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
    }
}
