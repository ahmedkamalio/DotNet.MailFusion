using FluentAssertions;
using MailFusion.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MailFusion.Tests;

public class DevelopmentEmailProviderTests
{
    private readonly Mock<ILogger<DevelopmentEmailProvider>> _mockLogger;
    private readonly DevelopmentEmailProvider _provider;

    public DevelopmentEmailProviderTests()
    {
        Mock<IOptions<EmailOptions>> mockOptions = new();
        _mockLogger = new Mock<ILogger<DevelopmentEmailProvider>>();

        var options = new EmailOptions
        {
            Provider = SupportedProviders.Development,
            Development = new ConsoleEmailOptions
            {
                UseColors = false,
                ShowHtmlBody = true,
                ShowPlainTextBody = true
            }
        };

        mockOptions.Setup(o => o.Value).Returns(options);
        _provider = new DevelopmentEmailProvider(mockOptions.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldAlwaysSucceed()
    {
        // Arrange
        var message = new EmailMessage
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Test Body</p>",
            PlainTextBody = "Test Body",
            Sender = new EmailSender
            {
                Name = "Test Sender",
                Email = "sender@test.com",
                ReplyEmail = "reply@test.com"
            },
            Recipients = new List<EmailRecipient>
            {
                new() { Email = "recipient@test.com", Name = "Test Recipient" }
            }
        };

        // Act
        var result = await _provider.SendEmailAsync(message);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}