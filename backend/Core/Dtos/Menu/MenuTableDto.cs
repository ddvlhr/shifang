using System.Collections.Generic;

namespace Core.Dtos.Menu;

public class MenuTableDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public int Level { get; set; }
    public bool State { get; set; }
    public List<Child> Children { get; set; }

    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
        public bool State { get; set; }
    }
}