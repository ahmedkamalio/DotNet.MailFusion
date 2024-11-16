namespace MailFusion.Templates;

/// <summary>
/// Defines the base contract for email template models that provide data for email template rendering.
/// Models implementing this interface can be used with the email template engine to generate
/// both HTML and plain text versions of email content.
/// </summary>
/// <remarks>
/// <para>
/// This interface defines the minimum required properties that all email template models must implement:
/// <list type="bullet">
///   <item><description>Subject - The email subject line</description></item>
///   <item><description>Email - The recipient's email address</description></item>
/// </list>
/// </para>
/// <para>
/// Implementing models can add additional properties specific to their email template needs,
/// which can then be accessed within the template using the property names.
/// </para>
/// </remarks>
/// <example>
/// Example implementation for a welcome email template:
/// <code>
/// public class WelcomeEmailModel : IEmailTemplateModel
/// {
///     public string Subject => "Welcome to Our Service!";
///     public string Email { get; init; }
///     public string UserName { get; init; }
///     public DateTime RegistrationDate { get; init; }
///     public string[] Features { get; init; }
/// }
/// </code>
/// 
/// Usage in a template:
/// <code>
/// Welcome {{ user_name }}!
/// 
/// Thank you for registering on {{ format_date(registration_date) }}.
/// 
/// Here are some features you might like:
/// {{ for feature in features }}
///   - {{ feature }}
/// {{ end }}
/// </code>
/// </example>
public interface IEmailTemplateModel
{
    /// <summary>
    /// Gets the subject line for the email message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The subject line should be:
    /// <list type="bullet">
    ///   <item><description>Concise and descriptive</description></item>
    ///   <item><description>Plain text (no HTML)</description></item>
    ///   <item><description>Limited to a reasonable length (typically under 78 characters)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// While this property is typically implemented as a constant or computed value,
    /// it can also be dynamic based on other model properties.
    /// </para>
    /// </remarks>
    string Subject { get; }

    /// <summary>
    /// Gets the recipient's email address.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property should contain a valid email address that will be used to:
    /// <list type="bullet">
    ///   <item><description>Validate the recipient address</description></item>
    ///   <item><description>Ensure the template has access to the recipient information</description></item>
    ///   <item><description>Support personalization based on the recipient's address</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// For bulk emails or multiple recipients, this should represent the primary/intended
    /// recipient's address. Additional recipients can be handled at the email service level.
    /// </para>
    /// </remarks>
    string Email { get; }
}