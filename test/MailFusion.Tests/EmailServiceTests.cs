using FluentAssertions;
using MailFusion.Templates;
using Microsoft.Extensions.Logging;
using Moq;
using ResultObject;

namespace MailFusion.Tests;

public class EmailServiceTests
{
    private readonly Mock<IEmailProvider> _mockEmailProvider;
    private readonly Mock<IEmailTemplateEngine> _mockTemplateEngine;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _mockEmailProvider = new Mock<IEmailProvider>();
        _mockTemplateEngine = new Mock<IEmailTemplateEngine>();
        Mock<ILogger<EmailService>> mockLogger = new();
        _emailService = new EmailService(_mockEmailProvider.Object, _mockTemplateEngine.Object, mockLogger.Object);
    }

    [Fact]
    public async Task SendAsync_WithValidMessage_ShouldSucceed()
    {
        // Arrange
        var message = CreateValidEmailMessage();
        _mockEmailProvider
            .Setup(p => p.SendEmailAsync(message, default))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _emailService.SendAsync(message);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockEmailProvider.Verify(p => p.SendEmailAsync(message, default), Times.Once);
    }

    [Fact]
    public async Task SendFromTemplateAsync_WithValidTemplateAndModel_ShouldSucceed()
    {
        // Arrange
        var templateName = "welcome";
        var model = new TestEmailModel();
        var sender = CreateValidSender();
        var recipients = CreateValidRecipients();
        var template = CreateValidTemplate();

        _mockTemplateEngine
            .Setup(e => e.LoadTemplateAsync(templateName, model))
            .ReturnsAsync(Result.Success(template));

        _mockEmailProvider
            .Setup(p => p.SendEmailAsync(It.IsAny<EmailMessage>(), default))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _emailService.SendFromTemplateAsync(templateName, model, sender, recipients);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockTemplateEngine.Verify(e => e.LoadTemplateAsync(templateName, model), Times.Once);
        _mockEmailProvider.Verify(p => p.SendEmailAsync(It.IsAny<EmailMessage>(), default), Times.Once);
    }

    [Fact]
    public async Task SendFromTemplateAsync_WithInvalidTemplateName_ShouldReturnFailure()
    {
        // Arrange
        var templateName = "";
        var model = new TestEmailModel();
        var sender = CreateValidSender();
        var recipients = CreateValidRecipients();

        // Act
        var result = await _emailService.SendFromTemplateAsync(templateName, model, sender, recipients);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(EmailServiceErrors.Codes.InvalidInput);
    }

    [Fact]
    public async Task SendFromTemplateAsync_WithEmptyRecipients_ShouldReturnFailure()
    {
        // Arrange
        var templateName = "welcome";
        var model = new TestEmailModel();
        var sender = CreateValidSender();
        var recipients = new List<EmailRecipient>();

        // Act
        var result = await _emailService.SendFromTemplateAsync(templateName, model, sender, recipients);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(EmailServiceErrors.Codes.InvalidInput);
    }

    private static EmailMessage CreateValidEmailMessage() =>
        new()
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Test Body</p>",
            PlainTextBody = "Test Body",
            Sender = CreateValidSender(),
            Recipients = CreateValidRecipients()
        };

    private static EmailSender CreateValidSender() =>
        new()
        {
            Name = "Test Sender",
            Email = "sender@test.com",
            ReplyEmail = "reply@test.com"
        };

    private static List<EmailRecipient> CreateValidRecipients() =>
    [
        new("recipient@test.com", "Test Recipient")
    ];

    private static EmailTemplate CreateValidTemplate() =>
        new()
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Test Body</p>",
            PlainTextBody = "Test Body"
        };

    private class TestEmailModel : IEmailTemplateModel
    {
        public string Subject => "Test Subject";
        public string Email => "test@example.com";
    }
}