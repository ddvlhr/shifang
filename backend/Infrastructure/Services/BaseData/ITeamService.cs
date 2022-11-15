using System.Collections;
using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.BaseData;

public interface ITeamService
{
    IEnumerable<BaseTableDto> GetTeams(BaseQueryInfoDto dto, out int total);
    bool Edit(BaseEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
}