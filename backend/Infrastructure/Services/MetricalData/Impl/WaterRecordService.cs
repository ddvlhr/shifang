using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.MetricalData;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.MetricalData.Impl;

public class WaterRecordService : IWaterRecordService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WaterRecord> _wrRepo;

    public WaterRecordService(IRepository<WaterRecord> wrRepo, IUnitOfWork uow)
    {
        _wrRepo = wrRepo;
        _uow = uow;
    }

    public bool Add(WaterRecordEditDto dto, out string water, out string failReason)
    {
        failReason = string.Empty;
        var list = new List<WaterRecord>();
        var removeList = new List<WaterRecord>();
        var isEdit = false;
        var dbWaterRecordIds = _wrRepo.All().Where(c => c.GroupId == dto.GroupId).Select(c => c.Id).ToList();
        var existIds = dto.Infos.Where(c => c.Id > 0).Select(c => c.Id).ToList();
        var removeIds = dbWaterRecordIds.Where(c => !existIds.Contains(c)).ToList();
        foreach (var info in dto.Infos)
        {
            var item = new WaterRecord();
            if (info.Id > 0)
            {
                isEdit = true;
                item = _wrRepo.Get(info.Id);
            }

            item.GroupId = dto.GroupId;
            item.Before = Convert.ToDouble(info.Before);
            item.After = Convert.ToDouble(info.After);
            item.Water = getWater(info.Before, info.After);
            list.Add(item);
        }

        if (dbWaterRecordIds.Count > 0) removeList = _wrRepo.All().Where(c => removeIds.Contains(c.Id)).ToList();

        water = list.Average(c => c.Water).ToString("F2");
        if (isEdit)
            _wrRepo.UpdateRange(list);
        else
            _wrRepo.AddRange(list);

        if (removeList.Count > 0)
            _wrRepo.DeleteRange(removeList);

        return isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
    }

    public WaterRecordEditDto GetWaterRecordByGroupId(int groupId)
    {
        var data = _wrRepo.All().Where(c => c.GroupId == groupId).ToList();
        if (data.Count > 0)
        {
            var dto = new WaterRecordEditDto
            {
                GroupId = groupId,
                TestTime = data.First().DataTestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Infos = data.Select(c => new WaterRecordEditDto.WaterInfo
                {
                    Id = c.Id,
                    After = c.After.ToString(),
                    Before = c.Before.ToString(),
                    Water = c.Water.ToString
                        ("F2")
                }).ToList()
            };

            return dto;
        }

        return null;
    }

    private double getWater(string before, string after)
    {
        var _before = double.Parse(before);
        var _after = double.Parse(after);
        var result = (_before - _after) / _before * 100;
        return result;
    }

    public class TempWaterRecord
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
    }
}