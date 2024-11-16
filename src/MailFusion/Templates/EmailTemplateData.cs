namespace MailFusion.Templates;

/// <summary>
/// Represents the raw data structure of an email template, containing the template name and its content components.
/// This class serves as a data transfer object (DTO) for storing and retrieving email template definitions.
/// </summary>
/// <remarks>
/// <para>
/// EmailTemplateData encapsulates four essential components:
/// <list type="bullet">
///   <item><description>Name - Unique identifier for the template</description></item>
///   <item><description>Subject - Template for generating email subject lines</description></item>
///   <item><description>HtmlTemplate - Template for generating HTML email content</description></item>
///   <item><description>TextTemplate - Template for generating plain text email content</description></item>
/// </list>
/// </para>
/// <para>
/// This class is typically used in scenarios such as:
/// <list type="bullet">
///   <item><description>Loading template definitions from storage</description></item>
///   <item><description>Storing template definitions in a database</description></item>
///   <item><description>Transferring template data between systems</description></item>
///   <item><description>Managing template versioning</description></item>
/// </list>
/// </para>
/// <para>
/// All properties are marked as required to ensure complete template information
/// is provided during initialization.
/// </para>
/// </remarks>
/// <example>
/// Creating a new template data instance:
/// <code>
/// var templateData = new EmailTemplateData
/// {
///     Name = "welcome-email",
///     Subject = "Welcome to {{ service_name }}",
///     HtmlTemplate = @"
///         &lt;h1&gt;Welcome to {{ service_name }}&lt;/h1&gt;
///         &lt;p&gt;Hello {{ user_name }},&lt;/p&gt;
///         &lt;p&gt;Thank you for joining our service.&lt;/p&gt;",
///     TextTemplate = @"
///         Welcome to {{ service_name }}
///         
///         Hello {{ user_name }},
///         
///         Thank you for joining our service."
/// };
/// </code>
/// </example>
public class EmailTemplateData
{
    /// <summary>
    /// Gets or sets the unique identifier name for the template.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The template name should:
    /// <list type="bullet">
    ///   <item><description>Be unique within the template storage system</description></item>
    ///   <item><description>Use a consistent naming convention (e.g., kebab-case)</description></item>
    ///   <item><description>Be descriptive of the template's purpose</description></item>
    ///   <item><description>Avoid special characters that might cause issues in file systems or URLs</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the template for generating email subject lines.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The subject template should:
    /// <list type="bullet">
    ///   <item><description>Support variable substitution</description></item>
    ///   <item><description>Remain concise even after variable expansion</description></item>
    ///   <item><description>Not contain HTML markup</description></item>
    ///   <item><description>Consider character limits of email clients</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string Subject { get; init; }

    /// <summary>
    /// Gets or sets the template for generating HTML email content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The HTML template should:
    /// <list type="bullet">
    ///   <item><description>Follow email HTML best practices</description></item>
    ///   <item><description>Include placeholders for dynamic content</description></item>
    ///   <item><description>Use inline CSS for styling</description></item>
    ///   <item><description>Consider email client compatibility</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string HtmlTemplate { get; init; }

    /// <summary>
    /// Gets or sets the template for generating plain text email content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The text template should:
    /// <list type="bullet">
    ///   <item><description>Provide a readable text-only version of the HTML content</description></item>
    ///   <item><description>Use consistent formatting for placeholders</description></item>
    ///   <item><description>Include clear structure through spacing and formatting</description></item>
    ///   <item><description>Maintain the same information hierarchy as the HTML version</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string TextTemplate { get; init; }
}