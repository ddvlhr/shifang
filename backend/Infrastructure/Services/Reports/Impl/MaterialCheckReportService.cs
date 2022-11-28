using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.MaterialCheckReport;
using Core.Dtos.MetricalData;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Services.MetricalData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IMaterialCheckReportService), InjectType.Scope)]
public class MaterialCheckReportService: IMaterialCheckReportService
{
    private readonly IRepository<MaterialCheckReport> _mcRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMetricalDataService _mdService;

    public MaterialCheckReportService(IRepository<MaterialCheckReport> mcRepo,
        IUnitOfWork uow,
        IMetricalDataService mdService)
    {
        _mcRepo = mcRepo;
        _uow = uow;
        _mdService = mdService;
    }
    public IEnumerable<MaterialCheckReportInfoDto> GetReports(MaterialCheckReportQueryInfoDto dto, out int total)
    {
        var data = _mcRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            var begin = Convert.ToDateTime(dto.Begin);
            var end = Convert.ToDateTime(dto.End);
            data = data.Where(c => c.TestDate.Date >= begin && c.TestDate.Date <= end);
        }
        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.TeamId))
        {
            var teamId = int.Parse(dto.TeamId);
            data = data.Where(c => c.TeamId == teamId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineId))
        {
            var machineId = int.Parse(dto.MachineId);
            data = data.Where(c => c.MachineId == machineId);
        }

        if (!string.IsNullOrEmpty(dto.Qualified))
        {
            var qualified = int.Parse(dto.Qualified);
            data = data.Where(c => c.Qualified == (QualifiedStatus)qualified);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (MaterialCheckStatus)state);
        }

        total = data.Count();

        var list = data.OrderByDescending(c => c.TestDate).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new MaterialCheckReportInfoDto()
            {
                Id = c.Id, TestDate = c.TestDate.ToString("yyyy-MM-dd"),
                SpecificationId = c.SpecificationId, SpecificationName = c.Specification.Name,
                TeamId = c.TeamId, TeamName = c.Team.Name, TurnId = c.TurnId, TurnName = c.Turn.Name,
                MachineId = c.MachineId, MachineName = c.Machine.Name, MeasureTypeId = c.MeasureTypeId,
                MeasureTypeName = c.MeasureType.Name, Originator = c.Originator, Operator = c.Originator,
                Qualified = (int)c.Qualified, MaterialCheckStatus = (int)c.Status, GroupId = c.GroupId
            }).ToList();

        foreach (var item in list)
        {
            var dataInfo = _mdService.GetDataInfoByGroupId(item.GroupId);
            item.DataInfo = dataInfo;
        }

        return list;
    }

    public bool EditReport(MaterialCheckReportInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var report = new MaterialCheckReport();
        if (modify)
        {
            report = _mcRepo.Get(dto.Id);
        }

        if (!modify)
        {
            report.TestDate = Convert.ToDateTime(dto.TestDate);
            report.SpecificationId = dto.SpecificationId;
            report.TeamId = dto.TeamId;
            report.TurnId = dto.TurnId;
            report.MachineId = dto.MachineId;
            report.MeasureTypeId = dto.MeasureTypeId;
            report.Originator = dto.Originator;
            report.Qualified = QualifiedStatus.Undefined;
            report.Status = MaterialCheckStatus.Undetected;
        } 
        else if (report.Status is MaterialCheckStatus.Undetected or MaterialCheckStatus.Rejected)
        {
            if (!string.IsNullOrEmpty(dto.Data))
            {
                var groupInfo = new MetricalDataGroupEditDto()
                {
                    Id = dto.GroupId,
                    TestTime = dto.TestDate,
                    SpecificationId = dto.SpecificationId,
                    TurnId = dto.TurnId,
                    MeasureTypeId = dto.MeasureTypeId,
                    TeamId = dto.TeamId,
                    MachineId = dto.MachineId,
                    ProductionTime = "",
                    DeliverTime = ""
                };
                if (dto.GroupId > 0)
                {
                    var dataInfo = new MetricalDataEditDataDto()
                    {
                        GroupId = dto.GroupId,
                        DataInfo = dto.Data
                    };
                    var ret = _mdService.UpdateDataInfo(dataInfo, out var dataMessage);
                    if (!ret)
                    {
                        message = dataMessage;
                        return false;
                    }
                }
                else
                {
                    if (_mdService.UpdateGroupInfo(groupInfo, out var groupId, out var groupMessage))
                    {
                        report.GroupId = groupId;
                        var dataInfo = new MetricalDataEditDataDto()
                        {
                            GroupId = groupId,
                            DataInfo = dto.Data
                        };
                        var ret = _mdService.UpdateDataInfo(dataInfo, out var dataMessage);
                        if (!ret)
                        {
                            message = dataMessage;
                            return false;
                        }
                    }
                    else
                    {
                        message = groupMessage;
                        return false;
                    }
                }
            }
            else
            {
                report.GroupId = dto.GroupId;
            }
        }
        
        if (modify) 
            _mcRepo.Update(report);
        else
            _mcRepo.Add(report);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = "";
        return result;
    }

    public bool RemoveReports(List<int> ids, out string message)
    {
        var reports = _mcRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        _mcRepo.DeleteRange(reports);
        var result = _uow.Save() == ids.Count;
        message = result ? "删除成功" : "删除失败";
        return result;
    }
}