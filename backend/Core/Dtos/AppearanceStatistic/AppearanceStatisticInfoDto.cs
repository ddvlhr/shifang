namespace Core.Dtos.AppearanceStatistic;

public class AppearanceStatisticInfoDto
{
    /// <summary>
    ///     外观名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     占比
    /// </summary>
    public string Percent { get; set; }

    /// <summary>
    ///     出现次数
    /// </summary>
    public int Count { get; set; }
}