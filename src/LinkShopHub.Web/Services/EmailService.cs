using Mailjet.Client;
using Mailjet.Client.Resources;

namespace LinkShopHub.Web.Services;

public interface IEmailService
{
    Task SendWelcomeAsync(string email);
    Task SendReceiptAsync(string email, decimal amount);
    Task SendConfirmationAsync(string email, string confirmationUrl);
}

public class EmailService : IEmailService
{
    private readonly IMailjetClient _client;
    private readonly ILogger<EmailService> _log;
    private readonly string _fromEmail = "semjasa0@gmail.com";   // <- change to your address
    private readonly string _fromName = "LinkShopHub";

    public EmailService(IConfiguration config, ILogger<EmailService> log)
    {
        var apiKey = config["Mailjet:ApiKey"];
        var secretKey = config["Mailjet:SecretKey"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(secretKey))
            throw new ArgumentException("Mailjet credentials missing in configuration.");

        _client = new MailjetClient(apiKey, secretKey);
        _log = log;
    }

    public Task SendWelcomeAsync(string email)
    {
        const string subject = "Welcome to LinkShopHub";
        const string text = "Hi! Your account is ready.";
        const string html = "<p>Hi!</p><p>Your account is ready.</p>";
        return SendEmailAsync(email, subject, text, html);
    }

    public Task SendReceiptAsync(string email, decimal amount)
    {
        var subject = "Payment Receipt – LinkShopHub";
        var text = $"Thank you! Your payment of €{amount:F2} has been received.";
        var html = $"<p>Thank you!</p><p>Your payment of <strong>€{amount:F2}</strong> has been received.</p>";
        return SendEmailAsync(email, subject, text, html);
    }

    public Task SendConfirmationAsync(string email, string confirmationUrl)
    {
        _log.LogInformation($"{email}, {confirmationUrl}");
        const string subject = "Confirm your LinkShopHub account";
        const string text = "Please confirm your account by clicking the link in this email.";
        var html = $@"
                <p>Hi there!</p>
                <p>Please confirm your account by clicking the link below:</p>
                <p><a href='{confirmationUrl}' target='_blank'>Confirm Email</a></p>
                <p>If you did not create an account, you can safely ignore this email.</p>
                <br>
                <p>Thanks,<br>LinkShopHub Team</p>";

        _log.LogInformation("Sending confirmation e-mail to {Email} with URL {Url}", email, confirmationUrl);
        return SendEmailAsync(email, subject, text, html);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string textContent, string htmlContent)
    {
        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, _fromEmail)
        .Property(Send.FromName, _fromName)
        .Property(Send.To, toEmail)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, htmlContent)
        .Property(Send.TextPart, textContent);

        MailjetResponse response = await _client.PostAsync(request);

        if (response.IsSuccessStatusCode)
            _log.LogInformation("E-mail to {Email} sent successfully. Status: {Status}", toEmail, response.StatusCode);
        else
        {
            _log.LogError(string.Format("StatusCode: {0}\n", response.StatusCode));
            _log.LogError(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            _log.LogError(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
        }
    }
}
