using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResultObject;

namespace MailFusion.Templates;

/// <summary>
/// Implements a file system-based template loader that reads email templates from a configured directory.
/// This loader supports both HTML and plain text template formats and includes security measures
/// to prevent directory traversal attacks.
/// </summary>
/// <remarks>
/// <para>
/// The FileTemplateLoader provides:
/// <list type="bullet">
///   <item><description>File system based template storage and retrieval</description></item>
///   <item><description>Security validation for template paths</description></item>
///   <item><description>Detailed error reporting for common failure scenarios</description></item>
///   <item><description>Support for both HTML and text template formats</description></item>
/// </list>
/// </para>
/// <para>
/// Expected file structure:
/// <list type="bullet">
///   <item><description>Template files must be in pairs (HTML and text)</description></item>
///   <item><description>HTML templates use .html extension</description></item>
///   <item><description>Text templates use .txt extension</description></item>
///   <item><description>Both files should share the same base name</description></item>
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
/// Directory structure:
/// <code>
/// Templates/
///   └── Email/
///       ├── welcome/
///       │   ├── welcome.html
///       │   └── welcome.txt
///       └── order-confirmation/
///           ├── order-confirmation.html
///           └── order-confirmation.txt
/// </code>
/// </example>
public class FileTemplateLoader : IEmailTemplateLoader
{
    private readonly string _templatesPath;
    private readonly ILogger<FileTemplateLoader> _logger;

    /// <summary>
    /// Initializes a new instance of the FileTemplateLoader class.
    /// </summary>
    /// <param name="options">The email template configuration options.</param>
    /// <param name="logger">The logger instance for recording operations and errors.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the template path is not configured in the options.
    /// </exception>
    /// <remarks>
    /// The constructor validates the configuration and sets up the template path.
    /// It ensures that the templates path is properly configured before the loader
    /// can be used.
    /// </remarks>
    public FileTemplateLoader(IOptions<EmailTemplateOptions> options, ILogger<FileTemplateLoader> logger)
    {
        var templatesPath = options.Value.File?.TemplatesPath;
        if (string.IsNullOrEmpty(templatesPath))
        {
            throw new InvalidOperationException("Template path is not configured");
        }

        _templatesPath = templatesPath;
        _logger = logger;
    }

    /// <summary>
    /// Loads both HTML and text versions of an email template asynchronously.
    /// </summary>
    /// <param name="templateName">The name of the template to load.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a result containing
    /// both HTML and text versions of the template if successful, or an error if the operation fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method performs several validations and operations:
    /// <list type="bullet">
    ///   <item><description>Validates the template name</description></item>
    ///   <item><description>Constructs and validates safe file paths</description></item>
    ///   <item><description>Checks for file existence</description></item>
    ///   <item><description>Reads and validates template content</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Security measures include:
    /// <list type="bullet">
    ///   <item><description>Path traversal prevention</description></item>
    ///   <item><description>Template path validation</description></item>
    ///   <item><description>Access restriction to template directory</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public async Task<IResult<(string html, string text)>> LoadTemplateAsync(string templateName)
    {
        try
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return Result.Failure<(string html, string text)>(
                    new ResultError(
                        TemplateErrors.Codes.InvalidTemplatePath,
                        TemplateErrors.Reasons.InvalidTemplatePath,
                        "Template name cannot be empty",
                        ErrorCategory.Validation
                    )
                );
            }

            var htmlPath = Path.Combine(_templatesPath, $"{templateName}.html");
            var textPath = Path.Combine(_templatesPath, $"{templateName}.txt");

            // Validate paths to prevent directory traversal
            if (!IsPathSafe(htmlPath) || !IsPathSafe(textPath))
            {
                return Result.Failure<(string html, string text)>(
                    CreateDetailedError(
                        TemplateErrors.Codes.InvalidTemplatePath,
                        TemplateErrors.Reasons.InvalidTemplatePath,
                        $"Invalid template path detected for template: {templateName}",
                        ErrorCategory.Validation
                    )
                );
            }

            // Check if both files exist
            if (!File.Exists(htmlPath))
            {
                return Result.Failure<(string html, string text)>(
                    CreateDetailedError(
                        TemplateErrors.Codes.HtmlTemplateNotFound,
                        TemplateErrors.Reasons.HtmlTemplateNotFound,
                        $"HTML template not found for: {templateName}",
                        ErrorCategory.NotFound,
                        templatePath: htmlPath
                    )
                );
            }

