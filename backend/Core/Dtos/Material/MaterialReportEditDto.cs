using System;

namespace Core.Dtos.Material;

public class MaterialReportEditDto
{
    public int Id { get; set; }
    public DateTime TestDate { get; set; }
    public string TestDateStr { get; set; }
    public string SpecificationName { get; set; }
    public string OrderNo { get; set; }
    public int ManufacturerId { get; set; }
    public string ManufacturerName { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public string SamplePlace { get; set; }
    public string SampleCount { get; set; }
    public string Unit { get; set; }
    public string Description { get; set; }
    public string OtherDesc { get; set; }
    public string ResultDesc { get; set; }
    public string Result { get; set; }
    public bool ReportRet { get; set; }
    public string Temperature { get; set; }
    public string Humidity { get; set; }
    public string ReportRetResult { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
}