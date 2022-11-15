using System.Collections.Generic;
using Core.Dtos.WorkShopQuality;

namespace Infrastructure.Services.BaseData;

public interface IWorShopQualityPointRuleService
{
    IEnumerable<WorkShopQualityPointRuleTableDto> GetTable(WorkShopQualityPointRuleQueryInfoDto dto, out int total);
    bool Add(WorkShopQualityPointRuleEditDto dto, out string failReason);
    bool Edit(WorkShopQualityPointRuleEditDto dto, out string failReason);
    bool Delete(IEnumerable<int> ids);
    WorkShopQualityPointRuleEditDto GetInfo(int id);
}