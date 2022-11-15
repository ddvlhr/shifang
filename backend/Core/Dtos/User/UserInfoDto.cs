namespace Core.Dtos.User;

public class UserInfoDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string NickName { get; set; }
    public int RoleId { get; set; }
    public bool CanSeeOtherData { get; set; }
    public bool ShowSettings { get; set; } = false;
}