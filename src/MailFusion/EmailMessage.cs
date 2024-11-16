using System.Diagnostics.CodeAnalysis;

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
///     HtmlBody = "&lt;h1&gt;Welcome!&lt;/h1&gt;&lt;p&gt;Thank you for joining.&lt;/p&gt;",
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

    /// <summary>
    /// Initializes a new instance of the EmailSender record with default values.
    /// </summary>
    /// <remarks>
    /// This parameterless constructor is provided to support object initialization syntax.
    /// When using this constructor, all required properties must be set through property initialization.
    /// </remarks>
    /// <example>
    /// Using the parameterless constructor with object initialization:
    /// <code>
    /// var sender = new EmailSender
    /// {
    ///     Name = "My Service",
    ///     Email = "noreply@myservice.com",
    ///     ReplyEmail = "support@myservice.com"
    /// };
    /// </code>
    /// </example>
    public EmailSender()
    {
    }

    /// <summary>
    /// Initializes a new instance of the EmailSender record with the specified sender information.
    /// </summary>
    /// <param name="email">The email address used as the sender address ("From" address).</param>
    /// <param name="name">The display name of the email sender.</param>
    /// <param name="replyEmail">Optional. The email address for recipients to reply to. If not specified, the sender email will be used.</param>
    /// <remarks>
    /// <para>
    /// This constructor provides a convenient way to create an EmailSender instance
    /// with the sender's information. The parameter order prioritizes the essential
    /// email address, while making the reply-to address optional.
    /// </para>
    /// <para>
    /// Usage considerations:
    /// <list type="bullet">
    ///   <item><description>The email must be a valid sender address verified with your email provider</description></item>
    ///   <item><description>The name parameter should be a human-readable display name</description></item>
    ///   <item><description>If replyEmail is not specified, the main email address will be used for replies</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Creating senders using the constructor:
    /// <code>
    /// // Basic usage with same reply address
    /// var sender1 = new EmailSender("noreply@myservice.com", "My Service");
    /// 
    /// // With separate reply-to address
    /// var sender2 = new EmailSender("noreply@myservice.com", "My Service", "support@myservice.com");
    /// 
    /// // With named parameters
    /// var sender3 = new EmailSender(
    ///     email: "noreply@myservice.com",
    ///     name: "My Service",
    ///     replyEmail: "support@myservice.com"
    /// );
    /// </code>
    /// </example>
    [SetsRequiredMembers]
    public EmailSender(string email, string name, string? replyEmail = null)
        => (Email, Name, ReplyEmail) = (email, name, replyEmail ?? email);
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

    /// <summary>
    /// Initializes a new instance of the EmailRecipient record with default values.
    /// </summary>
    /// <remarks>
    /// This parameterless constructor is provided to support object initialization syntax.
    /// When using this constructor, the required Email property must be set through property initialization.
    /// The Name property is optional and can be omitted.
    /// </remarks>
    /// <example>
    /// Using the parameterless constructor with object initialization:
    /// <code>
    /// var recipient = new EmailRecipient
    /// {
    ///     Email = "user@example.com",
    ///     Name = "John Doe"  // Optional
    /// };
    /// </code>
    /// </example>
    public EmailRecipient()
    {
    }

    /// <summary>
    /// Initializes a new instance of the EmailRecipient record with the specified email address and optional display name.
    /// </summary>
    /// <param name="email">The email address of the recipient.</param>
    /// <param name="name">Optional. The display name of the recipient.</param>
    /// <remarks>
    /// <para>
    /// This constructor provides a convenient way to create an EmailRecipient instance
    /// with required email and optional display name information. The parameter order
    /// and optional name parameter make it natural to create recipients with just
    /// an email address.
    /// </para>
    /// <para>
    /// Important considerations:
    /// <list type="bullet">
    ///   <item><description>The email parameter must be a valid email address format</description></item>
    ///   <item><description>The name parameter defaults to null if not provided</description></item>
    ///   <item><description>Email service providers will validate the email address format</description></item>
    ///   <item><description>When name is provided, it improves recipient identification in email clients</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Creating recipients using the constructor:
    /// <code>
    /// // With email only
    /// var recipient1 = new EmailRecipient("user@example.com");
    /// 
    /// // With email and display name
    /// var recipient2 = new EmailRecipient("user@example.com", "John Doe");
    /// 
    /// // With named parameters
    /// var recipient3 = new EmailRecipient(
    ///     email: "user@example.com",
    ///     name: "John Doe"
    /// );
    /// </code>
    /// </example>
    [SetsRequiredMembers]
    public EmailRecipient(string email, string? name = null) => (Email, Name) = (email, name);
}