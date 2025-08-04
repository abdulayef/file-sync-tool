using FileSyncTool.Interfaces;
using FileSyncTool.Services;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace FileSyncTool.Tests.Services;

/// <summary>
/// Unit tests for <see cref="Synchronizer"/> service
/// </summary>
public class SynchronizerTests
{
    /// <summary>
    /// Verifies the constructor properly initializes with valid parameters
    /// </summary>
    [Fact]
    public void Constructor_Initializes_Correctly()
    {
        // Using Moq to isolate ILogger dependency
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        Assert.NotNull(sut);
    }

    /// <summary>
    /// Tests if the main sync operation completes without throwing exceptions
    /// </summary>
    [Fact]
    public void Run_Executes_WithoutErrors()
    {
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        // Record.Exception captures any thrown exceptions
        var exception = Record.Exception(() => sut.Run());

        Assert.Null(exception);
    }


    /// <summary>
    /// Ensures the synchronizer logs at least one message during operation
    /// </summary>
    [Fact]
    public void Run_Logs_AtLeastOneMessage()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        // Act
        sut.Run();

        // Assert - Verify logger was called
        mockLogger.Verify(
            x => x.Info(It.IsAny<string>(), It.IsAny<string>()), // Explicitly provide all parameters
            Times.AtLeastOnce
        );
    }

    /// <summary>
    /// Tests that files with different content are NOT recopied unnecessarily
    /// </summary>
    [Fact]
    public void Run_DoesNotCopy_WhenFilesDiffer()
    {
        // Arrange
        var mockFs = new MockFileSystem();
        // Setup test files with different content
        mockFs.AddFile("C:/source/file1.txt", new MockFileData("Content1"));
        mockFs.AddFile("C:/replica/file1.txt", new MockFileData("Content2")); // Different content

        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        // Inject mock filesystem via reflection (since _fileSystem is private)
        var field = typeof(Synchronizer).GetField("_fileSystem",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(sut, mockFs);

        // Act
        sut.Run();

        // Assert - Verify no copy operation was logged
        mockLogger.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("Copied")), It.IsAny<string>()),
            Times.Never
        );
    }

}
