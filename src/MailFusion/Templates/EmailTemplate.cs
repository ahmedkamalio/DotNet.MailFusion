namespace MailFusion.Templates;

/// <summary>
/// Represents a complete email template containing subject, HTML content, and plain text content.
/// This class implements IEmailTemplate to provide a standard structure for processed email templates
/// ready for message composition.
/// </summary>
/// <remarks>
/// <para>
/// EmailTemplate is an immutable record that encapsulates all necessary components of an email template:
/// <list type="bullet">
///   <item><description>Subject - The email subject line</description></item>
///   <item><description>HTML body - Rich formatted content</description></item>
///   <item><description>Plain text body - Text-only alternative content</description></item>
/// </list>
/// </para>
/// <para>
/// The class uses init-only properties to ensure immutability after creation, making it
/// thread-safe and suitable for caching. All properties are marked as required to ensure
/// complete template information is provided during initialization.
/// </para>
/// <para>
/// Common usage patterns include:
/// <list type="bullet">
///   <item><description>Direct instantiation using object initialization syntax</description></item>
///   <item><description>Factory creation using the static Create method</description></item>
///   <item><description>Output from template engine processing</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Using object initialization syntax:
/// <code>
/// var template = new EmailTemplate
/// {
///     Subject = "Welcome to Our Service",
///     HtmlBody = "&lt;h1&gt;Welcome!&lt;/h1&gt;&lt;p&gt;Thank you for joining.&lt;/p&gt;",
///     PlainTextBody = "Welcome!\n\nThank you for joining."
/// };
/// </code>
/// 
/// Using the static factory method:
/// <code>
/// var template = EmailTemplate.Create(
///     "Welcome to Our Service",
///     "&lt;h1&gt;Welcome!&lt;/h1&gt;&lt;p&gt;Thank you for joining.&lt;/p&gt;",
///     "Welcome!\n\nThank you for joining."
/// );
/// </code>
/// </example>
public record EmailTemplate : IEmailTemplate
{
    /// <summary>
    /// Gets the subject line of the email message.
    /// </summary>
    /// <remarks>
    /// The subject line should be concise and descriptive, typically not exceeding
    /// 78 characters. It should contain only plain text without HTML formatting.
    /// </remarks>
    public required string Subject { get; init; }

    /// <summary>
    /// Gets the HTML-formatted body content of the email message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The HTML body should follow email HTML best practices:
    /// <list type="bullet">
    ///   <item><description>Use table-based layouts for compatibility</description></item>
    ///   <item><description>Include inline CSS styles</description></item>
    ///   <item><description>Avoid JavaScript and complex CSS</description></item>
    ///   <item><description>Test across multiple email clients</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string HtmlBody { get; init; }

    /// <summary>
    /// Gets the plain text version of the email body content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The plain text body serves several purposes:
    /// <list type="bullet">
    ///   <item><description>Provides accessibility for screen readers</description></item>
    ///   <item><description>Supports text-only email clients</description></item>
    ///   <item><description>Improves deliverability and spam scoring</description></item>
    ///   <item><description>Offers a fallback when HTML cannot be displayed</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string PlainTextBody { get; init; }

    /// <summary>
    /// Creates a new instance of EmailTemplate with the specified content.
    /// </summary>
    /// <param name="subject">The email subject line.</param>
    /// <param name="htmlBody">The HTML-formatted email body content.</param>
    /// <param name="plainTextBody">The plain text version of the email body content.</param>
    /// <returns>A new EmailTemplate instance initialized with the provided content.</returns>
    /// <remarks>
    /// This factory method provides a convenient way to create email templates when you have
    /// all content components available. It uses the same validation and structure as direct
    /// initialization but offers a more concise syntax.
    /// </remarks>
    /// <example>
    /// <code>
    /// var template = EmailTemplate.Create(
    ///     "Monthly Newsletter",
    ///     "&lt;h1&gt;Newsletter&lt;/h1&gt;&lt;p&gt;This month's updates...&lt;/p&gt;",
    ///     "Newsletter\n\nThis month's updates..."
    /// );
    /// </code>
    /// </example>
    public static EmailTemplate Create(string subject, string htmlBody, string plainTextBody)
        => new()
        {
            Subject = subject,
            HtmlBody = htmlBody,
            PlainTextBody = plainTextBody
        };
}