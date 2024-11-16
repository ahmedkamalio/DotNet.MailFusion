using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResultObject;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MailFusion.Providers;

/// <summary>
/// Implements email delivery through the SendGrid service. This provider handles authentication,
/// message transformation, and communication with the SendGrid API for sending emails.
/// </summary>
/// <remarks>
/// <para>
/// The provider offers several key features:
/// <list type="bullet">
///   <item><description>SendGrid API integration with automatic error handling</description></item>
///   <item><description>Support for HTML and plain text email content</description></item>
///   <item><description>Detailed error reporting for common SendGrid issues</description></item>
///   <item><description>Logging of all email operations and failures</description></item>
/// </list>
/// </para>
/// <para>
/// Common error scenarios handled include:
/// <list type="bullet">
///   <item><description>Authentication failures</description></item>
///   <item><description>Rate limiting</description></item>
///   <item><description>Message validation errors</description></item>
///   <item><description>API communication issues</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "Email": {
///     "Provider": "SendGrid",
///     "SendGrid": {
///       "ApiKey": "your-api-key-here"
///     }
///   }
/// }
/// </code>
/// 
/// Registration in Startup.cs:
/// <code>
/// services.AddConfiguredEmailService(configuration, environment);
/// </code>
/// </example>
public class SendGridEmailProvider : IEmailProvider
{
    /// <summary>
    /// The SendGrid client instance used for API communication.
    /// </summary>
    /// <remarks>
    /// Initialized in the constructor with the configured API key.
    /// This client handles all HTTP communication with SendGrid's API.
    /// </remarks>
    private readonly SendGridClient _client;

    /// <summary>
    /// The logger instance for recording operations and errors.
    /// </summary>
    private readonly ILogger<SendGridEmailProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the SendGridEmailProvider class.
    /// </summary>
    /// <param name="options">The email configuration options containing SendGrid settings.</param>
    /// <param name="logger">The logger instance for recording operations and errors.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the SendGrid API key is not configured in the application settings.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The constructor performs several initialization steps:
    /// <list type="bullet">
    ///   <item><description>Validates the SendGrid configuration</description></item>
    ///   <item><description>Initializes the SendGrid client</description></item>
    ///   <item><description>Sets up logging and error handling</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public SendGridEmailProvider(IOptions<EmailOptions> options, ILogger<SendGridEmailProvider> logger)
    {
        _logger = logger;

        var sendGridOptions = options.Value.SendGrid;
        if (sendGridOptions?.ApiKey is null)
        {
            throw new InvalidOperationException("SendGrid API key is not configured");
        }

        _client = new SendGridClient(sendGridOptions.ApiKey);
    }

    /// <summary>
    /// Sends an email message through the SendGrid API.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>
    /// A Result indicating success or failure of the send operation. If the operation fails,
    /// the result contains detailed error information specific to the failure type.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method performs several steps:
    /// <list type="bullet">
    ///   <item><description>Transforms the EmailMessage to SendGrid's format</description></item>
    ///   <item><description>Sets up proper email headers including From and Reply-To</description></item>
    ///   <item><description>Handles both HTML and plain text content</description></item>
    ///   <item><description>Processes multiple recipients</description></item>
    ///   <item><description>Executes the API call with error handling</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Error handling covers several scenarios:
    /// <list type="bullet">
    ///   <item><description>HTTP response status codes (400, 401, 429, etc.)</description></item>
    ///   <item><description>Rate limiting responses</description></item>
    ///   <item><description>API errors and validation failures</description></item>
    ///   <item><description>Network and connectivity issues</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Using the provider to send an email:
    /// <code>
    /// var message = new EmailMessage
    /// {
    ///     Subject = "Welcome",
    ///     HtmlBody = "&lt;h1&gt;Welcome!&lt;/h1&gt;",
    ///     Sender = new EmailSender 
    ///     { 
    ///         Name = "Service",
    ///         Email = "service@example.com",
    ///         ReplyEmail = "support@example.com"
    ///     },
    ///     Recipients = new[] 
    ///     { 
    ///         new EmailRecipient 
    ///         { 
    ///             Email = "user@example.com",
    ///             Name = "John Doe"
    ///         } 
    ///     }
    /// };
    /// 
    /// var result = await provider.SendEmailAsync(message);
    /// </code>
    /// </example>
    public async Task<IResult<Unit>> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var from = new EmailAddress(message.Sender.Email, message.Sender.Name);
        var replyTo = new EmailAddress(message.Sender.ReplyEmail);

        var msg = new SendGridMessage
        {
            From = from,
            ReplyTo = replyTo,
            Subject = message.Subject,
            HtmlContent = message.HtmlBody,
            PlainTextContent = message.PlainTextBody
        };

