using System;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace PhotoGeoExplorer;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppLog.Info("Program.Main starting.");

        try
        {
            Microsoft.WindowsAppRuntime.Bootstrap.Initialize(0x00010006);
            AppLog.Info("Windows App Runtime bootstrap initialized.");
        }
        catch (Exception ex)
        {
            AppLog.Error("Windows App SDK bootstrap failed.", ex);
        }

        try
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Application.Start(args =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = args;
                _ = new App();
            });
        }
        catch (Exception ex)
        {
            AppLog.Error("Application.Start failed.", ex);
            throw;
        }
        finally
        {
            try
            {
                Microsoft.WindowsAppRuntime.Bootstrap.Shutdown();
                AppLog.Info("Windows App Runtime bootstrap shutdown.");
            }
            catch (Exception ex)
            {
                AppLog.Error("Windows App SDK bootstrap shutdown failed.", ex);
            }
        }
    }
}
