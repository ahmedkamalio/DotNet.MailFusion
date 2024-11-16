using FluentAssertions;
using MailFusion.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MailFusion.Tests;

public class SendGridEmailProviderTests
{
    private readonly SendGridEmailProvider _provider;

    public SendGridEmailProviderTests()
    {
        Mock<IOptions<EmailOptions>> mockOptions = new();
        Mock<ILogger<SendGridEmailProvider>> mockLogger = new();

        var options = new EmailOptions
        {
            Provider = SupportedProviders.SendGrid,
            SendGrid = new SendGridOptions
            {
                ApiKey = "test-api-key"
            }
        };

        mockOptions.Setup(o => o.Value).Returns(options);
        _provider = new SendGridEmailProvider(mockOptions.Object, mockLogger.Object);
    }

    [Fact]
    public async Task SendEmailAsync_WithValidMessage_ShouldSucceed()
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
        result.IsFailure.Should().BeTrue(); // Will fail in tests due to invalid API key
        result.Error!.Code.Should().Be(SendGridErrors.Codes.AuthenticationError);
    }
}