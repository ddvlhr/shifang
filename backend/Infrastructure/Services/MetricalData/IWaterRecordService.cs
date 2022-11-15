using Core.Dtos.MetricalData;

namespace Infrastructure.Services.MetricalData;

public interface IWaterRecordService
{
    bool Add(WaterRecordEditDto dto, out string water, out string failReason);
    WaterRecordEditDto GetWaterRecordByGroupId(int groupId);
}