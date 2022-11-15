using System.Collections.Generic;
using Core.Dtos.Function;

namespace Core.Dtos.Menu;

public class AsideMenuDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public List<Child> Children { get; set; }

    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IEnumerable<FunctionTableDto> Functions { get; set; }
    }
}