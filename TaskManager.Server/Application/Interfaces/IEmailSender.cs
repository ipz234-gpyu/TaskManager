using TaskManager.Server.Application.Models;
using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Application.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
    IConfiguration GetConfiguration();
}
