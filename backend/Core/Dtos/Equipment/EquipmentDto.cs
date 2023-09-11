using System;
using Core.Entities;
using Core.Enums;

namespace Core.Dtos.Equipment;

public class EquipmentDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Instance { get; set; }
    public string Ip { get; set; }
    public string Version { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public string Status { get; set; }
    public string LastConnectTime { get; set; }
    public EquipmentType EquipmentType { get; set; }
    public string EquipmentTypeDesc { get; set; }
}