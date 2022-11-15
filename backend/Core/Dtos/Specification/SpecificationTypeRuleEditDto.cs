using System.Collections.Generic;

namespace Core.Dtos.Specification;

public class SpecificationTypeRuleEditDto
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public IEnumerable<Rule> Rules { get; set; }
}