using SendGrid;
using SendGrid.Helpers.Mail;

namespace LinkShopHub.Web.Services;

public interface IEmailService
{
    Task SendWelcomeAsync(string email);
    Task SendReceiptAsync(string email, decimal amount);
}

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly ISendGridClient _client = new SendGridClient(config["SendGrid:ApiKey"]);

    public async Task SendWelcomeAsync(string email)
    {
        var msg = MailHelper.CreateSingleEmail(
            from: new EmailAddress("noreply@linkshub.me", "LinkShopHub"),
            to: new EmailAddress(email),
            subject: "Welcome to LinkShopHub",
            plainTextContent: "Hi! Your account is ready.",
            htmlContent: "<p>Hi!</p><p>Your account is ready.</p>");
        await _client.SendEmailAsync(msg);
    }

    public async Task SendReceiptAsync(string email, decimal amount)
    {
        var msg = MailHelper.CreateSingleEmail(
            from: new EmailAddress("noreply@linkshub.me", "LinkShopHub"),
            to: new EmailAddress(email),
            subject: "Payment Receipt – LinkShopHub",
            plainTextContent: $"Thank you! Your payment of €{amount:F2} has been received.",
            htmlContent: $"<p>Thank you!</p><p>Your payment of <strong>€{amount:F2}</strong> has been received.</p>");

        await _client.SendEmailAsync(msg);
    }
}
