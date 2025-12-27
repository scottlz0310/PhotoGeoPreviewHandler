using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Windows.Graphics;

namespace PhotoGeoExplorer;

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
        await InitializeMapAsync();
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
            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = WinRT.Interop.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(1200, 800));
        }
        catch (Exception ex)
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
        catch (Exception ex)
        {
            AppLog.Error("Map WebView2 init failed.", ex);
        }
    }
}
