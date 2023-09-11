using System.Collections.Generic;
using Core.Dtos.Equipment;
using Core.SugarEntities;

namespace Infrastructure.Services.System;

public interface IEquipmentService
{
    IEnumerable<EquipmentDto> GetEquipments();
    bool AddEquipment(Equipment equipment);
    bool Offline(string instance);
}