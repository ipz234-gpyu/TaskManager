using System.Net.Mail;
using System.Net;
using TaskManager.Server.Application.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Application.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        string fromEmail = _configuration.GetValue<string>("EmailSettings:FromEmail");
        string fromPassword = _configuration.GetValue<string>("EmailSettings:FromPassword");

        var client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
    }

    public IConfiguration GetConfiguration() => _configuration;
}
