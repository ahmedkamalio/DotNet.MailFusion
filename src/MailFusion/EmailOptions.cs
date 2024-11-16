using System.Diagnostics.CodeAnalysis;

namespace MailFusion;

/// <summary>
/// Represents the configuration options for the email service.
/// This class is typically populated from the application's configuration system
/// and contains settings for the email provider and default sender information.
/// </summary>
/// <remarks>
/// <para>
/// The options are typically configured in appsettings.json under the "Email" section.
/// Only one provider (SendGrid, AmazonSES, or Development) should be configured at a time.
/// </para>
/// <example>
/// Example configuration in appsettings.json:
/// <code>
/// {
///   "Email": {
///     "Provider": "SendGrid",
///     "DefaultFromEmail": "noreply@example.com",
///     "DefaultFromName": "My Application",
///     "SendGrid": {
///       "ApiKey": "your-api-key"
///     }
///   }
/// }
/// </code>
/// </example>
/// </remarks>
public class EmailOptions
{
    /// <summary>
    /// The configuration section name where email settings should be defined in application configuration.
    /// </summary>
    public const string SectionName = "Email";

    /// <summary>
    /// Gets or sets the email service provider to use.
    /// Must be one of the values defined in <see cref="SupportedProviders"/>.
    /// </summary>
    /// <remarks>
    /// Valid values are:
    /// <list type="bullet">
    ///   <item><description>"SendGrid" - Use SendGrid as the email provider</description></item>
    ///   <item><description>"AmazonSES" - Use Amazon Simple Email Service as the provider</description></item>
    ///   <item><description>"Development" - Use the development provider (console output)</description></item>
    /// </list>
    /// </remarks>
    public required string Provider { get; init; }

    /// <summary>
    /// Gets or sets the default "From" email address to use when sending emails.
    /// This can be overridden for individual emails.
    /// </summary>
    public string? DefaultFromEmail { get; init; }

    /// <summary>
    /// Gets or sets the default sender name to use when sending emails.
    /// This can be overridden for individual emails.
    /// </summary>
    public string? DefaultFromName { get; init; }

    /// <summary>
    /// Gets or sets the SendGrid-specific configuration options.
    /// Only required when using SendGrid as the email provider.
    /// </summary>
    public SendGridOptions? SendGrid { get; init; }

    /// <summary>
    /// Gets or sets the Amazon SES-specific configuration options.
    /// Only required when using Amazon SES as the email provider.
    /// </summary>
    public AmazonSesOptions? AmazonSes { get; init; }

    /// <summary>
    /// Gets or sets the development environment-specific configuration options.
    /// Only used when the Provider is set to "Development".
    /// </summary>
    public ConsoleEmailOptions? Development { get; init; }
}

/// <summary>
/// Represents configuration options specific to the SendGrid email provider.
/// </summary>
/// <remarks>
/// These options are only required when using SendGrid as the email provider.
/// The API key can be obtained from the SendGrid dashboard.
/// </remarks>
public class SendGridOptions
{
    /// <summary>
    /// Gets or sets the SendGrid API key used for authentication.
    /// </summary>
    /// <remarks>
    /// The API key should have sufficient permissions to send emails.
    /// It's recommended to use an API key with minimal necessary permissions
    /// and to store it securely in application secrets or environment variables.
    /// </remarks>
    public required string ApiKey { get; init; }
}

/// <summary>
/// Represents configuration options specific to the Amazon Simple Email Service (SES) provider.
/// </summary>
/// <remarks>
/// These options are only required when using Amazon SES as the email provider.
/// The credentials should be for an IAM user or role with appropriate SES permissions.
/// </remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class AmazonSesOptions
{
    /// <summary>
    /// Gets or sets the AWS access key ID used for authentication.
    /// </summary>
    /// <remarks>
    /// It's recommended to use IAM role-based authentication in production
    /// environments rather than storing access keys in configuration.
    /// </remarks>
    public required string AccessKey { get; init; }

    /// <summary>
    /// Gets or sets the AWS secret access key used for authentication.
    /// </summary>
    /// <remarks>
    /// This should be kept secure and not stored in version control.
    /// Consider using AWS Secrets Manager or environment variables in production.
    /// </remarks>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Gets or sets the AWS region where the SES service is configured.
    /// </summary>
    /// <remarks>
    /// Should be a valid AWS region identifier where SES is available,
    /// for example: "us-east-1", "eu-west-1", etc.
    /// </remarks>
    public required string Region { get; init; }
}

/// <summary>
/// Represents configuration options for the development email provider
/// that outputs emails to the console instead of sending them.
/// </summary>
/// <remarks>
/// These options are only used when running in development mode
/// and the Provider is set to "Development". This provider is useful
/// for testing and debugging email functionality without sending actual emails.
/// </remarks>
public class ConsoleEmailOptions
{
    /// <summary>
    /// Gets or sets whether to use colors in the console output.
    /// Defaults to true.
    /// </summary>
    public bool UseColors { get; init; }

    /// <summary>
    /// Gets or sets whether to show the HTML body of the email in the console output.
    /// Defaults to true.
    /// </summary>
    public bool ShowHtmlBody { get; init; }

    /// <summary>
    /// Gets or sets whether to show the plain text body of the email in the console output.
    /// Defaults to false.
    /// </summary>
    public bool ShowPlainTextBody { get; init; } = true;
}