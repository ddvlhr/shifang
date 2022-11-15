namespace Core.Dtos.Function;

public class EditFunctionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FunctionName { get; set; }
    public string Type { get; set; }
    public int MenuId { get; set; }
    public int Position { get; set; }
    public bool State { get; set; }
}