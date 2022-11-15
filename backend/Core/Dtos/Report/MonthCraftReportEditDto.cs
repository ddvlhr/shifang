using System.Collections.Generic;
using Core.Models;

namespace Core.Dtos.Report;

public class MonthCraftReportEditDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public string TimeStr { get; set; }
    public string PartName { get; set; }
    public string User { get; set; }
    public double Score { get; set; }
    public List<MonthCraftReportItem> Result { get; set; }
}