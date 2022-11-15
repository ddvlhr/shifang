using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

/// <summary>
///     机型
/// </summary>
[Table("t_machine_model")]
public class MachineModel : BaseDataEntity
{
    [ForeignKey(nameof(WorkShopId))] public WorkShop WorkShop { get; set; }

    [Column("work_shop_id")] public int WorkShopId { get; set; }

    public Model Model { get; set; }

    [Column("model_id")] public int ModelId { get; set; }
}