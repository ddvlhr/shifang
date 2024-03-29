﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Dtos.MetricalData;
using Core.Models;
using Newtonsoft.Json.Linq;
using static Core.Dtos.DashboardDto;

namespace Infrastructure.Services.MetricalData;

public interface IMetricalDataService
{
    IEnumerable<MetricalDataTableDto> GetTable(MetricalDataQueryDto dto, out int total);
    bool AddGroupInfo(MetricalDataGroupEditDto dto, out string failReason);
    bool AddDataInfo(MetricalDataEditDataDto dto, out string failReason);
    bool UpdateGroupInfo(MetricalDataGroupEditDto dto, out int groupId, out string failReason);
    bool UpdateDataInfo(MetricalDataEditDataDto dto, out string failReason);
    bool Delete(List<int> ids);
    string GetIndicators(int id);
    List<Dictionary<string, object>> GetDataInfoByGroupId(int groupId);
    string GetDataInfo(int id);
    string GetSpecifications();
    string GetSpecification(int id);
    BaseStatisticInfoDto GetStatisticInfo(int groupId, bool dayStatistic, out string failReason);
    MetricalDataStatisticDto GetStatistic(int id, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
    IEnumerable<BaseOptionDto> GetOptions(int specificationId, DateTime testDate, int type);
    MemoryStream Download(MetricalDataQueryDto dto);
    MemoryStream DownloadStatistic(MetricalDataQueryDto dto);
    MemoryStream DownloadStatisticInfo(MetricalDataQueryDto dto);
    IEnumerable<BaseOptionDto> GetMeasureDataBySpecificationIdAndMeasureTypeId(int specificationId,
        int measureTypeId);

    Task<IEnumerable<MetricalDataInfoDto>> GetMetricalDataInfoAsync(int type);
    Task<IEnumerable<MetricalDataInfoDto>> GetManualMetricalDataInfoAsync(int type, string workShopName);
    List<DashboardDto.ManualCheckerInfo> GetManualCheckerInfos(string workshopName);
    Task<IEnumerable<MaterialDataHandicraftWorkshop>> GetHandicraftWorkshopAsync();
    IEnumerable<MetricalDataTableDto> GetHandicraftWorkshopMatrialData(string WorkShopLetter,int PageSize,int PageNum, out int total);
    ManualDataPushDto GetManualMetricalDataStatistic(string workShopLetter);
    IEnumerable<BaseOptionDto> GetNewestGroupIdsByMachine(NewestGroupIdsQueryDto dto);
    DashboardDto.ManualSummaryInfoDto GetManualSummaryInfo(ManualQueryInfoDto dto);
    IEnumerable<BaseOptionDto> GetSpecificationsByTeamIds(GetSpecificationByTurnsQueryDto dto);
}