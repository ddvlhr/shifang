using System.Collections.Generic;

namespace Core.Models;

public class Settings
{
    public string Version { get; set; }
    public bool CanSeeOtherData { get; set; }
    public bool EnableErrorPush { get; set; }
    public string[] ErrorPushAt { get; set; }
    public string MySqlServerName { get; set; }
    public int AdminTypeId { get; set; }
    public int PhysicalTypeId { get; set; }
    public int InspectionTypeId { get; set; }
    public int ProductionTypeId { get; set; }
    public int MaterialTypeId { get; set; }
    public int FactoryTypeId { get; set; }
    public int ChemicalTypeId { get; set; }
    public int CraftTypeId { get; set; }
    public int FirstCraftTypeId { get; set; }
    public int SecondCraftTypeId { get; set; }
    public int ThirdCraftTypeId { get; set; }
    public int[] CraftTypeIds { get; set; }
    public int FilterTypeId { get; set; }
    public int TestTypeId { get; set; }
    public int Weight { get; set; }
    public int Circle { get; set; }
    public int Oval { get; set; }
    public int Length { get; set; }
    public int Resistance { get; set; }
    public int Hardness { get; set; }
    public int[] RecycleBoxTypeId { get; set; }
    public int BoxMakingWorkShopId { get; set; }
    public int WaterIndicatorId { get; set; }
    public int QuantityIndicatorId { get; set; }
    public string WelcomeImages { get; set; }
    public int FactoryAuditUser { get; set; }
    public string CraftLogOrderNo { get; set; }
    public int CraftManager { get; set; }
    public string GetRemoteIpUrl { get; set; }
    public string MonthCraftReportOrderNo { get; set; }
    public int Burst { get; set; }
    public int GlueHole { get; set; }
    public int PeculiarSmell { get; set; }
    public int InnerBondLine { get; set; }
    public int OriginMaterialTypeId { get; set; }
    public int AddOriginMaterialTypeId { get; set; }
    public int SilkTypeId { get; set; }
    public int RollPackTypeId { get; set; }
    public int ManualTypeId { get; set; }
    public int ProcessingTypeId { get; set; }
    public int AppearanceTypeId { get; set; }
    public List<MonthCraftReportItem> MonthCraftReportItems { get; set; }
}