using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.Specification.Impl;

public class SpecificationService : ISpecificationService
{
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<Core.Entities.Specification> _speRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<SpecificationTypeRule> _sptRepo;
    private readonly IRepository<MachineModel> _mmRepo;
    private readonly IRepository<CraftIndicatorRule> _cirRepo;
    private readonly Settings _settings;

    public SpecificationService(IRepository<Core.Entities.Specification> speRepo,
        IRepository<Core.Entities.Indicator> iRepo, IUnitOfWork uow,
        IOptionsSnapshot<Settings> settings, IRepository<SpecificationTypeRule> sptRepo,
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
        var result = data.Include(c => c.SpecificationType).OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c =>
                new SpecificationTableDto
                {
                    Id = c.Id, Name = c.Name, OrderNo = c.OrderNo, TypeId = c.SpecificationTypeId,
                    TypeName = c.SpecificationType.Name, ModifiedTime = c.ModifiedAtUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                    State = c.Status == Status.Enabled
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

        return dto;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _speRepo.All().AsNoTracking().OrderByDescending(c => c.ModifiedAtUtc)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();
        return data;
    }

    public IEnumerable<BaseOptionDto> GetSpecificationsByTypeId(int id)
    {
        var data = _speRepo.All().Where(c => c.SpecificationTypeId == id && c.Status == Status.Enabled).Select(c =>
            new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }

    public bool GetIndicatorsTableDescById(int id,
        out string result, 
        out string message, 
        int measureTypeId = 0,
        int machineModelId = 0)
    {
        result = "";
        message = "";
        var specification = _speRepo.Get(id);
        if (specification == null)
        {
            message = "没有找到对应牌号信息";
            return false;
        }

        var indicators = _iRepo.All().AsNoTracking().ToList();
        List<Rule> rules;
        if (measureTypeId > 0 && measureTypeId == _settings.ChemicalTypeId)
        {
            var spt = _sptRepo.All().FirstOrDefault(c => c.SpecificationTypeId == specification.SpecificationTypeId);
            rules = JsonConvert.DeserializeObject<List<Rule>>(spt != null ? spt.Rules : specification.SingleRules);
        }
        else if (machineModelId > 0 && _settings.CraftTypeIds.Contains(measureTypeId))
        {
            var machineModel = _mmRepo.Get(machineModelId);
            CraftIndicatorRule craftRules = null;
            if (machineModel.ModelId > 0)
                craftRules = _cirRepo.All().FirstOrDefault(c => c.ModelId == machineModel.ModelId);

            rules = JsonConvert.DeserializeObject<List<Rule>>(craftRules != null
                ? craftRules.Rules
                : specification.SingleRules);
        }
        else
        {
            rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        }

        if (rules == null || rules.Count == 0)
        {
            message = "没有找到该牌号的测量指标信息, 请填写牌号中的单支规则信息";
            return false;
        }
        
        var label = new JObject { { "label", "原始数据" }, { "type", "table-editor" } };
        var columns = new JArray();
        var idObj = new JObject { { "prop", "id" }, { "label", "ID" }, { "width", "150" } };
        columns.Add(idObj);
        var testTime = new JObject { { "prop", "testTime" }, { "label", "测量时间" }, { "width", "300" } };
        var testTimeContent = new JObject { { "type", "el-date-picker" } };
        var testTimeAttrs = new JObject
        {
            { "type", "datetime" }, { "value-format", "yyyy-MM-dd HH:mm:ss" },
            { "default-value", DateTime.Now.Date },
            { "default-time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}" }
        };
        testTimeContent.Add("attrs", testTimeAttrs);
        testTime.Add("content", testTimeContent);
        columns.Add(testTime);
        foreach (var rule in rules.OrderByDescending(c => c.Standard))
        {
            var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
            if (indicator == null) continue;
            var indicatorName = indicator.Name;
            if (rule.Standard == "0") indicatorName += "(外观)";
            var column = new JObject { { "prop", rule.Id.ToString() }, { "label", indicatorName }, { "width", 150 } };
            var content = new JObject { { "type", "el-input" } };
            var attrs = new JObject { { "type", "number" }, { "step", "0.0000001" } };
            content.Add("attrs", attrs);
            column.Add("content", content);
            columns.Add(column);
        }

        var tableAttrs = new JObject { { "ref", "dataTable" }, { "height", "500" } };
        var parentAttrs = new JObject { { "tableAttrs", tableAttrs }, { "columns", columns } };
        label.Add("attrs", parentAttrs);
        var dataObj = new JObject { { "data", label } };
        var specificationObj = new JObject { { "id", specification.Id }, { "desc", dataObj } };
        result = specificationObj.ToString();
        return true;
    }
}