using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_volume_pick_up")]
[Comment("卷接机")]
public class VolumePickUp: BaseDataEntity
{
    
}