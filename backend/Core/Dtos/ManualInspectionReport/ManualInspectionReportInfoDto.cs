using System.Collections.Generic;
using Core.Entities;

namespace Core.Dtos.ManualInspectionReport;

public class ManualInspectionReportInfoDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public string Operation { get; set; }
    public int Count { get; set; }
    public List<ManualInspectionReportDefect> Defects { get; set; }

    public List<ManualInspectionReportDefectInfoDto> DefectInfo { get; set; } =
        new List<ManualInspectionReportDefectInfoDto>();
    public int Result { get; set; }
}