# MailFusion

[![NuGet](https://img.shields.io/nuget/v/MailFusion.svg)](https://www.nuget.org/packages/MailFusion/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

<img src="https://raw.githubusercontent.com/ahmedkamalio/DotNet.MailFusion/main/icon.png" alt="MailFusion Icon" width="100" height="100">

A modern, flexible email delivery library for .NET that simplifies sending emails through various providers like
SendGrid and Amazon SES. MailFusion offers robust template support, comprehensive error handling, and a clean,
strongly-typed API.

## Features

- 📧 Multiple Email Provider Support
    - SendGrid
    - Amazon SES
    - Development (console output for testing)
    - Custom Email provider support
- 📝 Template Support
    - Scriban template engine integration
    - HTML and plain text support
    - Template caching for performance
    - Strong typing for template models
    - Custom template engine support
    - Custom template loader support
- ⚡ Modern .NET Features
    - Async/await throughout
    - Nullable reference types
    - Record types for immutable data
    - Modern C# features
    - Builder pattern configuration
- 🛡️ Robust Error Handling
    - Detailed error information
    - Provider-specific error mapping
    - Strongly-typed error codes
    - Comprehensive logging
- 🧪 Development-Friendly
    - Development provider for testing
    - Detailed debugging information
    - Comprehensive XML documentation
    - Rich logging support
    - Sensible defaults

## Installation

Install MailFusion via NuGet:

```bash
dotnet add package MailFusion
```

## Quick Start

### 1. Basic Configuration

The simplest way to get started is with the basic configuration in your `appsettings.json`:

```json
{
  "Email": {
    "Provider": "SendGrid",
    "SendGrid": {
      "ApiKey": "your-api-key"
    }
  }
}
```

And in your `Program.cs` or `Startup.cs`:

```csharp
services.AddMailFusion(Configuration, Environment);
```

### 2. Advanced Configuration

MailFusion supports extensive customization through the builder pattern:

```csharp
services.AddMailFusion(Configuration, Environment, builder =>
{
    builder
        .UseCustomEmailProvider<MyEmailProvider>()
        .UseCustomTemplateEngine<MyTemplateEngine>()
        .UseCustomTemplateLoader<MyTemplateLoader>();
});
```

### 3. Create an Email Template Model

```csharp
public class WelcomeEmailModel : IEmailTemplateModel
{
    public string Subject => "Welcome to Our Service!";
    public string Email { get; init; }
    public string UserName { get; init; }
}
```

### 4. Send an Email

```csharp
public class EmailService
{
    private readonly IEmailService _emailService;

    public EmailService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmailAsync(string userEmail, string userName)
    {
        var model = new WelcomeEmailModel
        {
            Email = userEmail,
            UserName = userName
        };

        var sender = new EmailSender
        {
            Name = "My Service",
            Email = "noreply@myservice.com",
            ReplyEmail = "support@myservice.com"
        };

        var recipients = new[]
        {
            new EmailRecipient { Email = userEmail, Name = userName }
        };

        var result = await _emailService.SendFromTemplateAsync(
            "welcome-email",
            model,
            sender,
            recipients);

        if (result.IsFailure)
        {
            // Handle error
        }
    }
}
```

## Configuration Options

### Email Provider Configuration

#### SendGrid

```json
{
  "Email": {
    "Provider": "SendGrid",
    "SendGrid": {
      "ApiKey": "your-api-key"
    }
  }
}
```

#### Amazon SES

```json
{
  "Email": {
    "Provider": "AmazonSes",
    "AmazonSes": {
      "AccessKey": "your-access-key",
      "SecretKey": "your-secret-key",
      "Region": "us-east-1"
    }
  }
}
```

#### Development Provider

```json
{
  "Email": {
    "Provider": "Development",
    "Development": {
      "UseColors": true,
      "ShowHtmlBody": true,
      "ShowPlainTextBody": false
    }
  }
}
```

### Template Configuration

Template configuration is optional. If not specified, MailFusion will use these defaults:

- File-based template provider
- Templates directory at `{BaseDirectory}/Templates/Email`

To customize template settings:

```json
{
  "EmailTemplates": {
    "Provider": "File",
    "File": {
      "TemplatesPath": "path/to/templates"
    }
  }
}
```

## Custom Implementations

### Custom Email Provider

```csharp
public class MyEmailProvider : IEmailProvider
{
    public async Task<IResult<Unit>> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        // Custom implementation
    }
}

// Registration
services.AddMailFusion(Configuration, Environment, builder =>
{
    builder.UseCustomEmailProvider<MyEmailProvider>();
});
```

### Custom Template Engine

```csharp
public class MyTemplateEngine : IEmailTemplateEngine
{
    public async Task<IResult<IEmailTemplate>> LoadTemplateAsync<TModel>(
        string templateName, TModel model) where TModel : IEmailTemplateModel
    {
        // Custom implementation
    }
}

// Registration
services.AddMailFusion(Configuration, Environment, builder =>
{
    builder.UseCustomTemplateEngine<MyTemplateEngine>();
});
```

### Custom Template Loader

```csharp
public class MyTemplateLoader : IEmailTemplateLoader
{
    public async Task<IResult<(string html, string text)>> LoadTemplateAsync(string templateName)
    {
        // Custom implementation
    }
}

// Registration
services.AddMailFusion(Configuration, Environment, builder =>
{
    builder.UseCustomTemplateLoader<MyTemplateLoader>();
});
```

## Template System

When using the default file-based template system, organize your templates as follows:

```
Templates/
  └── Email/
      ├── welcome/
      │   ├── welcome.html
      │   └── welcome.txt
      └── order-confirmation/
          ├── order-confirmation.html
          └── order-confirmation.txt
```

### Example Template

HTML template (`welcome.html`):

```html
<h1>Welcome {{ UserName }}!</h1>
<p>Thank you for joining our service.</p>
```

Text template (`welcome.txt`):

```
Welcome {{ UserName }}!

Thank you for joining our service.
```

## Error Handling

MailFusion uses the Result pattern for error handling:

```csharp
var result = await _emailService.SendFromTemplateAsync(...);

if (result.IsFailure)
{
    var error = result.Error;
    logger.LogError("Failed to send email: {ErrorCode} - {ErrorMessage}",
        error.Code,
        error.Message);
}
```

## Development and Testing

For development and testing, use the Development provider:

```json
{
  "Email": {
    "Provider": "Development"
  }
}
```

This will output emails to the console instead of sending them.

## Logging

MailFusion integrates with Microsoft.Extensions.Logging:

```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    // Add other logging providers as needed
});
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [SendGrid](https://sendgrid.com/) - Email service provider
- [Amazon SES](https://aws.amazon.com/ses/) - Email service provider
- [Scriban](https://github.com/scriban/scriban) - Template engine

## Support

For support, please open an issue in the GitHub repository.

---

Made with ❤️ by [Ahmed Kamal](https://github.com/ahmedkamalio)
