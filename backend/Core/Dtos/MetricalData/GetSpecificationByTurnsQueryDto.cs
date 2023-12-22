using System.Collections.Generic;

namespace Core.Dtos.MetricalData;

public class GetSpecificationByTurnsQueryDto
{
    public string Begin { get; set; }
    public string End { get; set; }
    public List<int> TurnIds { get; set; }
    public bool IsManual { get; set; } = false;
}