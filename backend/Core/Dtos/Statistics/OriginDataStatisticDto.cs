using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos.Specification;

namespace Core.Dtos.Statistics
{
    public class OriginDataStatisticDto
    {
        public class OriginDataStatisticQueryDto
        {
            public string Begin { get; set; }
            public string End { get; set; }
            public string SpecificationTypeId { get; set; }
            public string SpecificationId { get; set; }
        }

        public class OriginDataStatisticDataDto
        {
            public int SpecificationId { get; set; }
            public string SpecificationName { get; set; }
            public string Mean { get; set; }
            public string Rate { get; set; }
            public string Quality { get; set; }
            public string Total { get; set; }
            public string QualityCount { get; set; }
            public List<double> List { get; set; }
            public Rule Rule { get; set; }
        }
    }
}
