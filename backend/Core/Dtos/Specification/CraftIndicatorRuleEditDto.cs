using System.Collections.Generic;

namespace Core.Dtos.Specification;

public class CraftIndicatorEditDto
{
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
    public IEnumerable<Rule> Rules { get; set; }
}