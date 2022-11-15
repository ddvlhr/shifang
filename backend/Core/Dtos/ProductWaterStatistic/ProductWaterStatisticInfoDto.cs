using System.Collections.Generic;

namespace Core.Dtos.ProductWaterStatistic;

public class ProductWaterStatisticInfoDto
{
    public List<string> TimeX { get; set; }
    public List<string> Rates { get; set; }
    public IEnumerable<ProductWaterInfo> ProductWaterInfos { get; set; }

    public class ProductWaterInfo
    {
        public string SpecificationName { get; set; }
        public List<string[]> WaterInfos { get; set; }
    }
}