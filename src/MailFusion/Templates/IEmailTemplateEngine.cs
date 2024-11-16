using ResultObject;

namespace MailFusion.Templates;

/// <summary>
/// Defines the contract for an email template engine that processes templates with model data
/// to generate complete email content. The engine is responsible for loading, compiling, and
/// rendering email templates using strongly-typed model data.
/// </summary>
/// <remarks>
/// <para>
/// The template engine serves as the core component for email content generation, providing:
/// <list type="bullet">
///   <item><description>Template compilation and rendering</description></item>
///   <item><description>Model binding and data validation</description></item>
///   <item><description>Support for both HTML and plain text output</description></item>
///   <item><description>Error handling and validation</description></item>
/// </list>
/// </para>
/// <para>
/// Common implementations might include:
/// <list type="bullet">
///   <item><description>ScribanEmailTemplateEngine - Uses the Scriban template engine</description></item>
///   <item><description>RazorEmailTemplateEngine - Uses Razor templating</description></item>
///   <item><description>HandlebarsEmailTemplateEngine - Uses Handlebars templating</description></item>
/// </list>
/// </para>
/// <para>
/// The engine typically works in conjunction with:
/// <list type="bullet">
///   <item><description>IEmailTemplateLoader - For loading raw template content</description></item>
///   <item><description>IEmailTemplateModel - For providing data to templates</description></item>
///   <item><description>IEmailTemplate - For representing processed templates</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Example usage with a welcome email template:
/// <code>
/// public class WelcomeEmailModel : IEmailTemplateModel
/// {
///     public string Subject => "Welcome to Our Service!";
///     public string Email { get; init; }
///     public string UserName { get; init; }
/// }
/// 
/// // Using the template engine
/// var model = new WelcomeEmailModel 
/// { 
///     Email = "user@example.com",
///     UserName = "John Doe"
/// };
/// 
/// var result = await templateEngine.LoadTemplateAsync(
///     "welcome-email",
///     model
/// );
/// 
/// if (result.IsSuccess)
/// {
///     var template = result.Value;
///     // Use template.Subject, template.HtmlBody, and template.PlainTextBody
/// }
/// </code>
/// </example>
public interface IEmailTemplateEngine
{
    /// <summary>
    /// Loads and processes an email template using the provided model data.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model, which must implement IEmailTemplateModel.</typeparam>
    /// <param name="templateName">The name of the template to load and process.</param>
    /// <param name="model">The model containing data to be used in the template.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a result with the processed
    /// email template if successful, or an error if the operation fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method performs several key operations:
    /// <list type="bullet">
    ///   <item><description>Loads the raw template content</description></item>
    ///   <item><description>Validates the template and model data</description></item>
    ///   <item><description>Compiles the template if necessary</description></item>
    ///   <item><description>Renders the template with the provided model</description></item>
    ///   <item><description>Generates both HTML and plain text versions</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Common failure scenarios that should be handled include:
    /// <list type="bullet">
    ///   <item><description>Template not found or invalid</description></item>
    ///   <item><description>Template compilation errors</description></item>
    ///   <item><description>Model validation failures</description></item>
    ///   <item><description>Rendering errors</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// While implementations should generally return failure results rather than throw exceptions,
    /// unexpected errors during template processing may still result in exceptions.
    /// </exception>
    Task<IResult<IEmailTemplate>> LoadTemplateAsync<TModel>(string templateName, TModel model)
        where TModel : IEmailTemplateModel;
}