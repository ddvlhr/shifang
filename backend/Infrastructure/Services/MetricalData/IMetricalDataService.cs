using System;
using System.Collections.Generic;
using System.IO;
using Core.Dtos;
using Core.Dtos.MetricalData;

namespace Infrastructure.Services.MetricalData;

public interface IMetricalDataService
{
    IEnumerable<MetricalDataTableDto> GetTable(MetricalDataQueryDto dto, out int total);
    bool AddGroupInfo(MetricalDataGroupEditDto dto, out string failReason);
    bool AddDataInfo(MetricalDataEditDataDto dto, out string failReason);
    bool UpdateGroupInfo(MetricalDataGroupEditDto dto, out string failReason);
    bool UpdateDataInfo(MetricalDataEditDataDto dto, out string failReason);
    bool Delete(List<int> ids);
    string GetIndicators(int id);
    string GetDataInfo(int id);
    string GetSpecifications();
    string GetSpecification(int id);
    MetricalDataStatisticDto GetStatistic(int id, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
    IEnumerable<BaseOptionDto> GetOptions(int specificationId, DateTime testDate, int type);
    MemoryStream Download(MetricalDataQueryDto dto);
    bool AddFilterData(FilterDataAddDto dto, out string failReason);

    IEnumerable<BaseOptionDto> GetMeasureDataBySpecificationIdAndMeasureTypeId(int specificationId,
        int measureTypeId);
}