using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string to, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var emailPassword = _configuration["EmailPassword"]; // Read the email password from secrets.json

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sadeen", senderEmail));
        message.To.Add(new MailboxAddress("Recipient Name", to));
        message.Subject = subject;

        var text = new TextPart("plain")
        {
            Text = body
        };

        message.Body = text;

        using (var client = new SmtpClient())
        {
            client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Bypass certificate validation
            client.Connect(smtpServer, port, SecureSocketOptions.SslOnConnect);
            client.Authenticate(senderEmail, emailPassword); // Use the email password
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
