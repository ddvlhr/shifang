namespace Core.Dtos.Report;

public class AddReportDto
{
    public int GroupId { get; set; }
    public string Water { get; set; }
    public string Humidity { get; set; }
    public string Temperature { get; set; }
    public int PhyRet { get; set; }
    public string PhyRetDes { get; set; }
    public string Remark { get; set; }
    public int FinalRet { get; set; }
}