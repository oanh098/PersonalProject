using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PersonalProject.Services;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var SendGridApiKey = _configuration["SendGrid:ApiKey"];
        var SendGridSenderEmail = _configuration["SendGrid:SenderEmail"];
        var SendGridSenderName = _configuration["SendGrid:SenderName"] ?? "PersonalProject";

        if (string.IsNullOrEmpty(SendGridApiKey))
        {
            _logger.LogError("SendGrid API key is not configured.");
            throw new InvalidOperationException("SendGrid API key is not configured.");
        }
        if (string.IsNullOrEmpty(SendGridSenderEmail))
        {
            _logger.LogError("SendGrid sender email is not configured.");
            throw new InvalidOperationException("SendGrid sender email is not configured.");
        }
        var client = new SendGridClient(SendGridApiKey);
        var from = new EmailAddress(SendGridSenderEmail, SendGridSenderName);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: "", htmlContent: htmlMessage);
        try
        {
            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            else
            {
                _logger.LogError("Failed to send email. Status code: {StatusCode}, Response: {Response}",
                    response.StatusCode, await response.Body.ReadAsStringAsync());
                throw new InvalidOperationException("Failed to send email.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending email.");
            throw;
        }
    }
}
