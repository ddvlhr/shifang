using System.Collections.Generic;

namespace Core.Dtos;

public class BaseTreeDto
{
    public int Id { get; set; }
    public string Label { get; set; }
    public IEnumerable<Child> Children { get; set; }

    public class Child
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int ParentId { get; set; }
    }
}