using System;
using System.IO;
using System.Text;

namespace PhotoGeoExplorer;

internal static class AppLog
{
    private static readonly object Gate = new();
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PhotoGeoExplorer",
        "Logs");

    private static readonly string LogPath = Path.Combine(LogDirectory, "app.log");
    private static readonly string FallbackLogDirectory = Path.Combine(
        AppContext.BaseDirectory,
        "Logs");

    private static readonly string FallbackLogPath = Path.Combine(FallbackLogDirectory, "app.log");

    internal static void Info(string message)
    {
        Write("INFO", message, null);
    }

    internal static void Error(string message, Exception? exception = null)
    {
        Write("ERROR", message, exception);
    }

    internal static string LogFilePath => LogPath;

    private static void Write(string level, string message, Exception? exception)
    {
        var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
        var builder = new StringBuilder();
        builder.Append(timestamp).Append(' ').Append(level).Append(' ').Append(message);
        if (exception is not null)
        {
            builder.AppendLine();
            builder.Append(exception);
        }

        builder.AppendLine();

        var payload = builder.ToString();

        if (TryWrite(LogDirectory, LogPath, payload))
        {
            return;
        }

        TryWrite(FallbackLogDirectory, FallbackLogPath, payload);
    }

    private static bool TryWrite(string directory, string path, string payload)
    {
        try
        {
            Directory.CreateDirectory(directory);
            lock (Gate)
            {
                File.AppendAllText(path, payload);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
