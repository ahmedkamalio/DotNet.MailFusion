namespace MailFusion.Templates;

/// <summary>
/// Represents configuration options for the email template system. This class provides settings
/// that control how email templates are loaded and processed within the application.
/// </summary>
/// <remarks>
/// <para>
/// These options are typically configured in the application's configuration system
/// (e.g., appsettings.json) under the "EmailTemplates" section.
/// </para>
/// <para>
/// The options support different template providers and their specific configurations:
/// <list type="bullet">
///   <item><description>File-based template storage</description></item>
///   <item><description>Database template storage</description></item>
///   <item><description>Remote template services</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "EmailTemplates": {
///     "Provider": "File",
///     "File": {
///       "TemplatesPath": "Templates/Email"
///     }
///   }
/// }
/// </code>
/// 
/// Registration in Startup.cs:
/// <code>
/// services.Configure&lt;EmailTemplateOptions&gt;(
///     configuration.GetSection(EmailTemplateOptions.SectionName)
/// );
/// </code>
/// </example>
public class EmailTemplateOptions
{
    /// <summary>
    /// The configuration section name where email template settings should be defined
    /// in application configuration.
    /// </summary>
    /// <remarks>
    /// This constant is used to bind the configuration section to the options class
    /// during application startup.
    /// </remarks>
    public const string SectionName = "EmailTemplates";

    /// <summary>
    /// Gets or sets the template provider to use for loading templates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Common provider values might include:
    /// <list type="bullet">
    ///   <item><description>"File" - Load templates from the file system</description></item>
    ///   <item><description>"Database" - Load templates from a database</description></item>
    ///   <item><description>"Remote" - Load templates from a remote service</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public required string Provider { get; set; }

    /// <summary>
    /// Gets or sets the configuration options specific to file-based template storage.
    /// Only required when using the file system as the template provider.
    /// </summary>
    /// <remarks>
    /// These options are only used when Provider is set to "File". For other providers,
    /// this property will be null.
    /// </remarks>
    public FileTemplateOptions? File { get; set; }
}

/// <summary>
/// Represents configuration options specific to file-based template storage.
/// This class provides settings for loading email templates from the file system.
/// </summary>
/// <remarks>
/// <para>
/// File-based template storage expects templates to be organized in a specific directory
/// structure with separate files for HTML and text versions of each template.
/// </para>
/// <para>
/// Best practices for template file organization:
/// <list type="bullet">
///   <item><description>Use consistent file naming conventions</description></item>
///   <item><description>Maintain HTML and text versions with the same base name</description></item>
///   <item><description>Organize templates into subdirectories by category if needed</description></item>
///   <item><description>Use appropriate file extensions (.html, .txt)</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Example directory structure:
/// <code>
/// Templates/
///   ├── Email/
///   │   ├── welcome/
///   │   │   ├── welcome.html
///   │   │   └── welcome.txt
///   │   ├── order-confirmation/
///   │   │   ├── order-confirmation.html
///   │   │   └── order-confirmation.txt
/// </code>
/// 
/// Configuration in appsettings.json:
/// <code>
/// {
///   "EmailTemplates": {
///     "Provider": "File",
///     "File": {
///       "TemplatesPath": "Templates/Email"
///     }
///   }
/// }
/// </code>
/// </example>
public class FileTemplateOptions
{
    /// <summary>
    /// Gets or sets the file system path where email templates are stored.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The templates path should:
    /// <list type="bullet">
    ///   <item><description>Be accessible to the application process</description></item>
    ///   <item><description>Have appropriate read permissions</description></item>
    ///   <item><description>Be a valid absolute or relative path</description></item>
    ///   <item><description>Follow the expected template directory structure</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// For security reasons, the application should validate that template access
    /// is restricted to the specified directory tree.
    /// </para>
    /// </remarks>
    public required string TemplatesPath { get; init; }
}