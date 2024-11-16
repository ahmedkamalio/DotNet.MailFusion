using MailFusion.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MailFusion;

/// <summary>
/// Provides extension methods for IServiceCollection to configure email services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures email services to the service collection based on application configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration instance containing email service settings.</param>
    /// <param name="environment">The web hosting environment.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///   <item><description>Binds email configuration from the "Email" section.</description></item>
    ///   <item><description>Registers IEmailService with EmailService implementation.</description></item>
    ///   <item><description>Configures the appropriate email provider based on configuration.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when:
    /// <list type="bullet">
    ///   <item><description>The development provider is used in non-development environment.</description></item>
    ///   <item><description>An invalid or unsupported email provider is specified.</description></item>
    /// </list>
    /// </exception>
    /// <example>
    /// Here's how to use this extension method in Startup.cs:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     services.AddConfiguredEmailService(Configuration, Environment);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddConfiguredEmailService(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var options = configuration.GetSection(EmailOptions.SectionName);
        if (!options.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{EmailOptions.SectionName}' not found.");
        }

        services.Configure<EmailOptions>(options);
        services.AddScoped<IEmailService, EmailService>();

        var emailProvider = options.Get<EmailOptions>()?.Provider;

        // If we're in development and no provider is specified, default to the development provider
        if (string.IsNullOrEmpty(emailProvider) && environment.IsDevelopment())
        {
            emailProvider = SupportedProviders.Development;
        }

        switch (emailProvider)
        {
            case SupportedProviders.SendGrid:
                services.AddScoped<IEmailProvider, SendGridEmailProvider>();
                break;

            case SupportedProviders.AmazonSes:
                services.AddScoped<IEmailProvider, AmazonSesEmailProvider>();
                break;

            case SupportedProviders.Development:
                if (!environment.IsDevelopment())
                {
                    throw new InvalidOperationException(
                        "The development email provider can only be used in the development environment");
                }

                services.AddScoped<IEmailProvider, DevelopmentEmailProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Invalid email provider specified \"{emailProvider}\". Valid providers are: " +
                    $"{SupportedProviders.SendGrid}, {SupportedProviders.AmazonSes}, {SupportedProviders.Development}");
        }

        return services;
    }
}