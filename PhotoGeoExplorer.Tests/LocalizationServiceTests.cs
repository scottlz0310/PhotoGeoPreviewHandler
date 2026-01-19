using PhotoGeoExplorer.Services;

namespace PhotoGeoExplorer.Tests;

public sealed class LocalizationServiceTests
{
    [Fact]
    public void GetString_WithNullKey_ReturnsEmptyString()
    {
        var result = LocalizationService.GetString(null!);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetString_WithEmptyKey_ReturnsEmptyString()
    {
        var result = LocalizationService.GetString(string.Empty);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetString_WithWhitespaceKey_ReturnsEmptyString()
    {
        var result = LocalizationService.GetString("   ");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetString_WithValidKey_ReturnsKeyInTestEnvironment()
    {
        // In test environment, ResourceManager is null, so the key is returned as-is
        var key = "TestKey";

        var result = LocalizationService.GetString(key);

        Assert.Equal(key, result);
    }

    [Fact]
    public void GetString_WithDottedKey_ReturnsKeyInTestEnvironment()
    {
        // Tests that dotted keys are handled (normalized to slashes internally)
        var key = "MainWindow.Title";

        var result = LocalizationService.GetString(key);

        Assert.Equal(key, result);
    }

    [Fact]
    public void Format_WithNullKey_ReturnsEmptyString()
    {
        var result = LocalizationService.Format(null!, "arg1");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Format_WithEmptyKey_ReturnsEmptyString()
    {
        var result = LocalizationService.Format(string.Empty, "arg1");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Format_WithValidKeyAndArgs_FormatsCorrectly()
    {
        // In test environment, key is returned as format string
        var key = "Hello {0}, you have {1} messages";

        var result = LocalizationService.Format(key, "User", 5);

        Assert.Equal("Hello User, you have 5 messages", result);
    }

    [Fact]
    public void Format_WithNoArgs_ReturnsKeyAsIs()
    {
        var key = "SimpleMessage";

        var result = LocalizationService.Format(key);

        Assert.Equal(key, result);
    }

    [Fact]
    public void Format_WithSingleArg_FormatsCorrectly()
    {
        var key = "Welcome {0}!";

        var result = LocalizationService.Format(key, "World");

        Assert.Equal("Welcome World!", result);
    }

    [Fact]
    public async Task GetString_IsThreadSafe()
    {
        // Verifies that concurrent access doesn't throw exceptions
        var tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                var result = LocalizationService.GetString($"Key{index}");
                Assert.Equal($"Key{index}", result);
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(true);
    }

    [Fact]
    public async Task Format_IsThreadSafe()
    {
        // Verifies that concurrent Format calls don't throw exceptions
        var tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                var result = LocalizationService.Format("Message {0}", index);
                Assert.Equal($"Message {index}", result);
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(true);
    }

    [Fact]
    public void GetString_WithSpecialCharacters_ReturnsKey()
    {
        var key = "Key.With.Dots.And/Slashes";

        var result = LocalizationService.GetString(key);

        Assert.Equal(key, result);
    }

    [Fact]
    public void GetString_CalledMultipleTimes_ReturnsSameResult()
    {
        var key = "ConsistentKey";

        var result1 = LocalizationService.GetString(key);
        var result2 = LocalizationService.GetString(key);
        var result3 = LocalizationService.GetString(key);

        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    [Fact]
    public void Format_WithNumericArgs_FormatsWithCurrentCulture()
    {
        var key = "Value: {0:N2}";

        var result = LocalizationService.Format(key, 1234.5678);

        // Result depends on current culture but should not throw
        Assert.NotNull(result);
        Assert.Contains("Value:", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Format_WithDateArgs_FormatsWithCurrentCulture()
    {
        var key = "Date: {0:d}";
        var date = new DateTime(2024, 1, 15);

        var result = LocalizationService.Format(key, date);

        // Result depends on current culture but should not throw
        Assert.NotNull(result);
        Assert.Contains("Date:", result, StringComparison.Ordinal);
    }
}
