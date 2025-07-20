using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailAddress = _configuration["EmailSettings:Email"];
        var emailPassword = _configuration["EmailSettings:Password"];

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(emailAddress, emailPassword),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailAddress, "YourAppName"),
            Subject = subject,

            Body = htmlMessage,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        return smtpClient.SendMailAsync(mailMessage);
    }
}