            if (!File.Exists(textPath))
            {
                return Result.Failure<(string html, string text)>(
                    CreateDetailedError(
                        TemplateErrors.Codes.TextTemplateNotFound,
                        TemplateErrors.Reasons.TextTemplateNotFound,
                        $"Text template not found for: {templateName}",
                        ErrorCategory.NotFound,
                        templatePath: textPath
                    )
                );
            }

            try
            {
                var html = await File.ReadAllTextAsync(htmlPath);
                var text = await File.ReadAllTextAsync(textPath);

                if (string.IsNullOrWhiteSpace(html) || string.IsNullOrWhiteSpace(text))
                {
                    return Result.Failure<(string html, string text)>(
                        CreateDetailedError(
                            TemplateErrors.Codes.TemplateReadError,
                            TemplateErrors.Reasons.TemplateReadError,
                            $"One or both templates are empty for: {templateName}",
                            ErrorCategory.Internal,
                            templatePath: htmlPath
                        )
                    );
                }

                return Result.Success((html, text));
            }
            catch (IOException ex)
            {
                return Result.Failure<(string html, string text)>(
                    CreateDetailedError(
                        TemplateErrors.Codes.TemplateReadError,
                        TemplateErrors.Reasons.TemplateReadError,
                        $"Failed to read template files for: {templateName}",
                        ErrorCategory.Internal,
                        exception: ex,
                        templatePath: htmlPath
                    )
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result.Failure<(string html, string text)>(
                    CreateDetailedError(
                        TemplateErrors.Codes.TemplateReadError,
                        TemplateErrors.Reasons.TemplateReadError,
                        $"Access denied while reading template files for: {templateName}",
                        ErrorCategory.Unauthorized,
                        exception: ex,
                        templatePath: htmlPath
                    )
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading template: {TemplateName}", templateName);

            return Result.Failure<(string html, string text)>(
                CreateDetailedError(
                    TemplateErrors.Codes.UnexpectedError,
                    TemplateErrors.Reasons.UnexpectedError,
                    TemplateErrors.Messages.UnexpectedError,
                    ErrorCategory.Internal,
                    exception: ex
                )
            );
        }
    }

    /// <summary>
    /// Validates that a given path is safe and within the configured templates directory.
    /// </summary>
    /// <param name="path">The path to validate.</param>
    /// <returns>True if the path is safe, false otherwise.</returns>
    /// <remarks>
    /// <para>
    /// Path safety checks include:
    /// <list type="bullet">
    ///   <item><description>Resolving to absolute path</description></item>
    ///   <item><description>Checking if path is under templates directory</description></item>
    ///   <item><description>Handling path normalization</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private bool IsPathSafe(string path)
    {
        try
        {
            // Get the full path and check if it's under the templates directory
            var fullPath = Path.GetFullPath(path);
            var templatesFullPath = Path.GetFullPath(_templatesPath);

            return fullPath.StartsWith(templatesFullPath, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a detailed error result for template operations.
    /// </summary>
    /// <param name="code">The error code identifying the type of error.</param>
    /// <param name="reason">The reason describing the error category.</param>
    /// <param name="message">The detailed error message.</param>
    /// <param name="category">The category of the error.</param>
    /// <param name="exception">Optional exception that caused the error.</param>
    /// <param name="templatePath">Optional path to the template that caused the error.</param>
    /// <returns>A ResultError object with detailed error information.</returns>
    /// <remarks>
    /// <para>
    /// The error includes:
    /// <list type="bullet">
    ///   <item><description>Error code and reason for programmatic handling</description></item>
    ///   <item><description>Detailed message for debugging</description></item>
    ///   <item><description>Template path for context</description></item>
    ///   <item><description>Exception details when available</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private static ResultError CreateDetailedError(
        string code,
        string reason,
        string message,
        ErrorCategory category,
        Exception? exception = null,
        string? templatePath = null)
    {
        var detailedMessage = new List<string>
        {
            message
        };

        if (templatePath != null)
        {
            detailedMessage.Add($"Template Path: {templatePath}");
        }

        if (exception != null)
        {
            detailedMessage.Add($"Exception Type: {exception.GetType().FullName}");
            detailedMessage.Add($"Exception Message: {exception.Message}");
        }

        var error = new ResultError(
            code,
            reason,
            string.Join(Environment.NewLine, detailedMessage),
            category
        );

        return error;
    }
}