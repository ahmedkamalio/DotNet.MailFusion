using ResultObject;

namespace MailFusion.Templates;

/// <summary>
/// Defines the contract for loading email templates from a storage system.
/// This interface abstracts the template loading mechanism, allowing for different
/// storage implementations (file system, database, remote storage, etc.).
/// </summary>
/// <remarks>
/// <para>
/// The interface provides a single method for loading both HTML and plain text versions
/// of an email template. Implementations should handle:
/// <list type="bullet">
///   <item><description>Template storage access and retrieval</description></item>
///   <item><description>Basic validation of template content</description></item>
///   <item><description>Security considerations (e.g., path traversal prevention)</description></item>
///   <item><description>Error handling for missing or invalid templates</description></item>
/// </list>
/// </para>
/// <para>
/// Common implementations might include:
/// <list type="bullet">
///   <item><description>FileTemplateLoader - Loads templates from the file system</description></item>
///   <item><description>DatabaseTemplateLoader - Loads templates from a database</description></item>
///   <item><description>CachedTemplateLoader - Adds caching to another loader</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Example implementation for file-based template loading:
/// <code>
/// public class FileTemplateLoader : IEmailTemplateLoader
/// {
///     private readonly string _templatesPath;
///     
///     public async Task&lt;IResult&lt;(string html, string text)&gt;&gt; LoadTemplateAsync(string templateName)
///     {
///         var htmlPath = Path.Combine(_templatesPath, $"{templateName}.html");
///         var textPath = Path.Combine(_templatesPath, $"{templateName}.txt");
///         
///         if (!File.Exists(htmlPath) || !File.Exists(textPath))
///         {
///             return Result.Failure&lt;(string, string)&gt;(new ResultError(...));
///         }
///         
///         var html = await File.ReadAllTextAsync(htmlPath);
///         var text = await File.ReadAllTextAsync(textPath);
///         
///         return Result.Success((html, text));
///     }
/// }
/// </code>
/// </example>
public interface IEmailTemplateLoader
{
    /// <summary>
    /// Loads both HTML and plain text versions of an email template asynchronously.
    /// </summary>
    /// <param name="templateName">The name or identifier of the template to load.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a result with a tuple of
    /// (html, text) strings if successful, or an error if the operation fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method should handle various failure scenarios including:
    /// <list type="bullet">
    ///   <item><description>Template not found</description></item>
    ///   <item><description>Invalid template content</description></item>
    ///   <item><description>Access permission issues</description></item>
    ///   <item><description>Storage system errors</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Both HTML and text versions should be loaded to support email clients
    /// that prefer or require plain text content.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// While implementations should generally return failure results rather than throw exceptions,
    /// unexpected errors may still result in exceptions that should be handled by the caller.
    /// </exception>
    Task<IResult<(string html, string text)>> LoadTemplateAsync(string templateName);
}