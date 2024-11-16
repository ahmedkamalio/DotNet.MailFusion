namespace MailFusion.Templates;

/// <summary>
/// Provides standardized error definitions specific to Scriban template processing operations.
/// This class defines error codes, reasons, and messages for failures that can occur during
/// template compilation, rendering, and model binding with the Scriban template engine.
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
/// These error definitions specifically handle Scriban-related issues such as:
/// <list type="bullet">
///   <item><description>Template syntax and compilation errors</description></item>
///   <item><description>Template rendering failures</description></item>
///   <item><description>Model binding and validation issues</description></item>
///   <item><description>Runtime execution errors in templates</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Using Scriban template errors in error handling:
/// <code>
/// if (model == null)
/// {
///     return Result.Failure(new ResultError(
///         ScribanTemplateErrors.Codes.ModelNull,
///         ScribanTemplateErrors.Reasons.ModelNull,
///         ScribanTemplateErrors.Messages.ModelNull
///     ));
/// }
/// </code>
/// </example>
public static class ScribanTemplateErrors
{
    /// <summary>
    /// Defines standardized error codes for Scriban template processing operations.
    /// These codes are used for programmatic error handling and logging.
    /// </summary>
    /// <remarks>
    /// Error codes follow the format: TEMPLATE_[ERROR_TYPE]
    /// These codes should remain stable across versions for reliable error handling.
    /// </remarks>
    public static class Codes
    {
        /// <summary>
        /// Indicates that the template model provided for rendering is null or invalid.
        /// This occurs when attempting to render a template without required model data.
        /// </summary>
        public const string ModelNull = "TEMPLATE_MODEL_NULL";

        /// <summary>
        /// Indicates that the template failed to compile due to syntax errors or invalid script.
        /// This typically occurs during the template parsing phase.
        /// </summary>
        public const string CompilationError = "TEMPLATE_COMPILATION_ERROR";

        /// <summary>
        /// Indicates that an error occurred while rendering the template with the provided model.
        /// This may be due to runtime errors in the template script.
        /// </summary>
        public const string RenderError = "TEMPLATE_RENDER_ERROR";

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
        /// Indicates that the required template model is missing.
        /// </summary>
        public const string ModelNull = "Template Model Missing";

        /// <summary>
        /// Indicates that the template compilation process failed.
        /// </summary>
        public const string CompilationError = "Template Compilation Failed";

        /// <summary>
        /// Indicates that the template rendering process failed.
        /// </summary>
        public const string RenderError = "Template Rendering Failed";

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
    ///   <item><description>Clear and informative for developers</description></item>
    ///   <item><description>Specific to Scriban template processing issues</description></item>
    ///   <item><description>Actionable with guidance on potential fixes</description></item>
    /// </list>
    /// </remarks>
    public static class Messages
    {
        /// <summary>
        /// Error message for when the template model is null or invalid.
        /// </summary>
        public const string ModelNull = "The template model cannot be null.";

        /// <summary>
        /// Error message for template compilation failures.
        /// Suggests checking template syntax as a remediation step.
        /// </summary>
        public const string CompilationError = "Failed to compile the template. Check template syntax.";

        /// <summary>
        /// Error message for template rendering failures.
        /// </summary>
        public const string RenderError = "Failed to render the template.";

        /// <summary>
        /// Error message for unexpected errors during template processing.
        /// </summary>
        public const string UnexpectedError = "An unexpected error occurred while processing the template.";
    }
}