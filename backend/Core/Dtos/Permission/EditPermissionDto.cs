namespace Core.Dtos.Permission;

public class EditPermissionDto
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string Name { get; set; }
    public int PermissionType { get; set; }
    public string Icon { get; set; }
    public string Path { get; set; }
    public string Component { get; set; }

    public string FunctionName { get; set; }
    public int ButtonType { get; set; }
    public int ButtonPosition { get; set; }
    
    public int Order { get; set; }
}