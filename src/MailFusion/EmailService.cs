using MailFusion.Templates;
using Microsoft.Extensions.Logging;
using ResultObject;

namespace MailFusion;

/// <summary>
/// Implements the core email service functionality, providing methods for sending both direct 
/// and template-based emails through configured email providers.
/// </summary>
/// <remarks>
/// <para>
/// The EmailService acts as the main entry point for sending emails in the application. It coordinates
/// between the template engine and email provider to process and deliver email messages.
/// </para>
/// <para>
/// The service supports:
/// <list type="bullet">
///   <item><description>Direct email sending with pre-formatted content</description></item>
///   <item><description>Template-based emails with dynamic content</description></item>
///   <item><description>Multiple email providers (SendGrid, Amazon SES, etc.)</description></item>
///   <item><description>Error handling and logging</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Basic usage with dependency injection:
/// <code>
/// public class MyService
/// {
///     private readonly IEmailService _emailService;
///     
///     public MyService(IEmailService emailService)
///     {
///         _emailService = emailService;
///     }
///     
///     public async Task SendWelcomeEmail(string userEmail, string userName)
///     {
///         var model = new WelcomeEmailModel
///         {
///             Subject = "Welcome!",
///             UserName = userName,
///             Email = userEmail
///         };
///         
///         var sender = new EmailSender 
///         {
///             Name = "My App",
///             Email = "noreply@myapp.com",
///             ReplyEmail = "support@myapp.com"
///         };
///         
///         var recipients = new[] 
///         { 
///             new EmailRecipient { Email = userEmail, Name = userName } 
///         };
///         
///         var result = await _emailService.SendFromTemplateAsync(
///             "welcome-email",
///             model,
///             sender,
///             recipients);
///             
///         if (result.IsFailure)
///         {
///             // Handle error
///         }
///     }
/// }
/// </code>
/// </example>
public class EmailService(
    IEmailProvider emailProvider,
    IEmailTemplateEngine templateEngine,
    ILogger<EmailService> logger) : IEmailService
{
    /// <summary>
    /// Sends a pre-formatted email message through the configured email provider.
    /// </summary>
    /// <param name="message">The complete email message to send.</param>
    /// <returns>
    /// A result indicating success or failure of the send operation.
    /// If the operation fails, the result will contain detailed error information.
    /// </returns>
    /// <remarks>
    /// This method is suitable when you have pre-formatted content and don't need
    /// template processing. For template-based emails, use <see cref="SendFromTemplateAsync{TModel}"/>.
    /// </remarks>
    public async Task<IResult<Unit>> SendAsync(EmailMessage message) => await emailProvider.SendEmailAsync(message);

    /// <summary>
    /// Sends an email using a template, processing it with the provided model data.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model, which must implement <see cref="IEmailTemplateModel"/>.</typeparam>
    /// <param name="templateName">The name of the template to use for generating the email content.</param>
    /// <param name="templateModel">The model containing data to be used in the template.</param>
    /// <param name="sender">The sender information for the email.</param>
    /// <param name="recipients">The list of recipients for the email.</param>
    /// <returns>
    /// A result indicating success or failure of the template processing and send operation.
    /// If the operation fails, the result will contain detailed error information.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs several steps:
    /// <list type="number">
    ///   <item><description>Validates the input parameters</description></item>
    ///   <item><description>Loads and processes the email template</description></item>
    ///   <item><description>Creates the email message with processed content</description></item>
    ///   <item><description>Sends the email through the configured provider</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Failures can occur at any step and will be reflected in the returned result.
    /// The operation is logged for monitoring and debugging purposes.
    /// </para>
    /// </remarks>
    /// <example>
    /// Using a custom template model:
    /// <code>
    /// public class OrderConfirmationModel : IEmailTemplateModel
    /// {
    ///     public string Subject => "Order Confirmation";
    ///     public string Email { get; init; }
    ///     public string OrderNumber { get; init; }
    ///     public decimal TotalAmount { get; init; }
    /// }
    /// 
    /// // Sending the email
    /// var result = await emailService.SendFromTemplateAsync(
    ///     "order-confirmation",
    ///     new OrderConfirmationModel 
    ///     {
    ///         Email = customer.Email,
    ///         OrderNumber = "ORD-123",
    ///         TotalAmount = 99.99m
    ///     },
    ///     sender,
    ///     recipients
    /// );
    /// </code>
    /// </example>
    /// <exception cref="Exception">
    /// An unexpected error occurred during template processing or email sending.
    /// The exception will be caught, logged, and wrapped in an error result.
    /// </exception>
    public async Task<IResult<Unit>> SendFromTemplateAsync<TModel>(
        string templateName,
        TModel templateModel,
        EmailSender sender,
        IList<EmailRecipient> recipients
    ) where TModel : IEmailTemplateModel
    {
        try
        {
            // Validate inputs
            var validationError = ValidateInputs(templateName, recipients);
            if (validationError != null)
            {
                return Result.Failure<Unit>(validationError);
            }

            // Load and process template
            var templateResult = await templateEngine.LoadTemplateAsync(templateName, templateModel);
            if (templateResult.IsFailure)
            {
                logger.LogError("Template processing failed for template {TemplateName}: {Error}",
                    templateName, templateResult.Error);

                return Result.Failure<Unit>(
                    new ResultError(
                        EmailServiceErrors.Codes.TemplateError,
                        EmailServiceErrors.Reasons.TemplateError,
                        EmailServiceErrors.Messages.TemplateError,
                        ErrorCategory.Internal,
                        templateResult.Error
                    )
                );
            }

            var template = templateResult.Value;

            // Create email message
            var message = new EmailMessage
            {
                Subject = template.Subject,
                HtmlBody = template.HtmlBody,
                PlainTextBody = template.PlainTextBody,
                Sender = sender,
                Recipients = recipients
            };

            // Send email
            return await emailProvider.SendEmailAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while sending templated email. Template: {TemplateName}",
                templateName);

            return Result.Failure<Unit>(
                new ResultError(
                    EmailServiceErrors.Codes.UnexpectedError,
                    EmailServiceErrors.Reasons.UnexpectedError,
                    EmailServiceErrors.Messages.UnexpectedError,
                    ErrorCategory.Internal
                )
            );
        }
    }

    /// <summary>
    /// Validates the input parameters for template-based email sending.
    /// </summary>
    /// <param name="templateName">The name of the template to validate.</param>
    /// <param name="recipients">The list of recipients to validate.</param>
    /// <returns>A ResultError if validation fails, null if validation succeeds.</returns>
    /// <remarks>
    /// Validates that:
    /// <list type="bullet">
    ///   <item><description>Template name is not null or empty</description></item>
    ///   <item><description>Recipients list is not empty</description></item>
    /// </list>
    /// </remarks>
    private static ResultError? ValidateInputs(string templateName, IList<EmailRecipient> recipients)
    {
        if (string.IsNullOrEmpty(templateName))
        {
            return new ResultError(
                EmailServiceErrors.Codes.InvalidInput,
                EmailServiceErrors.Reasons.InvalidInput,
                EmailServiceErrors.Messages.InvalidTemplateName,
                ErrorCategory.Validation
            );
        }

        if (recipients.Count == 0)
        {
            return new ResultError(
                EmailServiceErrors.Codes.InvalidInput,
                EmailServiceErrors.Reasons.InvalidInput,
                EmailServiceErrors.Messages.InvalidRecipients,
                ErrorCategory.Validation
            );
        }

        return null;
    }
}