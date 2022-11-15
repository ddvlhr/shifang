namespace Core.Dtos.Menu;

public class EditMenuDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Icon { get; set; }
    public int Level { get; set; }
    public bool State { get; set; }
}