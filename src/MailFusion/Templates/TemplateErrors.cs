namespace MailFusion.Templates;

/// <summary>
/// Provides standardized error definitions for email template operations.
/// This class includes error codes, reasons, and user-friendly messages for various
/// failure scenarios that may occur during template loading and processing.
/// </summary>
/// <remarks>
/// <para>
/// The class is organized into three nested classes:
/// <list type="bullet">
///   <item><description><see cref="Codes"/> - Contains constant error code strings used for programmatic error handling</description></item>
///   <item><description><see cref="Reasons"/> - Contains short, descriptive reasons for each error type</description></item>
///   <item><description><see cref="Messages"/> - Contains detailed, user-friendly error messages</description></item>
/// </list>
/// </para>
/// <para>
/// These error definitions are used throughout the template processing pipeline to provide
/// consistent error reporting and handling for scenarios such as:
/// <list type="bullet">
///   <item><description>Missing or invalid template configuration</description></item>
///   <item><description>Template file access and reading issues</description></item>
///   <item><description>Template validation failures</description></item>
///   <item><description>Unexpected errors during template processing</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Using template errors in error handling:
/// <code>
/// if (!File.Exists(templatePath))
/// {
///     return Result.Failure(new ResultError(
///         TemplateErrors.Codes.HtmlTemplateNotFound,
///         TemplateErrors.Reasons.HtmlTemplateNotFound,
///         TemplateErrors.Messages.HtmlTemplateNotFound
///     ));
/// }
/// </code>
/// </example>
public static class TemplateErrors
{
    /// <summary>
    /// Defines standardized error codes for template operations.
    /// These codes are used for programmatic error handling and logging.
    /// </summary>
    /// <remarks>
    /// Error codes follow the format: TEMPLATE_[ERROR_TYPE]
    /// These codes should remain stable across versions for reliable error handling.
    /// </remarks>
    public static class Codes
    {
        /// <summary>
        /// Indicates that the template configuration is missing or invalid.
        /// This typically occurs when the template path or other required settings are not properly configured.
        /// </summary>
        public const string TemplateNotConfigured = "TEMPLATE_NOT_CONFIGURED";

        /// <summary>
        /// Indicates that the HTML version of a template file could not be found at the specified location.
        /// </summary>
        public const string HtmlTemplateNotFound = "HTML_TEMPLATE_NOT_FOUND";

        /// <summary>
        /// Indicates that the plain text version of a template file could not be found at the specified location.
        /// </summary>
        public const string TextTemplateNotFound = "TEXT_TEMPLATE_NOT_FOUND";

        /// <summary>
        /// Indicates that the provided template path is invalid, inaccessible, or poses a security risk.
        /// This may occur due to insufficient permissions or attempted directory traversal.
        /// </summary>
        public const string InvalidTemplatePath = "INVALID_TEMPLATE_PATH";

        /// <summary>
        /// Indicates an error occurred while attempting to read or access the template file.
        /// This may be due to file system issues or permission problems.
        /// </summary>
        public const string TemplateReadError = "TEMPLATE_READ_ERROR";

        /// <summary>
        /// Indicates an unexpected or unhandled error occurred during template processing.
        /// </summary>
        public const string UnexpectedError = "TEMPLATE_UNEXPECTED_ERROR";
    }

    /// <summary>
    /// Provides concise, human-readable reasons for each error type.
    /// These reasons are used for logging and error reporting.
    /// </summary>
    /// <remarks>
    /// The reasons provide a quick overview of what went wrong, while the corresponding
    /// messages provide more detailed explanations.
    /// </remarks>
    public static class Reasons
    {
        /// <summary>
        /// Indicates that the template configuration is missing or invalid.
        /// </summary>
        public const string TemplateNotConfigured = "Template Configuration Missing";

        /// <summary>
        /// Indicates that the HTML template file could not be found.
        /// </summary>
        public const string HtmlTemplateNotFound = "HTML Template Not Found";

        /// <summary>
        /// Indicates that the text template file could not be found.
        /// </summary>
        public const string TextTemplateNotFound = "Text Template Not Found";

        /// <summary>
        /// Indicates that the provided template path is invalid or inaccessible.
        /// </summary>
        public const string InvalidTemplatePath = "Invalid Template Path";

        /// <summary>
        /// Indicates an error occurred while reading the template file.
        /// </summary>
        public const string TemplateReadError = "Template Read Error";

        /// <summary>
        /// Indicates an unexpected error occurred during template processing.
        /// </summary>
        public const string UnexpectedError = "Unexpected Error";
    }

    /// <summary>
    /// Provides detailed, user-friendly error messages for each error scenario.
    /// These messages can be displayed to users or included in error reports.
    /// </summary>
    /// <remarks>
    /// Messages are designed to be:
    /// <list type="bullet">
    ///   <item><description>Clear and informative for users</description></item>
    ///   <item><description>Actionable with suggested remediation steps</description></item>
    ///   <item><description>Consistent with the application's error messaging style</description></item>
    /// </list>
    /// </remarks>
    public static class Messages
    {
        /// <summary>
        /// Error message for when the template configuration is missing or invalid.
        /// </summary>
        public const string TemplateNotConfigured = "The template configuration is missing. Please configure the template path in your application settings.";

        /// <summary>
        /// Error message for when the HTML template file cannot be found.
        /// </summary>
        public const string HtmlTemplateNotFound = "The HTML template file could not be found at the specified location.";

        /// <summary>
        /// Error message for when the text template file cannot be found.
        /// </summary>
        public const string TextTemplateNotFound = "The text template file could not be found at the specified location.";

        /// <summary>
        /// Error message for when the template path is invalid or inaccessible.
        /// </summary>
        public const string InvalidTemplatePath = "The configured template path is invalid or inaccessible.";

        /// <summary>
        /// Error message for when an error occurs while reading the template file.
        /// </summary>
        public const string TemplateReadError = "An error occurred while reading the template file.";

        /// <summary>
        /// Error message for unexpected errors during template processing.
        /// </summary>
        public const string UnexpectedError = "An unexpected error occurred while processing the template.";
    }
}