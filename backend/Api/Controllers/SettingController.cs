using System.Linq;
using Api.Settings;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Api.Controllers;

[Authorize]
public class SettingController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly IRepository<SystemSetting> _ssRepo;
    private readonly IUnitOfWork _uow;
    private readonly IWriteAbleOptions<Core.Models.Settings> _writeAbleSettings;

    public SettingController(IWriteAbleOptions<Core.Models.Settings> writeAbleSettings,
        IConfiguration configuration, IRepository<SystemSetting> ssRepo, IUnitOfWork uow)
    {
        _writeAbleSettings = writeAbleSettings;
        _configuration = configuration;
        _configurationRoot = configuration as IConfigurationRoot;
        _ssRepo = ssRepo;
        _uow = uow;
    }

    [HttpGet]
    [Route("settings")]
    public IActionResult GetSettings()
    {
        var settings = _writeAbleSettings.Value;
        settings.ErrorPushAt = settings.ErrorPushAt.Distinct().ToArray();
        settings.CraftTypeIds = settings.CraftTypeIds.Distinct().ToArray();
        settings.RecycleBoxTypeId = settings.RecycleBoxTypeId.Distinct().ToArray();
        return Success(settings);
    }

    [HttpPost]
    [Route("settings")]
    public IActionResult SetSettingDesc([FromBody] Core.Models.Settings setting)
    {
        _writeAbleSettings.Update(opt =>
        {
            opt.Version = setting.Version;
            opt.CanSeeOtherData = setting.CanSeeOtherData;
            opt.EnableErrorPush = setting.EnableErrorPush;
            opt.ErrorPushAt = setting.ErrorPushAt.Distinct().ToArray();
            opt.MySqlServerName = setting.MySqlServerName;
            opt.AdminTypeId = setting.AdminTypeId;
            opt.PhysicalTypeId = setting.PhysicalTypeId;
            opt.InspectionTypeId = setting.InspectionTypeId;
            opt.ProductionTypeId = setting.ProductionTypeId;
            opt.MaterialTypeId = setting.MaterialTypeId;
            opt.FactoryTypeId = setting.FactoryTypeId;
            opt.ChemicalTypeId = setting.ChemicalTypeId;
            opt.CraftTypeId = setting.CraftTypeId;
            opt.FirstCraftTypeId = setting.FirstCraftTypeId;
            opt.SecondCraftTypeId = setting.SecondCraftTypeId;
            opt.ThirdCraftTypeId = setting.ThirdCraftTypeId;
            opt.CraftTypeIds = setting.CraftTypeIds.Distinct().ToArray();
            opt.FilterTypeId = setting.FilterTypeId;
            opt.TestTypeId = setting.TestTypeId;
            opt.Weight = setting.Weight;
            opt.Circle = setting.Circle;
            opt.Oval = setting.Oval;
            opt.Length = setting.Length;
            opt.Resistance = setting.Resistance;
            opt.Hardness = setting.Hardness;
            opt.RecycleBoxTypeId = setting.RecycleBoxTypeId.Distinct().ToArray();
            opt.BoxMakingWorkShopId = setting.BoxMakingWorkShopId;
            opt.WaterIndicatorId = setting.WaterIndicatorId;
            opt.QuantityIndicatorId = setting.QuantityIndicatorId;
            opt.WelcomeImages = setting.WelcomeImages;
            opt.FactoryAuditUser = setting.FactoryAuditUser;
            opt.CraftLogOrderNo = setting.CraftLogOrderNo;
            opt.CraftManager = setting.CraftManager;
            opt.Burst = setting.Burst;
            opt.GlueHole = setting.GlueHole;
            opt.PeculiarSmell = setting.PeculiarSmell;
            opt.InnerBondLine = setting.InnerBondLine;
            opt.MonthCraftReportOrderNo = setting.MonthCraftReportOrderNo;
            opt.OriginMaterialTypeId = setting.OriginMaterialTypeId;
            opt.AddOriginMaterialTypeId = setting.AddOriginMaterialTypeId;
            opt.SilkTypeId = setting.SilkTypeId;
            opt.RollPackTypeId = setting.RollPackTypeId;
            opt.ManualTypeId = setting.ManualTypeId;
            opt.ProcessingTypeId = setting.ProcessingTypeId;
            opt.AppearanceTypeId = setting.AppearanceTypeId;
        });
        var dbSetting = _ssRepo.All().FirstOrDefault();
        var isEdit = false;
        if (dbSetting == null)
        {
            var data = new SystemSetting
            {
                CanSeeOtherData = setting.CanSeeOtherData,
                AdminId = setting.AdminTypeId,
                MySqlServerName = setting.MySqlServerName,
                PhysicalTypeId = setting.PhysicalTypeId,
                InspectionTypeId = setting.InspectionTypeId,
                ProductionTypeId = setting.ProductionTypeId,
                MaterialTypeId = setting.MaterialTypeId,
                FactoryTypeId = setting.FactoryTypeId,
                ChemicalTypeId = setting.ChemicalTypeId,
                CraftTypeId = setting.CraftTypeId,
                FilterTypeId = setting.FilterTypeId,
                Weight = setting.Weight,
                Circle = setting.Circle,
                Oval = setting.Oval,
                Length = setting.Length,
                Resistance = setting.Resistance,
                Hardness = setting.Hardness,
                RecycleBoxTypeId = JsonConvert.SerializeObject(setting.RecycleBoxTypeId),
                BoxMakingWorkShopId = setting.BoxMakingWorkShopId
            };
            _ssRepo.Add(data);
        }
        else
        {
            isEdit = true;
            dbSetting.CanSeeOtherData = setting.CanSeeOtherData;
            dbSetting.AdminId = setting.AdminTypeId;
            dbSetting.MySqlServerName = setting.MySqlServerName;
            dbSetting.PhysicalTypeId = setting.PhysicalTypeId;
            dbSetting.InspectionTypeId = setting.InspectionTypeId;
            dbSetting.ProductionTypeId = setting.ProductionTypeId;
            dbSetting.MaterialTypeId = setting.MaterialTypeId;
            dbSetting.FactoryTypeId = setting.FactoryTypeId;
            dbSetting.ChemicalTypeId = setting.ChemicalTypeId;
            dbSetting.FilterTypeId = setting.FilterTypeId;
            dbSetting.Weight = setting.Weight;
            dbSetting.Circle = setting.Circle;
            dbSetting.Oval = setting.Oval;
            dbSetting.Length = setting.Length;
            dbSetting.Resistance = setting.Resistance;
            dbSetting.Hardness = setting.Hardness;
            dbSetting.RecycleBoxTypeId = JsonConvert.SerializeObject(setting.RecycleBoxTypeId);
            dbSetting.BoxMakingWorkShopId = setting.BoxMakingWorkShopId;
            _ssRepo.Update(dbSetting);
        }

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
        // appManager.Restart();
        return result ? Success() : Error("保存设置信息到数据库时发生错误");
    }
}