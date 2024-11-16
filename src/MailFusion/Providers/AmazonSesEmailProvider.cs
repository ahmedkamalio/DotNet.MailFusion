using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResultObject;

namespace MailFusion.Providers;

/// <summary>
/// Implements email delivery through Amazon Simple Email Service (SES). This provider handles 
/// authentication, message transformation, and communication with the AWS SES API for sending emails.
/// </summary>
/// <remarks>
/// <para>
/// The provider offers several key features:
/// <list type="bullet">
///   <item><description>AWS SES API integration with comprehensive error handling</description></item>
///   <item><description>Support for both HTML and plain text email content</description></item>
///   <item><description>Detailed error reporting for AWS-specific issues</description></item>
///   <item><description>Logging of all email operations and failures</description></item>
/// </list>
/// </para>
/// <para>
/// Common error scenarios handled include:
/// <list type="bullet">
///   <item><description>Account sending paused states</description></item>
///   <item><description>Configuration and domain verification issues</description></item>
///   <item><description>Message rejection scenarios</description></item>
///   <item><description>AWS API communication failures</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "Email": {
///     "Provider": "AmazonSes",
///     "AmazonSes": {
///       "AccessKey": "your-access-key",
///       "SecretKey": "your-secret-key",
///       "Region": "us-east-1"
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
public class AmazonSesEmailProvider : IEmailProvider
{
    /// <summary>
    /// The Amazon SES client instance used for API communication.
    /// </summary>
    /// <remarks>
    /// Initialized in the constructor with the configured AWS credentials and region.
    /// This client handles all communication with the AWS SES API.
    /// </remarks>
    private readonly AmazonSimpleEmailServiceClient _client;

    /// <summary>
    /// The logger instance for recording operations and errors.
    /// </summary>
    private readonly ILogger<AmazonSesEmailProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the AmazonSesEmailProvider class.
    /// </summary>
    /// <param name="options">The email configuration options containing AWS SES settings.</param>
    /// <param name="logger">The logger instance for recording operations and errors.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when AWS SES configuration options are missing or incomplete.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The constructor performs several initialization steps:
    /// <list type="bullet">
    ///   <item><description>Validates the AWS SES configuration</description></item>
    ///   <item><description>Initializes the AWS SES client</description></item>
    ///   <item><description>Sets up logging and error handling</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The AWS SES client is configured with:
    /// <list type="bullet">
    ///   <item><description>AWS access credentials</description></item>
    ///   <item><description>Specified AWS region</description></item>
    ///   <item><description>Basic AWS client configuration</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public AmazonSesEmailProvider(IOptions<EmailOptions> options, ILogger<AmazonSesEmailProvider> logger)
    {
        _logger = logger;

        var amazonSesOptions = options.Value.AmazonSes;
        if (amazonSesOptions is null)
        {
            throw new InvalidOperationException("AWS SES options are not configured");
        }

        _client = new AmazonSimpleEmailServiceClient(
            new Amazon.Runtime.BasicAWSCredentials(amazonSesOptions.AccessKey, amazonSesOptions.SecretKey),
            RegionEndpoint.GetBySystemName(amazonSesOptions.Region)
        );
    }

    /// <summary>
    /// Sends an email message through the Amazon SES API.
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
    ///   <item><description>Transforms the EmailMessage to AWS SES format</description></item>
    ///   <item><description>Sets up email headers including From and Reply-To</description></item>
    ///   <item><description>Handles both HTML and plain text content</description></item>
    ///   <item><description>Processes multiple recipients</description></item>
    ///   <item><description>Executes the API call with comprehensive error handling</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Error handling covers several AWS-specific scenarios:
    /// <list type="bullet">
    ///   <item><description>Account sending paused exceptions</description></item>
    ///   <item><description>Configuration set issues</description></item>
    ///   <item><description>Domain verification failures</description></item>
    ///   <item><description>Message rejection cases</description></item>
    ///   <item><description>Network and connectivity problems</description></item>
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
        var sendRequest = new SendEmailRequest
        {
            Source = FormatEmailAddress(message.Sender.Name, message.Sender.Email),
            Destination = new Destination
            {
                ToAddresses = message.Recipients
                    .Select(r => FormatEmailAddress(r.Name, r.Email))
                    .ToList()
            },
            Message = new Message
            {
                Subject = new Content(message.Subject),
                Body = new Body
                {
                    Html = new Content(message.HtmlBody),
                    Text = message.PlainTextBody is not null
                        ? new Content(message.PlainTextBody)
                        : null
                }
            },
            ReplyToAddresses = [message.Sender.ReplyEmail]
        };

