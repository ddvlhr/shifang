using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Models;

namespace Infrastructure.Services.BaseData;

public interface IDisciplineClassService
{
    Task<PageViewModel<BaseTableDto>> GetTableAsync(BaseQueryInfoDto dto);
    Task<ResultViewModel<BaseEditDto>> GetByIdAsync(int id);
    Task<ResultViewModel<BaseEditDto>> EditAsync(BaseEditDto dto);
    Task<IEnumerable<BaseOptionDto>> GetOptionsAsync();
}