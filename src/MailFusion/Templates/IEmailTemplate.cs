namespace MailFusion.Templates;

/// <summary>
/// Defines the contract for a processed email template that contains all content components
/// necessary for sending an email message. This interface represents the final output of
/// template processing and contains both required and optional email content formats.
/// </summary>
/// <remarks>
/// <para>
/// An email template consists of three main components:
/// <list type="bullet">
///   <item>
///     <description>Subject line - Required for all email messages</description>
///   </item>
///   <item>
///     <description>HTML body - Required primary content with rich formatting support</description>
///   </item>
///   <item>
///     <description>Plain text body - Alternative content for text-only email clients</description>
///   </item>
/// </list>
/// </para>
/// <para>
/// This interface is typically used in conjunction with:
/// <list type="bullet">
///   <item><description>IEmailTemplateEngine - For generating template content</description></item>
///   <item><description>IEmailService - For sending processed templates</description></item>
///   <item><description>EmailMessage - For constructing the final email message</description></item>
/// </list>
/// </para>
/// <para>
/// Best practices for implementation:
/// <list type="bullet">
///   <item><description>Ensure HTML content follows email HTML best practices</description></item>
///   <item><description>Provide meaningful plain text alternatives</description></item>
///   <item><description>Keep subject lines concise and descriptive</description></item>
///   <item><description>Consider email client compatibility</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Example implementation:
/// <code>
/// public class EmailTemplate : IEmailTemplate
/// {
///     public required string Subject { get; init; }
///     public required string HtmlBody { get; init; }
///     public required string PlainTextBody { get; init; }
///     
///     public static EmailTemplate Create(string subject, string htmlBody, string plainTextBody)
///         => new()
///         {
///             Subject = subject,
///             HtmlBody = htmlBody,
///             PlainTextBody = plainTextBody
///         };
/// }
/// </code>
/// 
/// Usage example:
/// <code>
/// // Creating an email template
/// var template = EmailTemplate.Create(
///     subject: "Welcome to Our Service",
///     htmlBody: "&lt;h1&gt;Welcome!&lt;/h1&gt;&lt;p&gt;Thank you for joining.&lt;/p&gt;",
///     plainTextBody: "Welcome!\n\nThank you for joining."
/// );
/// 
/// // Using the template
/// var message = new EmailMessage
/// {
///     Subject = template.Subject,
///     HtmlBody = template.HtmlBody,
///     PlainTextBody = template.PlainTextBody,
///     // ... other message properties
/// };
/// </code>
/// </example>
public interface IEmailTemplate
{
    /// <summary>
    /// Gets the subject line of the email message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The subject line should:
    /// <list type="bullet">
    ///   <item><description>Be concise and descriptive</description></item>
    ///   <item><description>Avoid excessive punctuation or special characters</description></item>
    ///   <item><description>Typically not exceed 78 characters</description></item>
    ///   <item><description>Contain plain text only (no HTML)</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    string Subject { get; }

    /// <summary>
    /// Gets the HTML-formatted body content of the email message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The HTML body should:
    /// <list type="bullet">
    ///   <item><description>Use email-safe HTML markup</description></item>
    ///   <item><description>Include inline CSS for styling</description></item>
    ///   <item><description>Use table-based layouts for compatibility</description></item>
    ///   <item><description>Avoid JavaScript and complex CSS</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    string HtmlBody { get; }

    /// <summary>
    /// Gets the plain text version of the email body content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The plain text body should:
    /// <list type="bullet">
    ///   <item><description>Provide a readable alternative to HTML content</description></item>
    ///   <item><description>Use clear formatting with line breaks and spacing</description></item>
    ///   <item><description>Maintain the same information hierarchy as HTML</description></item>
    ///   <item><description>Use ASCII characters for broad compatibility</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    string PlainTextBody { get; }
}