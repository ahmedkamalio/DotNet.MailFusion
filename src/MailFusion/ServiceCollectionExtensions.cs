using MailFusion.Providers;
using MailFusion.Templates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MailFusion;

/// <summary>
/// Provides extension methods for IServiceCollection to configure email services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures MailFusion services to the service collection based on application configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration instance containing email service settings.</param>
    /// <param name="environment">The web hosting environment.</param>
    /// <param name="configure">Optional action to configure the MailFusion builder.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///   <item><description>Binds email configuration from the "Email" section</description></item>
    ///   <item><description>Registers IEmailService with EmailService implementation</description></item>
    ///   <item><description>Configures email providers based on configuration or custom implementation</description></item>
    ///   <item><description>Registers template engine and loader services</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Basic usage:
    /// <code>
    /// services.AddMailFusion(Configuration, Environment);
    /// </code>
    /// 
    /// Using custom implementations:
    /// <code>
    /// services.AddMailFusion(Configuration, Environment, builder =>
    /// {
    ///     builder
    ///         .UseCustomEmailProvider&lt;MyEmailProvider&gt;()
    ///         .UseCustomTemplateEngine&lt;MyTemplateEngine&gt;()
    ///         .UseCustomTemplateLoader&lt;MyTemplateLoader&gt;();
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddMailFusion(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        Action<MailFusionBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Configure email options
        var emailOptions = configuration.GetSection(EmailOptions.SectionName);
        if (!emailOptions.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{EmailOptions.SectionName}' not found.");
        }

        services.Configure<EmailOptions>(emailOptions);

        // Configure template options with defaults if not present
        var templateOptions = configuration.GetSection(EmailTemplateOptions.SectionName);
        if (templateOptions.Exists())
        {
            services.Configure<EmailTemplateOptions>(templateOptions);
        }
        else
        {
            // Configure default template options
            services.Configure<EmailTemplateOptions>(options =>
            {
                options.Provider = "File";
                options.File = new FileTemplateOptions
                {
                    TemplatesPath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email")
                };
            });
        }

        // Register core services with default implementations
        services.AddScoped<IEmailService, EmailService>();
        services.TryAddScoped<IEmailTemplateEngine, ScribanEmailTemplateEngine>();
        services.TryAddScoped<IEmailTemplateLoader, FileTemplateLoader>();

        // Create and configure the builder
        var builder = new MailFusionBuilder(services, configuration, environment);

        // Invoke custom configure function if provided
        configure?.Invoke(builder);

        // Configure the default provider if no custom provider was specified
        builder.ConfigureDefaultProvider();

        return services;
    }
}

/// <summary>
/// Builder class for configuring MailFusion services.
/// </summary>
/// <remarks>
/// This builder allows for customization of email providers, template engines, and template loaders.
/// It provides methods to replace the default implementations with custom ones.
/// </remarks>
public class MailFusionBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private bool _providerConfigured;

    internal MailFusionBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _services = services;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Uses a custom email provider implementation instead of the default providers.
    /// </summary>
    /// <typeparam name="TProvider">The type implementing IEmailProvider.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    /// <remarks>
    /// This will override any provider configured in appsettings.json.
    /// </remarks>
    public MailFusionBuilder UseCustomEmailProvider<TProvider>() where TProvider : class, IEmailProvider
    {
        _services.RemoveAll<IEmailProvider>();
        _services.AddScoped<IEmailProvider, TProvider>();
        _providerConfigured = true;
        return this;
    }

    /// <summary>
    /// Uses a custom template engine implementation instead of the default Scriban engine.
    /// </summary>
    /// <typeparam name="TEngine">The type implementing IEmailTemplateEngine.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public MailFusionBuilder UseCustomTemplateEngine<TEngine>() where TEngine : class, IEmailTemplateEngine
    {
        _services.RemoveAll<IEmailTemplateEngine>();
        _services.AddScoped<IEmailTemplateEngine, TEngine>();
        return this;
    }

    /// <summary>
    /// Uses a custom template loader implementation instead of the default file system loader.
    /// </summary>
    /// <typeparam name="TLoader">The type implementing IEmailTemplateLoader.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public MailFusionBuilder UseCustomTemplateLoader<TLoader>() where TLoader : class, IEmailTemplateLoader
    {
        _services.RemoveAll<IEmailTemplateLoader>();
        _services.AddScoped<IEmailTemplateLoader, TLoader>();
        return this;
    }

    /// <summary>
    /// Configures custom service registration logic for any of the MailFusion services.
    /// </summary>
    /// <param name="configure">Action to configure services.</param>
    /// <returns>The builder instance for chaining.</returns>
    public MailFusionBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    internal void ConfigureDefaultProvider()
    {
        if (_providerConfigured) return;

        var emailOptions = _configuration.GetSection(EmailOptions.SectionName);
        var emailProvider = emailOptions.Get<EmailOptions>()?.Provider;

        // If we're in development and no provider is specified, default to the development provider
        if (string.IsNullOrEmpty(emailProvider) && _environment.IsDevelopment())
        {
            emailProvider = SupportedProviders.Development;
        }

        switch (emailProvider)
        {
            case SupportedProviders.SendGrid:
                _services.AddScoped<IEmailProvider, SendGridEmailProvider>();
                break;

            case SupportedProviders.AmazonSes:
                _services.AddScoped<IEmailProvider, AmazonSesEmailProvider>();
                break;

            case SupportedProviders.Development:
                if (!_environment.IsDevelopment())
                {
                    throw new InvalidOperationException(
                        "The development email provider can only be used in the development environment");
                }

                _services.AddScoped<IEmailProvider, DevelopmentEmailProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Invalid email provider specified \"{emailProvider}\". Valid providers are: " +
                    $"{SupportedProviders.SendGrid}, {SupportedProviders.AmazonSes}, {SupportedProviders.Development}");
        }
    }
}