using Core.Dtos.Statistics;

namespace Infrastructure.Services.Statistics;

public interface IFilterStandardDeviationStatisticService
{
    FilterStandardDeviationInfoDto Search(StatisticQueryInfoDto dto);
}