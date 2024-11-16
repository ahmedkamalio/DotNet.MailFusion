using MailFusion.Templates;
using ResultObject;

namespace MailFusion;

/// <summary>
/// Defines the core interface for sending emails via various email service providers.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="message">The email message to send, containing subject, body, sender, and recipient information.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a result that indicates success or failure.
    /// If the operation fails, the result will contain detailed error information.
    /// </returns>
    /// <remarks>
    /// This method handles direct email sending without template processing. For template-based emails,
    /// use <see cref="SendFromTemplateAsync{TModel}"/> instead.
    /// </remarks>
    Task<IResult<Unit>> SendAsync(EmailMessage message);

    /// <summary>
    /// Sends an email using a template, processing it with the provided model data.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model, which must implement IEmailTemplateModel.</typeparam>
    /// <param name="templateName">The name of the template to use for generating the email content.</param>
    /// <param name="templateModel">The model containing data to be used in the template.</param>
    /// <param name="sender">The sender information for the email.</param>
    /// <param name="recipients">The list of recipients for the email.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a result that indicates success or failure.
    /// If the operation fails, the result will contain detailed error information about template processing or sending.
    /// </returns>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///   <item><description>Loads and processes the specified template with the provided model</description></item>
    ///   <item><description>Generates both HTML and plain text versions of the email</description></item>
    ///   <item><description>Sends the processed email to the specified recipients</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var model = new WelcomeEmailModel 
    /// { 
    ///     Subject = "Welcome!",
    ///     Email = "user@example.com",
    ///     UserName = "John Doe"
    /// };
    /// var sender = new EmailSender 
    /// {
    ///     Name = "My App",
    ///     Email = "noreply@myapp.com",
    ///     ReplyEmail = "support@myapp.com"
    /// };
    /// var recipients = new[] { new EmailRecipient { Email = "user@example.com", Name = "John Doe" } };
    /// 
    /// var result = await emailService.SendFromTemplateAsync("welcome-email", model, sender, recipients);
    /// </code>
    /// </example>
    Task<IResult<Unit>> SendFromTemplateAsync<TModel>(
        string templateName,
        TModel templateModel,
        EmailSender sender,
        IList<EmailRecipient> recipients
    ) where TModel : IEmailTemplateModel;
}