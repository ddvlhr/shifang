using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_measure_type_indicators")]
public class MeasureTypeIndicators : Entity
{
    [Column("specification_type_id")] public int SpecificationTypeId { get; set; }

    [Column("measure_type_id")] public int MeasureTypeId { get; set; }

    [ForeignKey(nameof(MeasureTypeId))] public MeasureType MeasureType { get; set; }

    [Column("indicator_id")] public int IndicatorId { get; set; }

    [ForeignKey(nameof(IndicatorId))] public Indicator Indicator { get; set; }

    /// <summary>
    ///     单支分值
    /// </summary>
    [Column("points")]
    public int Points { get; set; }

    /// <summary>
    ///     单支单次扣分
    /// </summary>
    [Column("deduction")]
    public int Deduction { get; set; }

    /// <summary>
    ///     平均值分值
    /// </summary>
    [Column("mean_points")]
    public int MeanPoints { get; set; }

    /// <summary>
    ///     平均值单次扣分
    /// </summary>
    [Column("mean_deduction")]
    public int MeanDeduction { get; set; }

    /// <summary>
    ///     SD值分值
    /// </summary>
    [Column("sd_points")]
    public int SdPoints { get; set; }

    /// <summary>
    ///     SD值单次扣分
    /// </summary>
    [Column("sd_deduction")]
    public int SdDeduction { get; set; }

    /// <summary>
    ///     CV值分值
    /// </summary>
    [Column("cv_points")]
    public int CvPoints { get; set; }

    /// <summary>
    ///     CV值单次扣分
    /// </summary>
    [Column("cv_deduction")]
    public int CvDeduction { get; set; }

    /// <summary>
    ///     超过多少数量即为不合格
    /// </summary>
    [Column("unqualified_count")]
    public int UnQualifiedCount { get; set; }

    /// <summary>
    ///     不合格判定符号
    ///     1 大于 2 大于等于
    /// </summary>
    [Column("unqualified_operator")]
    public UnQualifiedOperator UnQualifiedOperator { get; set; }
}

public enum UnQualifiedOperator
{
    None = 0,
    MoreThan = 1,
    MoreAndEqual = 2,
    Percent = 3
}