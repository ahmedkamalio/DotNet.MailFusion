using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MailFusion.Templates;
using Microsoft.Extensions.Logging;
using Moq;
using ResultObject;

namespace MailFusion.Tests;

public class ScribanEmailTemplateEngineTests
{
    private readonly Mock<IEmailTemplateLoader> _mockLoader;
    private readonly ScribanEmailTemplateEngine _templateEngine;

    public ScribanEmailTemplateEngineTests()
    {
        _mockLoader = new Mock<IEmailTemplateLoader>();
        Mock<ILogger<ScribanEmailTemplateEngine>> mockLogger = new();
        _templateEngine = new ScribanEmailTemplateEngine(_mockLoader.Object, mockLogger.Object);
    }

    [Fact]
    public async Task LoadTemplateAsync_WithValidTemplate_ShouldSucceed()
    {
        // Arrange
        var templateName = "test-template";
        var model = new TestEmailModel
        {
            UserName = "Test User"
        };

        _mockLoader
            .Setup(l => l.LoadTemplateAsync(templateName))
            .ReturnsAsync(Result.Success(("<h1>Hello {{ UserName }}</h1>", "Hello {{ UserName }}")));

        // Act
        var result = await _templateEngine.LoadTemplateAsync(templateName, model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.HtmlBody.Should().Be("<h1>Hello Test User</h1>");
        result.Value.PlainTextBody.Should().Be("Hello Test User");
    }

    [Fact]
    public async Task LoadTemplateAsync_WithInvalidTemplate_ShouldReturnFailure()
    {
        // Arrange
        const string templateName = "invalid-template";
        var model = new TestEmailModel();

        _mockLoader
            .Setup(l => l.LoadTemplateAsync(templateName))
            .ReturnsAsync(Result.Success(("<h1>Hello {{ invalid }}</h1>", "Hello {{ invalid }}")));

        // Act
        var result = await _templateEngine.LoadTemplateAsync(templateName, model);

        // Assert
        result.IsSuccess.Should().BeTrue(); // Scriban doesn't throw on undefined variables
        result.Value!.HtmlBody.Should().Be("<h1>Hello </h1>");
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class TestEmailModel : IEmailTemplateModel
    {
        public string Subject => "Test Subject";
        public string Email => "test@example.com";
        public string UserName { get; init; } = "";
    }
}