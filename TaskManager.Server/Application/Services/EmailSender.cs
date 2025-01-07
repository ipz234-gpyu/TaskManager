using System.Net.Mail;
using System.Net;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;
using TaskManager.Server.Application.Models;

namespace TaskManager.Server.Application.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ITeamInvitationRepository _invitationRepository;

    public EmailSender(IConfiguration configuration, ITeamInvitationRepository invitationRepository)
    {
        _configuration = configuration;
        _invitationRepository = invitationRepository;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        string fromEmail = "timetrackersana@gmail.com";
        string fromPassword = "ddss ldya nusv suzm";

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

    public async Task SendInviteToTeamEmailAsync(TokenResponse token, User user, Team team)
    {
        DateTime utcDate = DateTimeOffset.FromUnixTimeMilliseconds(token.ExpiresAt).UtcDateTime;
        string formattedUtcDate = utcDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC";

        string emailBody = $@"
    <html>
    <body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
        <h2 style=""color: #007BFF;"">Hi {user.Name},</h2>
        <p>The <strong>{team.NameTeam}</strong> team invites you to join their team. To accept the invitation, please click the button below:</p>
        <a href=""{_configuration.GetValue<string>("BaseUrl")}/team/jointoteam/{token.Token}"" 
           style=""display: inline-block; padding: 10px 20px; margin: 10px 0; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold;"">
           Accept Invitation
        </a>
        <p>Please note, this invitation is valid until <strong>{formattedUtcDate}</strong>.</p>
        <p>Best regards,<br/>The Task Manager</p>
    </body>
    </html>";

        await SendEmailAsync(
            user.Email,
            $"Hi {user.Name}, You have been invited to join the team {team.NameTeam}!",
            emailBody
        );
    }

}
