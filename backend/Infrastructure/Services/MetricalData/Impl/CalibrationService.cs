using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Calibration;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.MetricalData.Impl;

[AutoInject(typeof(ICalibrationService), InjectType.Scope)]
public class CalibrationService: ICalibrationService
{
    private readonly IRepository<Calibration> _calibrationRepo;

    public CalibrationService(IRepository<Calibration> calibrationRepo)
    {
        _calibrationRepo = calibrationRepo;
    }
    public IEnumerable<CalibrationInfoDto> GetCalibrations(BaseQueryInfoDto dto, out int total)
    {
        var data = _calibrationRepo.All().AsNoTracking();

        total = data.Count();

        var result = data.OrderByDescending(c => c.Time).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new CalibrationInfoDto()
            {
                Id = c.Id, Time = c.Time.ToString("yyyy-MM-dd HH:mm:ss"), Instance = c.Instance,
                EquipmentTypeName = c.EquipmentType.toDescription(), Operation = c.Operation,
                Unit = c.Unit, UnitType = c.UnitType, State = c.ResultCode == 1,
                Description = c.Description
            }).ToList();

        return result;
    }
}