namespace MailFusion;

/// <summary>
/// Represents an email message with all its components including subject, body content, sender,
/// and recipients. This is the core data structure used for sending emails through the email service.
/// </summary>
/// <remarks>
/// <para>
/// The message structure supports both HTML and plain text content, allowing for rich formatting
/// while maintaining compatibility with email clients that prefer plain text.
/// </para>
/// <para>
/// All required properties must be initialized when creating a new instance. Optional properties
/// can be omitted and will be handled appropriately by the email provider.
/// </para>
/// </remarks>
/// <example>
/// Creating a new email message:
/// <code>
/// var message = new EmailMessage
/// {
///     Subject = "Welcome to Our Service",
///     HtmlBody = "<h1>Welcome!</h1><p>Thank you for joining.</p>",
///     PlainTextBody = "Welcome!\n\nThank you for joining.",
///     Sender = new EmailSender 
///     {
///         Name = "My Service",
///         Email = "noreply@myservice.com",
///         ReplyEmail = "support@myservice.com"
///     },
///     Recipients = new List&lt;EmailRecipient&gt;
///     {
///         new() { Email = "user@example.com", Name = "John Doe" }
///     }
/// };
/// </code>
/// </example>
public record EmailMessage
{
    /// <summary>
    /// Gets the subject line of the email message.
    /// </summary>
    /// <remarks>
    /// The subject should be concise and descriptive of the email content.
    /// Most email clients have limitations on subject line length, typically around 78-80 characters.
    /// </remarks>
    public required string Subject { get; init; }

    /// <summary>
    /// Gets the HTML-formatted body content of the email message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Should contain valid HTML markup for email clients. Consider following email HTML best practices:
    /// <list type="bullet">
    ///   <item><description>Use table-based layouts for better compatibility</description></item>
    ///   <item><description>Use inline CSS styles</description></item>
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
    /// While optional, providing a plain text version is recommended for:
    /// <list type="bullet">
    ///   <item><description>Better accessibility</description></item>
    ///   <item><description>Improved deliverability</description></item>
    ///   <item><description>Support for text-only email clients</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public string? PlainTextBody { get; init; }

    /// <summary>
    /// Gets information about the sender of the email.
    /// </summary>
    /// <remarks>
    /// The sender information should comply with email service provider requirements
    /// and use verified domains/email addresses where required.
    /// </remarks>
    public required EmailSender Sender { get; init; }

    /// <summary>
    /// Gets the list of email recipients.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The list must contain at least one recipient. Some email providers may have
    /// limitations on the maximum number of recipients per message.
    /// </para>
    /// <para>
    /// For large recipient lists, consider:
    /// <list type="bullet">
    ///   <item><description>Batch sending in smaller groups</description></item>
    ///   <item><description>Using provider-specific bulk sending features</description></item>
    ///   <item><description>Respecting rate limits and quotas</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required IList<EmailRecipient> Recipients { get; init; }
}

/// <summary>
/// Represents the sender information for an email message, including display name
/// and email addresses for sending and replies.
/// </summary>
/// <remarks>
/// When using email service providers like SendGrid or Amazon SES, ensure the sender
/// email addresses are properly verified according to the provider's requirements.
/// </remarks>
public record EmailSender
{
    /// <summary>
    /// Gets the display name of the email sender.
    /// </summary>
    /// <remarks>
    /// This is the friendly name that appears in email clients, like "My Service" in
    /// "My Service &lt;noreply@myservice.com&gt;".
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the email address used as the sender address ("From" address).
    /// </summary>
    /// <remarks>
    /// Must be a valid email address that you are authorized to send from.
    /// Typically needs to be verified with your email service provider.
    /// </remarks>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the email address for recipients to reply to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This can be different from the sender email address. Common uses include:
    /// <list type="bullet">
    ///   <item><description>Using a monitored address for replies to no-reply sender addresses</description></item>
    ///   <item><description>Directing replies to specific departments or team inboxes</description></item>
    ///   <item><description>Tracking responses through dedicated addresses</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string ReplyEmail { get; init; }
}

/// <summary>
/// Represents a recipient of an email message, including their email address
/// and optional display name.
/// </summary>
/// <remarks>
/// Each recipient in an email message must have a valid email address, while
/// the display name is optional but recommended for better user experience.
/// </remarks>
public record EmailRecipient
{
    /// <summary>
    /// Gets the email address of the recipient.
    /// </summary>
    /// <remarks>
    /// Must be a valid email address format. The email service will typically
    /// validate the format before attempting delivery.
    /// </remarks>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the display name of the recipient.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When provided, this name will be displayed in email clients instead of
    /// the raw email address, improving readability and personalization.
    /// </para>
    /// <para>
    /// For example, "John Doe" in "John Doe &lt;john@example.com&gt;".
    /// </para>
    /// </remarks>
    public string? Name { get; init; }
}