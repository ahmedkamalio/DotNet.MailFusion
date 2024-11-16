using ResultObject;

namespace MailFusion;

/// <summary>
/// Defines the interface for email delivery providers that handle the actual sending of email messages.
/// </summary>
/// <remarks>
/// This interface is implemented by specific email provider classes such as:
/// <list type="bullet">
///   <item><description>SendGridEmailProvider for SendGrid delivery</description></item>
///   <item><description>AmazonSESEmailProvider for Amazon SES delivery</description></item>
///   <item><description>DevelopmentEmailProvider for local development usage</description></item>
/// </list>
/// The provider implementation is automatically configured based on the application's email settings.
/// </remarks>
public interface IEmailProvider
{
    /// <summary>
    /// Sends an email message through the configured email delivery service.
    /// </summary>
    /// <param name="message">The email message to send, containing the complete message details including sender, recipients, subject, and body content.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the send operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a Result indicating success or failure.
    /// If the operation fails, the Result will contain detailed error information specific to the provider.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method handles all aspects of message delivery, including:
    /// <list type="bullet">
    ///   <item><description>Authentication with the email service</description></item>
    ///   <item><description>Message formatting according to provider requirements</description></item>
    ///   <item><description>Error handling and reporting</description></item>
    ///   <item><description>Rate limiting and retry logic (if supported by the provider)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Each provider implementation may have specific limitations or requirements. Refer to the concrete
    /// provider's documentation for provider-specific details.
    /// </para>
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is cancelled via the cancellation token.
    /// </exception>
    Task<IResult<Unit>> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}