using System;
using System.Linq;
using System.Text.Json;
using Api.Settings;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Api.Controllers;

[Authorize]
public class SettingController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly ISqlSugarClient _db;
    private readonly IWriteAbleOptions<Core.Models.ShiFangSettings> _writeAbleSettings;

    public SettingController(IWriteAbleOptions<ShiFangSettings> writeAbleSettings,
        IConfiguration configuration,
        ISqlSugarClient db)
    {
        _writeAbleSettings = writeAbleSettings;
        _configuration = configuration;
        _configurationRoot = configuration as IConfigurationRoot;
        _db = db;
    }

    [HttpGet]
    [Route("settings")]
    public IActionResult GetSettings()
    {
        var settings = _writeAbleSettings.Value;
        settings.ErrorPushAt = settings.ErrorPushAt.Distinct().ToArray();
        return Success(settings);
    }

    [HttpPost]
    [Route("settings")]
    public IActionResult SetSettingDesc([FromBody] ShiFangSettings setting)
    {
        _writeAbleSettings.Update(opt =>
        {
            // opt = setting;
            opt.DataSource = setting.DataSource;
            opt.EnableErrorPush = setting.EnableErrorPush;
            opt.ErrorPushAt = setting.ErrorPushAt.Distinct().ToArray();
            opt.AdminTypeId = setting.AdminTypeId;
            opt.CigarTypeId = setting.CigarTypeId;
            opt.Resistance = setting.Resistance;
        });
        var serializeOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        var dbSetting = new Core.SugarEntities.ShiFangSettings()
        {
            Time = DateTime.Now,
            Settings = JsonSerializer.Serialize(setting, serializeOptions),
            CreatedAtUtc = DateTime.Now,
            ModifiedAtUtc = DateTime.Now
        };

        var result = _db.Storageable(dbSetting).ExecuteCommand() > 0;
        return result ? Success() : Error("保存设置信息到数据库时发生错误");
    }
}