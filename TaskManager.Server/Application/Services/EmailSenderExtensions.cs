using System.Globalization;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Models;
using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Application.Services
{
    public static class EmailSenderExtensions
    {
        public static async Task InviteToTeamEmailSendAsync(
            this IEmailSender emailSender,
            TokenResponse token,
            User user,
            Team team)
        {
            DateTime utcDate = DateTimeOffset.FromUnixTimeMilliseconds(token.ExpiresAt).UtcDateTime;
            string formattedUtcDate = utcDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " UTC";
            var configuration = emailSender.GetConfiguration();

            string emailBody = $@"
            <html>
            <body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
                <h2 style=""color: #007BFF;"">Hi {user.Name},</h2>
                <p>The <strong>{team.NameTeam}</strong> team invites you to join their team. To accept the invitation, please click the button below:</p>
                <a href=""{configuration.GetValue<string>("BaseUrl")}/team/jointoteam/{token.Token}"" 
                   style=""display: inline-block; padding: 10px 20px; margin: 10px 0; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold;"">
                   Accept Invitation
                </a>
                <p>Please note, this invitation is valid until <strong>{formattedUtcDate}</strong>.</p>
                <p>Best regards,<br/>The Task Manager</p>
            </body>
            </html>";

            await emailSender.SendEmailAsync(
                user.Email,
                $"Hi {user.Name}, You have been invited to join the team {team.NameTeam}!",
                emailBody
            );
        }
    }
}
