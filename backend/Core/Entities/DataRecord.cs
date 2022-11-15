using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_data_record")]
public class DataRecord : BaseData
{
    [Column("weight")] public double? Weight { get; set; }

    [Column("circle")] public double? Circle { get; set; }

    [Column("oval")] public double? Oval { get; set; }

    [Column("length")] public double? Length { get; set; }

    [Column("resistance")] public double? Resistance { get; set; }

    [Column("resistance_open")] public double? ResistanceOpen { get; set; }

    [Column("hardness")] public double? Hardness { get; set; }

    [Column("ventilation_filter")] public double? VentilationFilter { get; set; }

    [Column("ventilation_cigarette")] public double? VentilationCigarette { get; set; }

    [Column("ventilation_total")] public double? VentilationTotal { get; set; }

    [Column("count")] public int Count { get; set; }
}