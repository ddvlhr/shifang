using System.Collections.Generic;
using Core.Dtos.Equipment;
using Core.Entities;
using Core.Enums;
using Core.SugarEntities;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;

namespace Infrastructure.Services.System.Impl;

[AutoInject(typeof(IEquipmentService), InjectType.Scope)]
public class EquipmentService: SugarRepository<Equipment>,  IEquipmentService
{
    public IEnumerable<EquipmentDto> GetEquipments()
    {
        var list =  Context.Queryable<Equipment>().Select(c => new EquipmentDto()
        {
            Name = c.Name, Description = c.Description, EquipmentType = c.EquipmentType, Instance = c.Instance,
            Ip = c.Ip, LastConnectTime = c.LastConnectTime.ToString("yyyy-MM-dd HH:mm:ss"), OnlineStatus = c.Status,
            Version = c.Version
        }).ToList();

        foreach (var item in list)
        {
            item.EquipmentTypeDesc = item.EquipmentType.toDescription();
            item.Status = item.OnlineStatus.toDescription();
        }

        return list;
    }

    public bool AddEquipment(Equipment equipment)
    {
        if (base.IsAny(c => c.Instance == equipment.Instance))
        {
            return Context.Updateable<Equipment>()
                .SetColumns(c=>c.Name == equipment.Name)
                .SetColumns(c=>c.Description == equipment.Description)
                .SetColumns(c=>c.EquipmentType == equipment.EquipmentType)
                .SetColumns(c => c.LastConnectTime == equipment.LastConnectTime)
                .SetColumns(c=>c.Ip == equipment.Ip)
                .SetColumns(c=>c.Version == equipment.Version)
                .SetColumns(c=>c.Status == equipment.Status)
                .Where(c => c.Instance == equipment.Instance).ExecuteCommand() >= 0;
        }
        else
        {
            equipment.Id = 0;
            return base.Insert(equipment);
        }
    }

    public bool Offline(string instance)
    {
        return Context.Updateable<Equipment>()
            .SetColumns(c => c.Status == OnlineStatus.Offline)
            .Where(c => c.Instance == instance).ExecuteCommand() > 0;
    }
}