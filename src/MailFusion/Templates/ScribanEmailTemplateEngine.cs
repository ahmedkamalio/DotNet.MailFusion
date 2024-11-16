using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Logging;
using ResultObject;
using Scriban;
using Scriban.Runtime;

namespace MailFusion.Templates;

/// <summary>
/// Implements email template processing using the Scriban template engine.
/// This class handles loading, compiling, and rendering email templates for both HTML and plain text formats.
/// </summary>
/// <remarks>
/// <para>
/// The ScribanEmailTemplateEngine provides:
/// <list type="bullet">
///   <item><description>Template compilation caching for improved performance</description></item>
///   <item><description>Support for both HTML and plain text email formats</description></item>
///   <item><description>Custom template functions for common formatting tasks</description></item>
///   <item><description>Strongly-typed model binding for template data</description></item>
/// </list>
/// </para>
/// <para>
/// Templates are cached after compilation to improve performance for frequently used templates.
/// The cache uses a concurrent dictionary to ensure thread safety.
/// </para>
/// </remarks>
public class ScribanEmailTemplateEngine(
    IEmailTemplateLoader templateLoader,
    ILogger<ScribanEmailTemplateEngine> logger) : IEmailTemplateEngine
{
    /// <summary>
    /// Static cache of compiled templates to improve performance.
    /// Templates are stored using a key format of "{templateName}_{format}" where format is either "html" or "text".
    /// </summary>
    private static readonly ConcurrentDictionary<string, Template> CompiledTemplates = new();

    /// <summary>
    /// Loads and processes an email template with the provided model data.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model, which must implement IEmailTemplateModel.</typeparam>
    /// <param name="templateName">The name of the template to load and process.</param>
    /// <param name="model">The model containing data to be used in the template.</param>
    /// <returns>
    /// A result containing the processed email template with both HTML and plain text versions,
    /// or an error if template processing fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The method performs the following steps:
    /// <list type="number">
    ///   <item><description>Loads the template files using the template loader</description></item>
    ///   <item><description>Compiles the templates if not already cached</description></item>
    ///   <item><description>Creates a template context with the model data</description></item>
    ///   <item><description>Renders both HTML and plain text versions</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Templates are cached after compilation to improve performance for subsequent uses.
    /// If a template compilation error occurs, it will be logged and returned as a failure result.
    /// </para>
    /// </remarks>
    /// <example>
    /// Using the template engine with a custom model:
    /// <code>
    /// public class WelcomeEmailModel : IEmailTemplateModel
    /// {
    ///     public string Subject => "Welcome!";
    ///     public string Email { get; init; }
    ///     public string UserName { get; init; }
    /// }
    /// 
    /// var result = await templateEngine.LoadTemplateAsync(
    ///     "welcome-email",
    ///     new WelcomeEmailModel 
    ///     { 
    ///         Email = "user@example.com",
    ///         UserName = "John Doe"
    ///     }
    /// );
    /// </code>
    /// </example>
    public async Task<IResult<IEmailTemplate>> LoadTemplateAsync<TModel>(string templateName, TModel model)
        where TModel : IEmailTemplateModel
    {
        try
        {
            var result = await templateLoader.LoadTemplateAsync(templateName);
            if (result.IsFailure)
            {
                return Result.Failure<IEmailTemplate>(result.Error);
            }

            try
            {
                // Get or compile HTML template
                var compiledHtml = GetOrCompileTemplateAsync($"{templateName}_html", result.Value.html);
                // Get or compile text template
                var compiledText = GetOrCompileTemplateAsync($"{templateName}_text", result.Value.text);

                // Create template context with the model
                var context = CreateTemplateContext(model);

                try
                {
                    // Render templates
                    var htmlBody = await compiledHtml.RenderAsync(context);
                    var plainTextBody = await compiledText.RenderAsync(context);

                    return Result.Success<IEmailTemplate>(
                        EmailTemplate.Create(model.Subject, htmlBody, plainTextBody));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to render template: {TemplateName}", templateName);

                    return Result.Failure<IEmailTemplate>(
                        new ResultError(
                            ScribanTemplateErrors.Codes.RenderError,
                            ScribanTemplateErrors.Reasons.RenderError,
                            ScribanTemplateErrors.Messages.RenderError,
                            ErrorCategory.Internal
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to compile template: {TemplateName}", templateName);

                return Result.Failure<IEmailTemplate>(
                    new ResultError(
                        ScribanTemplateErrors.Codes.CompilationError,
                        ScribanTemplateErrors.Reasons.CompilationError,
                        $"{ScribanTemplateErrors.Messages.CompilationError} Template: {templateName}",
                        ErrorCategory.Internal
                    )
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while loading template: {TemplateName}", templateName);

            return Result.Failure<IEmailTemplate>(
                new ResultError(
                    ScribanTemplateErrors.Codes.UnexpectedError,
                    ScribanTemplateErrors.Reasons.UnexpectedError,
                    ScribanTemplateErrors.Messages.UnexpectedError,
                    ErrorCategory.Internal
                )
            );
        }
    }

    /// <summary>
    /// Compiles a template or retrieves it from cache.
    /// </summary>
    /// <param name="key">The cache key for the template.</param>
    /// <param name="templateContent">The template content to compile if not found in cache.</param>
    /// <returns>The compiled Scriban template.</returns>
    /// <remarks>
    /// This method ensures thread-safe access to the template cache and handles template compilation errors.
    /// If compilation fails, the error details will be included in the thrown exception.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when template compilation fails, including details about compilation errors.
    /// </exception>
    private static Template GetOrCompileTemplateAsync(string key, string templateContent)
    {
        if (CompiledTemplates.TryGetValue(key, out var compiledTemplate))
        {
            return compiledTemplate;
        }

        var template = Template.Parse(templateContent);
        if (template.HasErrors)
        {
            var errors = string.Join("\n", template.Messages);
            throw new Exception($"Template compilation errors:\n{errors}");
        }

        CompiledTemplates.TryAdd(key, template);

        return template;
    }

    /// <summary>
    /// Creates a template context with the provided model data and custom functions.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model.</typeparam>
    /// <param name="model">The model containing data for the template.</param>
    /// <returns>A configured Scriban template context.</returns>
    /// <remarks>
    /// The context includes:
    /// <list type="bullet">
    ///   <item><description>All public properties from the model</description></item>
    ///   <item><description>Custom formatting functions for dates, times, and currency</description></item>
    ///   <item><description>Text manipulation functions like Capitalize</description></item>
    /// </list>
    /// </remarks>
    private static TemplateContext CreateTemplateContext<TModel>(TModel model) where TModel : IEmailTemplateModel
    {
        var context = new TemplateContext();
        var scriptObject = new ScriptObject();

        // Add the model properties to the context
        foreach (var prop in model.GetType().GetProperties())
        {
            scriptObject.Add(prop.Name, prop.GetValue(model));
        }

        // Add custom functions
        scriptObject.Import(typeof(CustomFunctions));

        context.PushGlobal(scriptObject);

        return context;
    }

    /// <summary>
    /// Provides custom functions available within email templates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Available functions include:
    /// <list type="bullet">
    ///   <item><description>FormatDate - Formats a date in the current culture's long date pattern</description></item>
    ///   <item><description>FormatTime - Formats a time in the current culture's short time pattern</description></item>
    ///   <item><description>FormatCurrency - Formats a decimal as currency using the current culture</description></item>
    ///   <item><description>Capitalize - Capitalizes the first character of a string</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Using custom functions in a template:
    /// <code>
    /// Welcome {{ capitalize(user_name) }}!
    /// Order Date: {{ format_date(order_date) }}
    /// Total: {{ format_currency(total_amount) }}
    /// </code>
    /// </example>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static class CustomFunctions
    {
        /// <summary>
        /// Formats a date using the current culture's long date pattern.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>The formatted date string.</returns>
        public static string FormatDate(DateTime date) => date.ToString("D");

        /// <summary>
        /// Formats a time using the current culture's short time pattern.
        /// </summary>
        /// <param name="time">The time to format.</param>
        /// <returns>The formatted time string.</returns>
        public static string FormatTime(DateTime time) => time.ToString("t");

        /// <summary>
        /// Formats a decimal value as currency using the current culture.
        /// </summary>
        /// <param name="amount">The amount to format.</param>
        /// <returns>The formatted currency string.</returns>
        public static string FormatCurrency(decimal amount) => amount.ToString("C", CultureInfo.CurrentCulture);

        /// <summary>
        /// Capitalizes the first character of a string.
        /// </summary>
        /// <param name="text">The text to capitalize.</param>
        /// <returns>The input text with its first character capitalized, or empty string if input is null.</returns>
        public static string Capitalize(string? text) =>
            text?.Length > 0 ? char.ToUpper(text[0]) + text[1..] : text ?? string.Empty;
    }
}