using System.Collections.Generic;

namespace Core.Dtos.MetricalData;

public class WaterRecordEditDto
{
    public int GroupId { get; set; }
    public string TestTime { get; set; }
    public string DataId { get; set; }
    public IEnumerable<WaterInfo> Infos { get; set; }

    public class WaterInfo
    {
        public int Id { get; set; }
        public string After { get; set; }
        public string Before { get; set; }
        public string Water { get; set; }
    }
}