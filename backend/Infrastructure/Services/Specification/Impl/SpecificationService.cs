using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Core.Enums;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services.Specification.Impl;

public class SpecificationService : ISpecificationService
{
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<Core.Entities.Specification> _speRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<SpecificationTypeRule> _sptRepo;
    private readonly IRepository<MachineModel> _mmRepo;
    private readonly IRepository<CraftIndicatorRule> _cirRepo;
    private readonly ShiFangSettings _settings;

    public SpecificationService(IRepository<Core.Entities.Specification> speRepo,
        IRepository<Core.Entities.Indicator> iRepo, IUnitOfWork uow,
        IOptionsSnapshot<ShiFangSettings> settings, IRepository<SpecificationTypeRule> sptRepo,
        IRepository<MachineModel> mmRepo, IRepository<CraftIndicatorRule> cirRepo)
    {
        _speRepo = speRepo;
        _iRepo = iRepo;
        _uow = uow;
        _sptRepo = sptRepo;
        _mmRepo = mmRepo;
        _cirRepo = cirRepo;
        _settings = settings.Value;
    }

    public IEnumerable<SpecificationTableDto> Get(SpecificationQueryInfoDto dto, out int total)
    {
        var data = _speRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c =>
                c.Name.Contains(dto.Query) || c.OrderNo.Contains(dto.Query) || c.Remark.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.TypeId))
        {
            var typeId = int.Parse(dto.TypeId);
            data = data.Where(c => c.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Include(c => c.SpecificationType).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c =>
                new SpecificationTableDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    OrderNo = c.OrderNo,
                    TypeId = c.SpecificationTypeId,
                    TypeName = c.SpecificationType.Name,
                    ModifiedTime = c.ModifiedAtUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                    State = c.Status == Status.Enabled,
                    EquipmentTypeName = c.EquipmentType.toDescription()
                }).ToList();

        return result;
    }

    public bool Add(SpecificationEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_speRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该牌号名称已被使用, 请使用其他名称";
            return false;
        }

        var indicators = _iRepo.All().Where(c => c.IndicatorProject == IndicatorProject.Measure).ToList();
        if (dto.SingleRules != null)
            foreach (var rule in dto.SingleRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
                if (dto.EquipmentTypeId == (int)EquipmentType.SingleResistance ||
                    dto.EquipmentTypeId == (int)EquipmentType.Mts)
                {
                    var mid = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var high = mid + upper;
                    var low = mid - lower;
                    if (rule.Id == _settings.Resistance)
                    {
                        rule.Standard = ConvertHelper.mmWGToPa(mid).ToString("F5");
                        rule.Upper = ConvertHelper.mmWGToPa(high).ToString("F5");
                        rule.Lower = ConvertHelper.mmWGToPa(low).ToString("F5");
                    }
                }
            }


        if (dto.MeanRules != null)
            foreach (var rule in dto.MeanRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
            }

        if (dto.SdRules != null)
            foreach (var rule in dto.SdRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
            }


        var specification = new Core.Entities.Specification
        {
            Name = dto.Name,
            OrderNo = dto.OrderNo,
            SpecificationTypeId = dto.TypeId,
            Remark = dto.Remark,
            EquipmentType = (EquipmentType)dto.EquipmentTypeId,
            Status = dto.State ? Status.Enabled : Status.Disabled,
            SingleRules = JsonConvert.SerializeObject(dto.SingleRules),
            MeanRules = JsonConvert.SerializeObject(dto.MeanRules),
            SdRules = JsonConvert.SerializeObject(dto.SdRules),
            CvRules = JsonConvert.SerializeObject(dto.CvRules),
            CpkRules = JsonConvert.SerializeObject(dto.CpkRules)
        };

        _speRepo.Add(specification);

        return _uow.Save() > 0;
    }

    public bool Update(SpecificationEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_speRepo.All().Any(c => c.Name == dto.Name && c.Id != dto.Id))
        {
            failReason = "该牌号名称已被使用, 请使用其他名称";
            return false;
        }

        var indicators = _iRepo.All().Where(c => c.IndicatorProject == IndicatorProject.Measure).ToList();
        if (dto.SingleRules != null)
            foreach (var rule in dto.SingleRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
                if (dto.EquipmentTypeId == (int)EquipmentType.SingleResistance ||
                    dto.EquipmentTypeId == (int)EquipmentType.Mts)
                {
                    var mid = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var high = mid + upper;
                    var low = mid - lower;
                    if (rule.Id == _settings.Resistance)
                    {
                        rule.Standard = ConvertHelper.mmWGToPa(mid).ToString("F5");
                        rule.Upper = ConvertHelper.mmWGToPa(high).ToString("F5");
                        rule.Lower = ConvertHelper.mmWGToPa(low).ToString("F5");
                    }
                }
            }

        if (dto.MeanRules != null)
            foreach (var rule in dto.MeanRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
            }

        if (dto.SdRules != null)
            foreach (var rule in dto.SdRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                rule.Name = indicator == null ? "" : indicator.Name;
            }


        var specification = _speRepo.Get(dto.Id);
        specification.Name = dto.Name;
        specification.OrderNo = dto.OrderNo;
        specification.SpecificationTypeId = dto.TypeId;
        specification.Remark = dto.Remark;
        specification.EquipmentType = (EquipmentType)dto.EquipmentTypeId;
        specification.SingleRules = JsonConvert.SerializeObject(dto.SingleRules);
        specification.MeanRules = JsonConvert.SerializeObject(dto.MeanRules);
        specification.SdRules = JsonConvert.SerializeObject(dto.SdRules);
        specification.CpkRules = JsonConvert.SerializeObject(dto.CpkRules);
        specification.CvRules = JsonConvert.SerializeObject(dto.CvRules);
        specification.Status = dto.State ? Status.Enabled : Status.Disabled;

        _speRepo.Update(specification);

        return _uow.Save() >= 0;
    }

    public SpecificationEditDto GetInfo(int id)
    {
        var specification = _speRepo.Get(id);
        var dto = new SpecificationEditDto
        {
            Id = specification.Id,
            Name = specification.Name,
            OrderNo = specification.OrderNo,
            TypeId = specification.SpecificationTypeId,
            Remark = specification.Remark,
            EquipmentTypeId = (int)specification.EquipmentType,
            SingleRules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(specification.SingleRules),
            MeanRules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(specification.MeanRules),
            SdRules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(specification.SdRules),
            CvRules = specification.CvRules == null
                ? null
                : JsonConvert.DeserializeObject<IEnumerable<Rule>>(specification.CvRules),
            CpkRules = specification.CpkRules == null
                ? null
                : JsonConvert.DeserializeObject<IEnumerable<Rule>>(specification.CpkRules),
            State = specification.Status == Status.Enabled
        };

        if (specification.EquipmentType == EquipmentType.SingleResistance ||
            specification.EquipmentType == EquipmentType.Mts)
        {
            if (dto.SingleRules != null)
            {
                foreach (var item in dto.SingleRules)
                {
                    var mid = double.Parse(item.Standard);
                    var high = double.Parse(item.Upper);
                    var low = double.Parse(item.Lower);
                    var upper = Math.Round(high - mid, 3);
                    var lower = Math.Round(mid - low, 3);
                    if (item.Id == _settings.Resistance)
                    {
                        item.Standard = ConvertHelper.paToMMWG(mid).ToString("F0");
                        item.Upper = ConvertHelper.paToMMWG(upper).ToString("F0");
                        item.Lower = ConvertHelper.paToMMWG(lower).ToString("F0");
                    }
                }
            }
        }

        return dto;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _speRepo.All().AsNoTracking().OrderByDescending(c => c.ModifiedAtUtc)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id,
                Text = c.Name
            }).ToList();
        return data;
    }

    public IEnumerable<BaseOptionDto> GetSpecificationsByTypeId(int id)
    {
        var data = _speRepo.All().Where(c => c.SpecificationTypeId == id && c.Status == Status.Enabled).Select(c =>
            new BaseOptionDto
            {
                Value = c.Id,
                Text = c.Name
            }).ToList();

        return data;
    }

    public bool GetIndicatorsTableDescById(int id,
        out TableEditor result,
        out string message,
        int measureTypeId = 0,
        int machineModelId = 0)
    {
        result = new TableEditor();
        message = "";
        var specification = _speRepo.Get(id);
        if (specification == null)
        {
            message = "没有找到对应牌号信息";
            return false;
        }

        var indicators = _iRepo.All().AsNoTracking().ToList();
        List<Rule> rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);

        if (rules == null || rules.Count == 0)
        {
            message = "没有找到该牌号的测量指标信息, 请填写牌号中的单支规则信息";
            return false;
        }

        var tableEditor = new TableEditor
        {
            Label = "原始数据",
            Type = "table-editor"
        };

        var tableEditorAttrs = new TableEditor.TableEditorAttr()
        {
            IsShowDelete = true,
            Rules = new object(),
            TableAttrs = new Dictionary<string, object>()
            {
                { "max-height", "500" },
                {"ref", "dataTable" }
            }
        };

        var tableEditorColumns = new List<TableEditor.ColumnAttr>();
        var idColumn = new TableEditor.ColumnAttr
        {
            Prop = "id",
            Label = "ID",
            Width = "100"
        };
        tableEditorColumns.Add(idColumn);
        var timeColumns = new TableEditor.ColumnAttr()
        {
            Prop = "testTime",
            Label = "测量时间",
            Width = "300",
            Content = new Dictionary<string, object>()
            {
                {"type", "el-date-picker"},
                {"attrs", new Dictionary<string, object>()
                {
                    {"type", "datetime"},
                    {"placeholder", "选择测量时间"},
                    {"value-format", "yyyy-MM-dd HH:mm:ss"},
                    {"default-value", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                }}
            }
        };
        tableEditorColumns.Add(timeColumns);
        foreach (var rule in rules.OrderByDescending(c => c.Standard))
        {
            var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
            if (indicator == null) continue;
            var indicatorName = indicator.Name;
            if (rule.Standard == "0") indicatorName += "(外观)";

            var indicatorColumn = new TableEditor.ColumnAttr()
            {
                Prop = rule.Id.ToString(),
                Label = indicatorName,
                Width = "220",
                Content = new Dictionary<string, object>()
                {
                    { "type", "el-input-number" },
                    {
                        "attrs", new Dictionary<string, object>()
                        {
                            { "width", "200" }
                        }
                    }
                }
            };
            tableEditorColumns.Add(indicatorColumn);
        }

        tableEditorAttrs.Columns = tableEditorColumns;
        tableEditor.Attrs = tableEditorAttrs;

        result = tableEditor;
        return true;
    }
}