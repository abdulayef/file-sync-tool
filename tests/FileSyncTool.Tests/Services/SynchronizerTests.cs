using FileSyncTool.Interfaces;
using FileSyncTool.Services;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace FileSyncTool.Tests.Services;

public class SynchronizerTests
{
    // Test 1: Verify constructor works
    [Fact]
    public void Constructor_Initializes_Correctly()
    {
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        Assert.NotNull(sut);
    }

    // Test 2: Verify basic sync operation
    [Fact]
    public void Run_Executes_WithoutErrors()
    {
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        var exception = Record.Exception(() => sut.Run());

        Assert.Null(exception);
    }

    // Test 3: Verify logging happens
    [Fact]
    public void Run_Logs_AtLeastOneMessage()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        // Act
        sut.Run();

        // Assert (fixed version)
        mockLogger.Verify(
            x => x.Info(It.IsAny<string>(), It.IsAny<string>()), // Explicitly provide all parameters
            Times.AtLeastOnce
        );
    }


    [Fact]
    public void Run_DoesNotCopy_WhenFilesDiffer()
    {
        // Arrange
        var mockFs = new MockFileSystem();
        mockFs.AddFile("C:/source/file1.txt", new MockFileData("Content1"));
        mockFs.AddFile("C:/replica/file1.txt", new MockFileData("Content2")); // Different content

        var mockLogger = new Mock<ILogger>();
        var sut = new Synchronizer("C:/source", "C:/replica", mockLogger.Object);

        var field = typeof(Synchronizer).GetField("_fileSystem",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(sut, mockFs);

        // Act
        sut.Run();

        // Assert - Verify no "Copied" log was generated
        mockLogger.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("Copied")), It.IsAny<string>()),
            Times.Never
        );
    }

}
