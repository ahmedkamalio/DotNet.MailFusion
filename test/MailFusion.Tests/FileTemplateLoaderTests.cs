using FluentAssertions;
using MailFusion.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MailFusion.Tests;

public class FileTemplateLoaderTests : IDisposable
{
    private readonly Mock<IOptions<EmailTemplateOptions>> _mockOptions;
    private readonly Mock<ILogger<FileTemplateLoader>> _mockLogger;
    private readonly string _tempPath;

    public FileTemplateLoaderTests()
    {
        _mockOptions = new Mock<IOptions<EmailTemplateOptions>>();
        _mockLogger = new Mock<ILogger<FileTemplateLoader>>();
        _tempPath = Path.Combine(Path.GetTempPath(), "MailFusionTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempPath);

        var options = new EmailTemplateOptions
        {
            Provider = "File",
            File = new FileTemplateOptions
            {
                TemplatesPath = _tempPath
            }
        };

        _mockOptions.Setup(o => o.Value).Returns(options);
    }

    [Fact]
    public async Task LoadTemplateAsync_WithValidTemplate_ShouldSucceed()
    {
        // Arrange
        const string templateName = "test-template";
        const string htmlContent = "<h1>Test Template</h1>";
        const string textContent = "Test Template";

        CreateTestTemplate(templateName, htmlContent, textContent);

        var loader = new FileTemplateLoader(_mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await loader.LoadTemplateAsync(templateName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.html.Should().Be(htmlContent);
        result.Value.text.Should().Be(textContent);
    }

    [Fact]
    public async Task LoadTemplateAsync_WithMissingTemplate_ShouldReturnFailure()
    {
        // Arrange
        const string templateName = "missing-template";
        var loader = new FileTemplateLoader(_mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await loader.LoadTemplateAsync(templateName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(TemplateErrors.Codes.HtmlTemplateNotFound);
    }

    private void CreateTestTemplate(string templateName, string htmlContent, string textContent)
    {
        File.WriteAllText(Path.Combine(_tempPath, $"{templateName}.html"), htmlContent);
        File.WriteAllText(Path.Combine(_tempPath, $"{templateName}.txt"), textContent);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempPath, true);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}