using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResultObject;

namespace MailFusion.Providers;

/// <summary>
/// Implements a development-focused email provider that outputs email messages to the console
/// instead of actually sending them. This provider is designed for use in development and testing
/// environments to inspect email content and verify email generation without sending real emails.
/// </summary>
/// <remarks>
/// <para>
/// The development provider offers several features useful for development:
/// <list type="bullet">
///   <item><description>Console output of email details with optional colorization</description></item>
///   <item><description>Configurable display of HTML and plain text content</description></item>
///   <item><description>Visual separation of email components for easy reading</description></item>
///   <item><description>Logging of email operations for monitoring</description></item>
/// </list>
/// </para>
/// <para>
/// Configuration options are controlled through the ConsoleEmailOptions class in application settings:
/// <list type="bullet">
///   <item><description>UseColors - Enables or disables colorized output</description></item>
///   <item><description>ShowHtmlBody - Controls display of HTML content</description></item>
///   <item><description>ShowPlainTextBody - Controls display of plain text content</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "Email": {
///     "Provider": "Development",
///     "Development": {
///       "UseColors": true,
///       "ShowHtmlBody": true,
///       "ShowPlainTextBody": false
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
public class DevelopmentEmailProvider(
    IOptions<EmailOptions> options,
    ILogger<DevelopmentEmailProvider> logger) : IEmailProvider
{
    /// <summary>
    /// Gets the email configuration options including development-specific settings.
    /// </summary>
    /// <remarks>
    /// Initialized from the provided IOptions{EmailOptions} in the constructor.
    /// If no development options are configured, defaults from ConsoleEmailOptions are used.
    /// </remarks>
    private readonly EmailOptions _options = options.Value;

    /// <summary>
    /// Simulates sending an email by writing the message content to the console.
    /// </summary>
    /// <param name="message">The email message to display.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>
    /// A successful result indicating the message was displayed. This provider does not return
    /// failure results as console output operations are considered non-failing.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method formats the output using Unicode box drawing characters to create
    /// a visually distinct display of the email content. The output includes:
    /// <list type="bullet">
    ///   <item><description>A header indicating this is a development email</description></item>
    ///   <item><description>Sender information (From and Reply-To addresses)</description></item>
    ///   <item><description>Recipient list with names and email addresses</description></item>
    ///   <item><description>Email subject</description></item>
    ///   <item><description>HTML body content (if enabled)</description></item>
    ///   <item><description>Plain text body content (if enabled)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// When colors are enabled:
    /// <list type="bullet">
    ///   <item><description>The output is displayed in green for better visibility</description></item>
    ///   <item><description>Colors are reset after the email display</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The method also logs a summary of the operation including the number of recipients
    /// for monitoring and debugging purposes.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example console output:
    /// <code>
    /// ╔════════════════════════════════════════════════════════════════╗
    /// ║                     Development Email                          ║
    /// ╚════════════════════════════════════════════════════════════════╝
    /// From: Service Name &lt;service@example.com&gt;
    /// Reply-To: support@example.com
    /// To: John Doe &lt;john@example.com&gt;
    /// Subject: Welcome to the Service
    /// ────────────────────────────────────────────────────────────────
    /// HTML Body:
    /// &lt;h1&gt;Welcome!&lt;/h1&gt;
    /// &lt;p&gt;Thank you for joining our service.&lt;/p&gt;
    /// ────────────────────────────────────────────────────────────────
    /// </code>
    /// </example>
    public async Task<IResult<Unit>> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var options = _options.Development ?? new ConsoleEmailOptions();

        if (options.UseColors)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     Development Email                          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"From: {message.Sender.Name} <{message.Sender.Email}>");
        Console.WriteLine($"Reply-To: {message.Sender.ReplyEmail}");
        Console.WriteLine("To: " + string.Join(", ", message.Recipients.Select(r => $"{r.Name} <{r.Email}>")));
        Console.WriteLine($"Subject: {message.Subject}");
        Console.WriteLine("────────────────────────────────────────────────────────────────");

        if (options.ShowHtmlBody)
        {
            Console.WriteLine("HTML Body:");
            Console.WriteLine(message.HtmlBody);
            Console.WriteLine("────────────────────────────────────────────────────────────────");
        }

        if (options.ShowPlainTextBody && message.PlainTextBody is not null)
        {
            Console.WriteLine("Plain Text Body:");
            Console.WriteLine(message.PlainTextBody);
            Console.WriteLine("────────────────────────────────────────────────────────────────");
        }

        if (options.UseColors)
        {
            Console.ResetColor();
        }

        logger.LogInformation("Development email sent to {RecipientCount} recipients", message.Recipients.Count);

        return await Task.FromResult(Result.Success());
    }
}