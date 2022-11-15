using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Calibration;

namespace Infrastructure.Services.MetricalData;

public interface ICalibrationService
{
    IEnumerable<CalibrationInfoDto> GetCalibrations(BaseQueryInfoDto dto, out int total);
}