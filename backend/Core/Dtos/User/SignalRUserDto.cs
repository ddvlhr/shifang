namespace Core.Dtos.User;

public class SignalRUserDto
{
    public string ConnectionId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Token { get; set; }
}