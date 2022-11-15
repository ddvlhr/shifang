using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_report_order")]
public class ReportOrder : BaseDataEntity
{
    public string Remark { get; set; }
}