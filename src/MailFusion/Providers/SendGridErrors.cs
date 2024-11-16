namespace MailFusion.Providers;

/// <summary>
/// Provides standardized error definitions specific to SendGrid email provider operations.
/// This class defines error codes, reasons, and messages for failures that can occur during
/// email sending operations through the SendGrid API.
/// </summary>
/// <remarks>
/// <para>
/// The class is organized into three nested classes:
/// <list type="bullet">
///   <item><description><see cref="Codes"/> - Contains constant error code strings for programmatic error handling</description></item>
///   <item><description><see cref="Reasons"/> - Contains short, descriptive reasons for each error type</description></item>
///   <item><description><see cref="Messages"/> - Contains detailed, user-friendly error messages</description></item>
/// </list>
/// </para>
/// <para>
/// These error definitions specifically handle SendGrid-related issues such as:
/// <list type="bullet">
///   <item><description>API authentication and authorization failures</description></item>
///   <item><description>Rate limiting and quota exceeded scenarios</description></item>
///   <item><description>Message validation and delivery issues</description></item>
///   <item><description>Unexpected API responses and client errors</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Using SendGrid error definitions in error handling:
/// <code>
/// if (response.StatusCode == HttpStatusCode.TooManyRequests)
/// {
///     return Result.Failure(new ResultError(
///         SendGridErrors.Codes.RateLimitExceeded,
///         SendGridErrors.Reasons.RateLimitExceeded,
///         SendGridErrors.Messages.RateLimitExceeded
///     ));
/// }
/// </code>
/// </example>
public static class SendGridErrors
{
    /// <summary>
    /// Defines standardized error codes for SendGrid operations.
    /// These codes are used for programmatic error handling and logging.
    /// </summary>
    /// <remarks>
    /// Error codes follow the format: SENDGRID_[ERROR_TYPE]
    /// These codes should remain stable across versions for reliable error handling.
    /// The only exception is OPERATION_CANCELLED which follows a different pattern
    /// as it's a general operation state rather than a SendGrid-specific error.
    /// </remarks>
    public static class Codes
    {
        /// <summary>
        /// Indicates a general error response from the SendGrid API.
        /// This is used when the error doesn't match other more specific error types.
        /// </summary>
        public const string ApiError = "SENDGRID_API_ERROR";

        /// <summary>
        /// Indicates that the SendGrid API rate limit has been exceeded.
        /// This typically occurs when too many requests are made within a time window.
        /// </summary>
        public const string RateLimitExceeded = "SENDGRID_RATE_LIMIT";

        /// <summary>
        /// Indicates that authentication with the SendGrid API failed.
        /// This typically occurs with invalid or expired API keys.
        /// </summary>
        public const string AuthenticationError = "SENDGRID_AUTH_ERROR";

        /// <summary>
        /// Indicates that the email message failed SendGrid's validation checks.
        /// This can occur due to invalid email addresses, content issues, or other validation failures.
        /// </summary>
        public const string ValidationError = "SENDGRID_VALIDATION_ERROR";

        /// <summary>
        /// Indicates that the email sending operation was cancelled by the client.
        /// This typically occurs when a cancellation token is triggered.
        /// </summary>
        public const string OperationCancelled = "OPERATION_CANCELLED";

        /// <summary>
        /// Indicates an unexpected or unhandled error occurred during the SendGrid operation.
        /// This is used for errors that don't fall into other categories or are truly unexpected.
        /// </summary>
        public const string UnexpectedError = "SENDGRID_UNEXPECTED_ERROR";
    }

    /// <summary>
    /// Provides concise, human-readable reasons for each error type.
    /// These reasons are used for logging and error reporting.
    /// </summary>
    /// <remarks>
    /// The reasons provide a quick overview of what went wrong, while the corresponding
    /// messages in the Messages class provide more detailed explanations.
    /// </remarks>
    public static class Reasons
    {
        /// <summary>
        /// Indicates a general SendGrid API error occurred.
        /// </summary>
        public const string ApiError = "SendGrid API Error";

        /// <summary>
        /// Indicates that the rate limit for SendGrid API requests has been exceeded.
        /// </summary>
        public const string RateLimitExceeded = "Rate Limit Exceeded";

        /// <summary>
        /// Indicates that authentication with the SendGrid API failed.
        /// </summary>
        public const string AuthenticationError = "Authentication Failed";

        /// <summary>
        /// Indicates that the email message failed SendGrid's validation checks.
        /// </summary>
        public const string ValidationError = "Validation Error";

        /// <summary>
        /// Indicates that the operation was cancelled by the client.
        /// </summary>
        public const string OperationCancelled = "Operation Cancelled";

        /// <summary>
        /// Indicates an unexpected error occurred during the SendGrid operation.
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
    ///   <item><description>Clear and informative for developers</description></item>
    ///   <item><description>Specific to SendGrid operations</description></item>
    ///   <item><description>Actionable with guidance on potential fixes</description></item>
    ///   <item><description>Suitable for logging and error reporting</description></item>
    /// </list>
    /// </remarks>
    public static class Messages
    {
        /// <summary>
        /// Error message for general SendGrid API errors.
        /// Suggests checking the detailed error information for more specific guidance.
        /// </summary>
        public const string ApiError = "The SendGrid API returned an error. Please check the error details for more information.";

        /// <summary>
        /// Error message for rate limit exceeded scenarios.
        /// Provides guidance on handling rate limiting through retry or rate adjustment.
        /// </summary>
        public const string RateLimitExceeded = "SendGrid rate limit has been exceeded. Please try again later or adjust your sending rate.";

        /// <summary>
        /// Error message for authentication failures.
        /// Directs users to verify their API key configuration.
        /// </summary>
        public const string AuthenticationError = "Failed to authenticate with SendGrid. Please check your API key configuration.";

        /// <summary>
        /// Error message for validation failures.
        /// Suggests checking both message content and recipient information.
        /// </summary>
        public const string ValidationError = "The email message failed validation by SendGrid. Please check the message content and recipients.";

        /// <summary>
        /// Error message for cancelled operations.
        /// Indicates that the cancellation was client-initiated.
        /// </summary>
        public const string OperationCancelled = "The email sending operation was cancelled by the client.";

        /// <summary>
        /// Error message for unexpected errors.
        /// Used when the error doesn't match other more specific categories.
        /// </summary>
        public const string UnexpectedError = "An unexpected error occurred while sending the email through SendGrid.";
    }
}