using System.Collections.Generic;

namespace Core.Dtos.Specification;

public class MeasureTypeIndicatorsEditDto
{
    public int Id { get; set; }
    public IEnumerable<IndicatorRule> IndicatorRules { get; set; }

    public class IndicatorRule
    {
        public int DbId { get; set; }
        public int Id { get; set; }
        public int SpecificationTypeId { get; set; }
        public string Points { get; set; }
        public string Deduction { get; set; }
        public string MeanPoints { get; set; }
        public string MeanDeduction { get; set; }
        public string SdPoints { get; set; }
        public string SdDeduction { get; set; }
        public string CvPoints { get; set; }
        public string CvDeduction { get; set; }
        public string UnQualifiedCount { get; set; }
        public int UnQualifiedOperator { get; set; }
    }
}