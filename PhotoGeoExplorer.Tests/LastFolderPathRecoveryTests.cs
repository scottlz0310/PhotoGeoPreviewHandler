using System.Reflection;

namespace PhotoGeoExplorer.Tests;

[Collection("NonParallel")]
public sealed class LastFolderPathRecoveryTests
{
    private static readonly MethodInfo? FindValidAncestorPathMethod = ResolveFindValidAncestorPathMethod();

    [Fact]
    public void FindValidAncestorPathReturnsPathWhenValid()
    {
        // Arrange
        var tempRoot = CreateTempDirectory();
        try
        {
            var validPath = Path.Combine(tempRoot, "subfolder");
            Directory.CreateDirectory(validPath);

            // Act
            if (!TryInvokeFindValidAncestorPath(validPath, out var result))
            {
                return;
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Path.GetFullPath(validPath), result);
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
        }
    }

    [Fact]
    public void FindValidAncestorPathReturnsParentWhenChildNotExists()
    {
        // Arrange
        var tempRoot = CreateTempDirectory();
        try
        {
            var parentPath = Path.Combine(tempRoot, "parent");
            var childPath = Path.Combine(parentPath, "child");
            Directory.CreateDirectory(parentPath);
            // childPath は作成しない

            // Act
            if (!TryInvokeFindValidAncestorPath(childPath, out var result))
            {
                return;
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Path.GetFullPath(parentPath), result);
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
        }
    }

    [Fact]
    public void FindValidAncestorPathReturnsGrandparentWhenParentAndChildNotExist()
    {
        // Arrange
        var tempRoot = CreateTempDirectory();
        try
        {
            var grandparentPath = Path.Combine(tempRoot, "grandparent");
            var parentPath = Path.Combine(grandparentPath, "parent");
            var childPath = Path.Combine(parentPath, "child");
            Directory.CreateDirectory(grandparentPath);
            // parentPath と childPath は作成しない

            // Act
            if (!TryInvokeFindValidAncestorPath(childPath, out var result))
            {
                return;
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Path.GetFullPath(grandparentPath), result);
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
        }
    }

    [Fact]
    public void FindValidAncestorPathReturnsNullWhenNoValidAncestor()
    {
        // Arrange
        // 存在しないドライブやルートパスを使用
        var invalidPath = Path.Combine("Z:", "nonexistent", "path", "folder");

        // Act
        if (!TryInvokeFindValidAncestorPath(invalidPath, out var result))
        {
            return;
        }

        // Assert
        // Z: ドライブが存在する場合は Z: が返る可能性があるため、
        // 結果が null または無効なパスであることを確認
        if (result is not null)
        {
            // 返されたパスが元のパスの祖先であることを確認
            var normalizedResult = Path.GetFullPath(result);
            var normalizedInvalid = Path.GetFullPath(invalidPath);
            Assert.True(
                normalizedInvalid.StartsWith(normalizedResult, StringComparison.OrdinalIgnoreCase),
                $"Expected '{normalizedInvalid}' to start with '{normalizedResult}'");
        }
    }

    [Fact]
    public void FindValidAncestorPathReturnsNullForEmptyPath()
    {
        // Act
        if (!TryInvokeFindValidAncestorPath(string.Empty, out var result))
        {
            return;
        }

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindValidAncestorPathReturnsNullForNullPath()
    {
        // Act
        if (!TryInvokeFindValidAncestorPath(null, out var result))
        {
            return;
        }

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindValidAncestorPathHandlesRelativePaths()
    {
        // Arrange
        var tempRoot = CreateTempDirectory();
        try
        {
            var subfolder = Path.Combine(tempRoot, "subfolder");
            Directory.CreateDirectory(subfolder);

            // 相対パスから絶対パスに変換
            var currentDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(tempRoot);
                var relativePath = Path.Combine("subfolder", "nonexistent");

                // Act
                if (!TryInvokeFindValidAncestorPath(relativePath, out var result))
                {
                    return;
                }

                // Assert
                Assert.NotNull(result);
                Assert.Equal(Path.GetFullPath(subfolder), result);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
        }
    }

    [Fact]
    public void FindValidAncestorPathHandlesDeepNesting()
    {
        // Arrange
        var tempRoot = CreateTempDirectory();
        try
        {
            var level1 = Path.Combine(tempRoot, "level1");
            var level2 = Path.Combine(level1, "level2");
            var level3 = Path.Combine(level2, "level3");
            var level4 = Path.Combine(level3, "level4");
            var level5 = Path.Combine(level4, "level5");

            Directory.CreateDirectory(level1);
            // level2 以降は作成しない

            // Act
            if (!TryInvokeFindValidAncestorPath(level5, out var result))
            {
                return;
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Path.GetFullPath(level1), result);
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
        }
    }

    private static bool TryInvokeFindValidAncestorPath(string? path, out string? result)
    {
        result = null;
        var method = FindValidAncestorPathMethod;
        if (method is null)
        {
            return false;
        }

        result = method.Invoke(null, new object?[] { path }) as string;
        return true;
    }

    private static MethodInfo? ResolveFindValidAncestorPathMethod()
    {
        // MainWindow クラスの FindValidAncestorPath メソッドをリフレクションで呼び出す
        var mainWindowType = Type.GetType("PhotoGeoExplorer.MainWindow, PhotoGeoExplorer")
            ?? LoadMainWindowTypeFromLocalAssembly();
        if (mainWindowType is null)
        {
            // CI では PhotoGeoExplorer の参照が外れるため、テスト対象が解決できない場合は null を返す。
            return null;
        }

        var method = mainWindowType.GetMethod(
            "FindValidAncestorPath",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        return method;
    }

    private static Type? LoadMainWindowTypeFromLocalAssembly()
    {
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, "PhotoGeoExplorer.dll");
        if (!File.Exists(assemblyPath))
        {
            return null;
        }

        var assembly = Assembly.LoadFrom(assemblyPath);
        return assembly.GetType("PhotoGeoExplorer.MainWindow");
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
            // Intentionally empty - cleanup should not fail tests
        }
        catch (UnauthorizedAccessException)
        {
            // Intentionally empty - cleanup should not fail tests
        }
    }
}
