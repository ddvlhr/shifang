using Core.Dtos.GroupRecord;

namespace Infrastructure.Services.MetricalData;

public interface IGroupRecordService
{
    GroupInfoDto GetGroupRecords(GroupRecordQueryInfoDto dto, out int total);
}