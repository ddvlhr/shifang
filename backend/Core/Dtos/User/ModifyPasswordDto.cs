namespace Core.Dtos.User;

public class ModifyPasswordDto
{
    public int Id { get; set; }
    public string Origin { get; set; }
    public string New { get; set; }
}