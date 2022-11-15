using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.GroupRecord;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.MetricalData.Impl;

public class GroupRecordService : IGroupRecordService
{
    private readonly IRepository<DataRecord> _drRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<GroupRecord> _grRepo;

    public GroupRecordService(IRepository<GroupRecord> grRepo, IRepository<DataRecord> drRepo, IRepository<Group> gRepo)
    {
        _grRepo = grRepo;
        _drRepo = drRepo;
        _gRepo = gRepo;
    }

    public GroupInfoDto GetGroupRecords(GroupRecordQueryInfoDto dto, out int total)
    {
        var result = new GroupInfoDto();
        var groupRecords = new List<GroupRecordInfoDto>();
        if (!string.IsNullOrEmpty(dto.GroupId))
        {
            var groupId = int.Parse(dto.GroupId);
            var group = _gRepo.Get(groupId);
            if (!string.IsNullOrEmpty(group.FromRecords))
                result.FromRecords = JsonConvert.DeserializeObject<List<GroupRecordInfoDto>>(group.FromRecords);
        }

        var data = _grRepo.All().AsNoTracking();

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.MeasureTypeId))
        {
            var typeId = int.Parse(dto.MeasureTypeId);
            data = data.Where(c => c.MeasureTypeId == typeId);
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

        if (!string.IsNullOrEmpty(dto.BeginTime) && !string.IsNullOrEmpty(dto.EndTime))
        {
            var begin = Convert.ToDateTime(dto.BeginTime);
            var end = Convert.ToDateTime(dto.EndTime);
            data = data.Where(c => c.BeginTime.Date >= begin && c.EndTime.Date <= end);
        }

        total = data.Count();

        var tempGroupList = data.Include(c => c.Specification)
            .Include(c => c.Turn)
            .Include(c => c.Machine)
            .Include(c => c.MeasureType).OrderByDescending(c => c.BeginTime).Skip(dto.Skip()).Take(dto.PageSize).Select(c=> new GroupRecordInfoDto()
            {
                GroupId = c.Id,
                SpecificationName = c.Specification.Name,
                MachineName = c.Machine.Name,
                TurnName = c.Turn.Name,
                TestTime = c.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MeasureTypeName = c.MeasureType.Name,
                IndicatorName = "",
                Count = c.Count
            }).ToList();

        foreach (var gr in tempGroupList)
        {
            var tempDataList = _drRepo.All().Where(c => c.GroupId == gr.GroupId).Select(c => new
            {
                c.Weight,
                c.Circle,
                c.Oval,
                c.Length,
                c.Resistance,
                c.Hardness
            }).ToList();
            var indicatorNames = new List<string>();
            if (tempDataList.Any(c => c.Weight != null)) indicatorNames.Add("重量");

            if (tempDataList.Any(c => c.Circle != null)) indicatorNames.Add("圆周");

            if (tempDataList.Any(c => c.Oval != null)) indicatorNames.Add("圆度");

            if (tempDataList.Any(c => c.Length != null)) indicatorNames.Add("长度");

            if (tempDataList.Any(c => c.Resistance != null)) indicatorNames.Add("吸阻");

            if (tempDataList.Any(c => c.Hardness != null)) indicatorNames.Add("硬度");

            gr.IndicatorName = string.Join(";", indicatorNames);
            groupRecords.Add(gr);
        }
        
        result.GroupRecords = groupRecords;
        // result.GroupRecords = tempGroupList;
        return result;
    }
}