namespace MailFusion.Providers;

/// <summary>
/// Provides standardized error definitions specific to Amazon Simple Email Service (SES) operations.
/// This class defines error codes, reasons, and messages for failures that can occur during
/// email sending operations through the AWS SES API.
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
/// These error definitions specifically handle AWS SES-related issues such as:
/// <list type="bullet">
///   <item><description>Account and configuration set status issues</description></item>
///   <item><description>Domain verification requirements</description></item>
///   <item><description>Message rejection scenarios</description></item>
///   <item><description>AWS-specific service errors</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Using AWS SES error definitions in error handling:
/// <code>
/// catch (AccountSendingPausedException ex)
/// {
///     return Result.Failure(new ResultError(
///         AmazonSesErrors.Codes.AccountPaused,
///         AmazonSesErrors.Reasons.AccountPaused,
///         AmazonSesErrors.Messages.AccountPaused
///     ));
/// }
/// </code>
/// </example>
public static class AmazonSesErrors
{
    /// <summary>
    /// Defines standardized error codes for AWS SES operations.
    /// These codes are used for programmatic error handling and logging.
    /// </summary>
    /// <remarks>
    /// Error codes follow the format: AWS_[ERROR_TYPE]
    /// These codes should remain stable across versions for reliable error handling.
    /// The only exception is OPERATION_CANCELLED which follows a different pattern
    /// as it's a general operation state rather than an AWS-specific error.
    /// </remarks>
    public static class Codes
    {
        /// <summary>
        /// Indicates that email sending is currently disabled for the AWS SES account.
        /// This typically occurs when AWS has detected potential policy violations or unusual activity.
        /// </summary>
        public const string AccountPaused = "AWS_ACCOUNT_PAUSED";

        /// <summary>
        /// Indicates that the specified AWS SES configuration set does not exist.
        /// This occurs when attempting to use a configuration set that hasn't been created in AWS SES.
        /// </summary>
        public const string ConfigNotFound = "AWS_CONFIG_NOT_FOUND";

        /// <summary>
        /// Indicates that email sending is disabled for the specified configuration set.
        /// This can occur when a configuration set has been manually paused or disabled.
        /// </summary>
        public const string ConfigPaused = "AWS_CONFIG_PAUSED";

        /// <summary>
        /// Indicates that the MAIL FROM domain has not been verified in AWS SES.
        /// This error occurs when attempting to send from a domain that hasn't completed verification.
        /// </summary>
        public const string DomainNotVerified = "AWS_DOMAIN_NOT_VERIFIED";

        /// <summary>
        /// Indicates that AWS SES rejected the email message.
        /// This can occur for various reasons including content issues or policy violations.
        /// </summary>
        public const string MessageRejected = "AWS_MESSAGE_REJECTED";

        /// <summary>
        /// Indicates that the email sending operation was cancelled by the client.
        /// This typically occurs when a cancellation token is triggered.
        /// </summary>
        public const string OperationCancelled = "OPERATION_CANCELLED";

        /// <summary>
        /// Indicates an unexpected or unhandled error occurred during the AWS SES operation.
        /// This is used for errors that don't fall into other categories or are truly unexpected.
        /// </summary>
        public const string UnexpectedError = "AWS_UNEXPECTED_ERROR";
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
        /// Indicates that the AWS SES account's sending capability is paused.
        /// </summary>
        public const string AccountPaused = "Account Sending Paused";

        /// <summary>
        /// Indicates that the specified configuration set doesn't exist.
        /// </summary>
        public const string ConfigNotFound = "Invalid Configuration";

        /// <summary>
        /// Indicates that the configuration set's sending capability is paused.
        /// </summary>
        public const string ConfigPaused = "Configuration Set Paused";

        /// <summary>
        /// Indicates that the sender domain hasn't been verified.
        /// </summary>
        public const string DomainNotVerified = "Domain Not Verified";

        /// <summary>
        /// Indicates that the message was rejected by AWS SES.
        /// </summary>
        public const string MessageRejected = "Message Rejected";

        /// <summary>
        /// Indicates that the operation was cancelled by the client.
        /// </summary>
        public const string OperationCancelled = "Operation Cancelled";

        /// <summary>
        /// Indicates an unexpected error occurred during the AWS SES operation.
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
    ///   <item><description>Specific to AWS SES operations</description></item>
    ///   <item><description>Actionable with guidance on potential fixes</description></item>
    ///   <item><description>Suitable for logging and error reporting</description></item>
    /// </list>
    /// </remarks>
    public static class Messages
    {
        /// <summary>
        /// Error message for account sending paused scenarios.
        /// Provides guidance on checking account status and contacting AWS support.
        /// </summary>
        public const string AccountPaused = "Email sending is currently disabled for this AWS SES account. Check your AWS SES account status and contact AWS support if needed.";

        /// <summary>
        /// Error message for configuration set not found scenarios.
        /// Suggests verifying the configuration set name and setup.
        /// </summary>
        public const string ConfigNotFound = "The specified AWS SES configuration set does not exist. Verify the configuration set name and ensure it's properly set up in AWS SES.";

        /// <summary>
        /// Error message for configuration set paused scenarios.
        /// Directs users to check the configuration set status.
        /// </summary>
        public const string ConfigPaused = "Email sending is currently disabled for this configuration set. Check the configuration set status in AWS SES.";

        /// <summary>
        /// Error message for domain verification issues.
        /// Reminds users to complete domain verification before sending.
        /// </summary>
        public const string DomainNotVerified = "The MAIL FROM domain is not verified in AWS SES. Verify your domain in AWS SES before sending emails.";

        /// <summary>
        /// Error message for cancelled operations.
        /// Indicates that the cancellation was client-initiated.
        /// </summary>
        public const string OperationCancelled = "The email sending operation was cancelled by the client.";

        /// <summary>
        /// Error message for unexpected errors.
        /// Used when the error doesn't match other more specific categories.
        /// </summary>
        public const string UnexpectedError = "An unexpected error occurred while sending the email through AWS SES.";
    }
}