        try
        {
            var response = await _client.SendEmailAsync(sendRequest, cancellationToken);

            _logger.LogInformation("AWS SES email sent successfully. MessageId: {MessageId}",
                response.MessageId);

            return Result.Success();
        }
        catch (AccountSendingPausedException ex)
        {
            _logger.LogError(ex, "Email sending is disabled for the AWS SES account");

            return CreateDetailedError(
                AmazonSesErrors.Codes.AccountPaused,
                AmazonSesErrors.Reasons.AccountPaused,
                AmazonSesErrors.Messages.AccountPaused,
                ex);
        }
        catch (ConfigurationSetDoesNotExistException ex)
        {
            _logger.LogError(ex, "AWS SES configuration set does not exist");

            return CreateDetailedError(
                AmazonSesErrors.Codes.ConfigNotFound,
                AmazonSesErrors.Reasons.ConfigNotFound,
                AmazonSesErrors.Messages.ConfigNotFound,
                ex);
        }
        catch (ConfigurationSetSendingPausedException ex)
        {
            _logger.LogError(ex, "Email sending is disabled for the configuration set");

            return CreateDetailedError(
                AmazonSesErrors.Codes.ConfigPaused,
                AmazonSesErrors.Reasons.ConfigPaused,
                AmazonSesErrors.Messages.ConfigPaused,
                ex);
        }
        catch (MailFromDomainNotVerifiedException ex)
        {
            _logger.LogError(ex, "MAIL FROM domain not verified");

            return CreateDetailedError(
                AmazonSesErrors.Codes.DomainNotVerified,
                AmazonSesErrors.Reasons.DomainNotVerified,
                AmazonSesErrors.Messages.DomainNotVerified,
                ex);
        }
        catch (MessageRejectedException ex)
        {
            _logger.LogError(ex, "Email message rejected by AWS SES");

            return CreateDetailedError(
                AmazonSesErrors.Codes.MessageRejected,
                AmazonSesErrors.Reasons.MessageRejected,
                $"{ex.Message} Request ID: {ex.RequestId}, HTTP Status Code: {ex.StatusCode}",
                ex);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("AWS SES send operation was cancelled");

            return CreateDetailedError(
                AmazonSesErrors.Codes.OperationCancelled,
                AmazonSesErrors.Reasons.OperationCancelled,
                AmazonSesErrors.Messages.OperationCancelled,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while sending email via AWS SES");

            return CreateDetailedError(
                AmazonSesErrors.Codes.UnexpectedError,
                AmazonSesErrors.Reasons.UnexpectedError,
                AmazonSesErrors.Messages.UnexpectedError,
                ex);
        }
    }

    /// <summary>
    /// Creates a detailed error result for AWS SES operations.
    /// </summary>
    /// <param name="code">The error code identifying the type of error.</param>
    /// <param name="reason">The reason describing the error category.</param>
    /// <param name="message">The detailed error message.</param>
    /// <param name="exception">The exception that caused the error.</param>
    /// <returns>A Result containing detailed error information.</returns>
    /// <remarks>
    /// <para>
    /// The method creates comprehensive error information including:
    /// <list type="bullet">
    ///   <item><description>Standard error code and reason</description></item>
    ///   <item><description>Detailed message with context</description></item>
    ///   <item><description>AWS-specific error details when available</description></item>
    ///   <item><description>Request and response information</description></item>
    ///   <item><description>Exception details and stack traces</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private static Result<Unit> CreateDetailedError(
        string code,
        string reason,
        string message,
        Exception exception)
    {
        var detailedMessage = string.Join(
            Environment.NewLine,
            message,
            $"Exception Type: {exception.GetType().FullName}",
            $"Exception Message: {exception.Message}",
            exception is AmazonSimpleEmailServiceException awsEx
                ? $"AWS Request ID: {awsEx.RequestId}, Status Code: {awsEx.StatusCode}"
                : string.Empty
        );

        var innerError = exception is AmazonSimpleEmailServiceException awsException
            ? new ResultError(
                awsException.ErrorCode ?? "UNKNOWN_AWS_ERROR",
                awsException.ErrorType.ToString(),
                awsException.Message)
            : null;

        var error = new ResultError(
            code,
            reason,
            detailedMessage.Trim(),
            ErrorCategory.External,
            innerError
        );

        return Result.Failure<Unit>(error);
    }

    /// <summary>
    /// Formats an email address with an optional display name according to AWS SES requirements.
    /// </summary>
    /// <param name="name">Optional display name for the email address.</param>
    /// <param name="email">The email address to format.</param>
    /// <returns>
    /// A properly formatted email address string. If a name is provided, returns in the format
    /// "Name &lt;email@example.com&gt;". If no name is provided, returns just the email address.
    /// </returns>
    /// <remarks>
    /// This format complies with AWS SES requirements for email address formatting and
    /// ensures proper display in email clients.
    /// </remarks>
    private static string FormatEmailAddress(string? name, string email) =>
        name is not null ? $"{name} <{email}>" : email;
}