        foreach (var recipient in message.Recipients)
        {
            msg.AddTo(new EmailAddress(recipient.Email, recipient.Name));
        }

        try
        {
            var response = await _client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SendGrid email sent successfully to {RecipientCount} recipients",
                    message.Recipients.Count);

                return Result.Success();
            }

            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);

            // Map specific HTTP status codes to appropriate error types
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.TooManyRequests =>
                    CreateDetailedError(
                        SendGridErrors.Codes.RateLimitExceeded,
                        SendGridErrors.Reasons.RateLimitExceeded,
                        SendGridErrors.Messages.RateLimitExceeded,
                        response,
                        responseBody),

                System.Net.HttpStatusCode.Unauthorized =>
                    CreateDetailedError(
                        SendGridErrors.Codes.AuthenticationError,
                        SendGridErrors.Reasons.AuthenticationError,
                        SendGridErrors.Messages.AuthenticationError,
                        response,
                        responseBody),

                System.Net.HttpStatusCode.BadRequest =>
                    CreateDetailedError(
                        SendGridErrors.Codes.ValidationError,
                        SendGridErrors.Reasons.ValidationError,
                        SendGridErrors.Messages.ValidationError,
                        response,
                        responseBody),

                _ => CreateDetailedError(
                    SendGridErrors.Codes.ApiError,
                    SendGridErrors.Reasons.ApiError,
                    SendGridErrors.Messages.ApiError,
                    response,
                    responseBody)
            };
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("SendGrid send operation was cancelled");

            return CreateDetailedError(
                SendGridErrors.Codes.OperationCancelled,
                SendGridErrors.Reasons.OperationCancelled,
                SendGridErrors.Messages.OperationCancelled,
                exception: ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SendGrid");

            return CreateDetailedError(
                SendGridErrors.Codes.UnexpectedError,
                SendGridErrors.Reasons.UnexpectedError,
                SendGridErrors.Messages.UnexpectedError,
                exception: ex);
        }
    }

    /// <summary>
    /// Creates a detailed error result for SendGrid operations.
    /// </summary>
    /// <param name="code">The error code identifying the type of error.</param>
    /// <param name="reason">The reason describing the error category.</param>
    /// <param name="message">The detailed error message.</param>
    /// <param name="response">Optional SendGrid API response for additional error details.</param>
    /// <param name="responseBody">Optional response body content for error details.</param>
    /// <param name="exception">Optional exception that caused the error.</param>
    /// <returns>A Result containing detailed error information.</returns>
    /// <remarks>
    /// <para>
    /// The method creates comprehensive error information including:
    /// <list type="bullet">
    ///   <item><description>Standard error code and reason</description></item>
    ///   <item><description>Detailed message with context</description></item>
    ///   <item><description>API response information when available</description></item>
    ///   <item><description>Rate limit information when relevant</description></item>
    ///   <item><description>Exception details when applicable</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private static Result<Unit> CreateDetailedError(
        string code,
        string reason,
        string message,
        Response? response = null,
        string? responseBody = null,
        Exception? exception = null)
    {
        var detailedMessage = new List<string>
        {
            message
        };

        if (response != null)
        {
            detailedMessage.Add($"Status Code: {(int)response.StatusCode} ({response.StatusCode})");

            if (!string.IsNullOrEmpty(responseBody))
            {
                detailedMessage.Add($"Response: {responseBody}");
            }

            foreach (var header in response.Headers)
            {
                // Include relevant headers like X-RateLimit-* for rate limiting info
                if (header.Key.StartsWith("X-RateLimit-", StringComparison.OrdinalIgnoreCase))
                {
                    detailedMessage.Add($"{header.Key}: {string.Join(", ", header.Value)}");
                }
            }
        }

        if (exception != null)
        {
            detailedMessage.Add($"Exception Type: {exception.GetType().FullName}");
            detailedMessage.Add($"Exception Message: {exception.Message}");
        }

        // Determine the error category based on the error code
        var category = code switch
        {
            SendGridErrors.Codes.ValidationError => ErrorCategory.Validation,
            SendGridErrors.Codes.AuthenticationError => ErrorCategory.Unauthorized,
            SendGridErrors.Codes.RateLimitExceeded => ErrorCategory.External,
            _ => ErrorCategory.External
        };

        var innerError = response is not null
            ? new ResultError(
                $"HTTP_{(int)response.StatusCode}",
                response.StatusCode.ToString(),
                responseBody ?? "No response body available")
            : null;

        var error = new ResultError(
            code,
            reason,
            string.Join(Environment.NewLine, detailedMessage),
            category,
            innerError
        );

        return Result.Failure<Unit>(error);
    }
}