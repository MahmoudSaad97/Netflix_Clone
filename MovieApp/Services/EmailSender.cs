using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;

namespace WebPWrecover.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        //string? fromEmail = Options.SenderEmail;
        //string? fromName = Options.SenderName;
        //string? apiKey = Options.ApiKey;
        //var sendGridClient = new SendGridClient(apiKey);
        //var from = new EmailAddress(fromEmail, fromName);
        //var to = new EmailAddress(toEmail);
        //var plainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", "");
        //var msg = MailHelper.CreateSingleEmail(from, to, subject,
        //plainTextContent, message);
        //var response = await sendGridClient.SendEmailAsync(msg);

        if (string.IsNullOrEmpty(Options.ApiKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Options.ApiKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(Options.SenderEmail, Options.SenderName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}