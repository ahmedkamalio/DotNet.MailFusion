namespace MailFusion;

/// <summary>
/// Provides standardized error definitions for the email service operations.
/// This class includes error codes, reasons, and user-friendly messages for various
/// failure scenarios that may occur during email processing and delivery.
/// </summary>
/// <remarks>
/// The class is organized into three nested classes:
/// <list type="bullet">
///   <item><description><see cref="Codes"/> - Contains constant error code strings used for programmatic error handling</description></item>
///   <item><description><see cref="Reasons"/> - Contains short, descriptive reasons for each error type</description></item>
///   <item><description><see cref="Messages"/> - Contains detailed, user-friendly error messages</description></item>
/// </list>
/// </remarks>
public static class EmailServiceErrors
{
    /// <summary>
    /// Defines standardized error codes for email service operations.
    /// These codes are used for programmatic error handling and logging.
    /// </summary>
    /// <remarks>
    /// Error codes follow the format: EMAIL_[ERROR_TYPE]
    /// These codes should remain stable across versions for reliable error handling.
    /// </remarks>
    public static class Codes
    {
        /// <summary>
        /// Indicates that one or more input parameters are invalid or missing.
        /// </summary>
        public const string InvalidInput = "EMAIL_INVALID_INPUT";

        /// <summary>
        /// Indicates an error occurred during email template processing.
        /// </summary>
        public const string TemplateError = "EMAIL_TEMPLATE_ERROR";

        /// <summary>
        /// Indicates an unexpected or unhandled error occurred during email processing.
        /// </summary>
        public const string UnexpectedError = "EMAIL_UNEXPECTED_ERROR";
    }

    /// <summary>
    /// Provides concise, human-readable reasons for each error type.
    /// These reasons are used for logging and error reporting.
    /// </summary>
    public static class Reasons
    {
        /// <summary>
        /// Indicates that the provided email input parameters are invalid.
        /// </summary>
        public const string InvalidInput = "Invalid Email Input";

        /// <summary>
        /// Indicates that the email template processing operation failed.
        /// </summary>
        public const string TemplateError = "Template Processing Failed";

        /// <summary>
        /// Indicates an unexpected error occurred during email processing.
        /// </summary>
        public const string UnexpectedError = "Unexpected Error";
    }

    /// <summary>
    /// Provides detailed, user-friendly error messages for each error scenario.
    /// These messages can be displayed to users or included in error reports.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Error message for when the email template model is null or invalid.
        /// </summary>
        public const string InvalidModel = "The email template model cannot be null.";

        /// <summary>
        /// Error message for when the email sender information is missing or invalid.
        /// </summary>
        public const string InvalidSender = "The email sender information cannot be null.";

        /// <summary>
        /// Error message for when the email recipients list is empty or invalid.
        /// </summary>
        public const string InvalidRecipients = "The email recipients list cannot be null or empty.";

        /// <summary>
        /// Error message for when the template name is missing or invalid.
        /// </summary>
        public const string InvalidTemplateName = "The template name cannot be null or empty.";

        /// <summary>
        /// Error message for when email template processing fails.
        /// </summary>
        public const string TemplateError = "Failed to process the email template.";

        /// <summary>
        /// Error message for unexpected errors during email processing.
        /// </summary>
        public const string UnexpectedError = "An unexpected error occurred while sending the email.";
    }
}