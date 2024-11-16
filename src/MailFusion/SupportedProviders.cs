namespace MailFusion;

/// <summary>
/// Defines the supported email service providers in the MailFusion library.
/// </summary>
public static class SupportedProviders
{
    /// <summary>
    /// Represents SendGrid as an email service provider.
    /// Use this value in configuration to send emails via SendGrid's email delivery service.
    /// </summary>
    public const string SendGrid = "SendGrid";

    /// <summary>
    /// Represents Amazon Simple Email Service (SES) as an email service provider.
    /// Use this value in configuration to send emails via Amazon SES.
    /// </summary>
    public const string AmazonSes = "AmazonSes";

    /// <summary>
    /// Represents the development email provider that outputs emails to the console.
    /// Use this value in configuration to log emails to the console instead of sending them.
    /// This provider is only available in development environments.
    /// </summary>
    public const string Development = "Development";
}