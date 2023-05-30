using System.Collections.Generic;

namespace Core.Dtos.Specification;

public class SpecificationEditDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string OrderNo { get; set; }
    public string Remark { get; set; }
    public int TypeId { get; set; }
    public int EquipmentTypeId { get; set; }
    public IEnumerable<Rule> SingleRules { get; set; }
    public IEnumerable<Rule> MeanRules { get; set; }
    public IEnumerable<Rule> SdRules { get; set; }
    public IEnumerable<Rule> CvRules { get; set; }
    public IEnumerable<Rule> CpkRules { get; set; }
    public bool State { get; set; }
}

public class Rule
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Standard { get; set; }
    public string Upper { get; set; }
    public string Lower { get; set; }

    /// <summary>
    ///     是否包含等于上下限值
    /// </summary>
    public bool Equal { get; set; } = true;

    public string Points { get; set; }
    public string Deduction { get; set; }
}

public class DoubleRule
{
    public double Standard { get; set; }
    public double Upper { get; set; }
    public double Lower { get; set; }
}