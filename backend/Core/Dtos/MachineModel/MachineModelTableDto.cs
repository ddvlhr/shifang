namespace Core.Dtos.MachineModel;

public class MachineModelTableDto : BaseTableDto
{
    public int WorkShopId { get; set; }
    public string WorkShopName { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
}