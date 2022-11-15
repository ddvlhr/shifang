using Core.Dtos.ProductWaterStatistic;

namespace Infrastructure.Services.Statistics;

public interface IProductWaterStatisticService
{
    ProductWaterStatisticInfoDto GetInfo(ProductWaterStatisticQueryInfoDto dto);
}