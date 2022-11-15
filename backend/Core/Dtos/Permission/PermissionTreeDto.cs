using System.Collections.Generic;

namespace Core.Dtos.Permission;

public class PermissionTreeDto
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Icon { get; set; }
    public int PermissionType { get; set; }
    public List<MenuItem> Children { get; set; }
    public int Order { get; set; }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Path { get; set; }
        public string Component { get; set; }
        public int Order { get; set; }
        public int PermissionType { get; set; }
        public List<FunctionItem> Children { get; set; }
    }

    public class FunctionItem
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string FunctionName { get; set; }
        public int ButtonType { get; set; }
        public string ButtonTypeName { get; set; }
        public int ButtonPosition { get; set; }
        public int PermissionType { get; set; }
        public int Order { get; set; }
    }
}