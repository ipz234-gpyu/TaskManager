namespace TaskManager.Server.Application.Models;

public class HashPasswordResponse
{
    public string Password { get; set; }
    public string Salt { get; set; }
}
