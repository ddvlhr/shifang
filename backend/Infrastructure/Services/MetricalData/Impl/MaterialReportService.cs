using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Dtos.Specification;
using Core.Dtos.Material;
using Core.Dtos.MetricalData;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.MetricalData.Impl;

public class MaterialReportService : IMaterialReportService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<Manufacturer> _mRepo;
    private readonly IRepository<MaterialReport> _mrRepo;
    private readonly IRepository<MaterialTemplate> _mtRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IUnitOfWork _uow;

    public MaterialReportService(IRepository<MaterialReport> mrRepo,
        IRepository<Manufacturer> mRepo, IRepository<MaterialTemplate> mtRepo,
        IRepository<Group> gRepo, IRepository<Core.Entities.Indicator> iRepo,
        IRepository<Core.Entities.Specification> spRepo, IHttpContextAccessor accessor,
        IUnitOfWork uow, IOptionsSnapshot<Settings> settings)
    {
        _mrRepo = mrRepo;
        _mRepo = mRepo;
        _mtRepo = mtRepo;
        _gRepo = gRepo;
        _iRepo = iRepo;
        _spRepo = spRepo;
        _accessor = accessor;
        _uow = uow;
        _settings = settings.Value;
    }

    public IEnumerable<MaterialReportTableDto> GetTable(MaterialReportQueryInfoDto dto, out int total)
    {
        var data = _mrRepo.All().AsNoTracking();
        if (!_settings.CanSeeOtherData)
        {
            var roleId = _accessor.HttpContext.getUserRoleId();
            if (roleId != _settings.AdminTypeId)
            {
                var userId = _accessor.HttpContext.getUserId();
                data = data.Where(c => c.Group.UserId == userId);
            }
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.ManufacturerName.Contains(dto.Query) ||
                                   c.SpecificationType.Name.Contains(dto.Query) ||
                                   c.SamplePlace.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.ManufacturerId))
        {
            var manufacturerId = int.Parse(dto.ManufacturerId);
            data = data.Where(c => c.ManufacturerId == manufacturerId);
        }

        if (!string.IsNullOrEmpty(dto.TypeId))
        {
            var typeId = int.Parse(dto.TypeId);
            data = data.Where(c => c.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.ReportRet == (ReportRet)state);
        }

        total = data.Count();

        var result = data.OrderByDescending(c => c.Group.BeginTime).Include(c => c.SpecificationType)
            .Include(c => c.Group).ThenInclude(c => c.Specification).Skip(dto.Skip()).Take
            (dto
                .PageSize).Select(c => new
                MaterialReportTableDto
                {
                    Id = c.Id, Manufacturer = c.ManufacturerName, TestDate = c.TestDate.ToString("yyyy-MM-dd"),
                    TypeName = c.SpecificationType.Name, State = (int)c.ReportRet,
                    SamplePlace = c.SamplePlace, SpecificationName = c.Group.Specification.Name,
                    UserName = c.Group.UserName
                }).ToList();

        return result;
    }

    public bool Edit(MaterialReportEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var isEdit = dto.Id > 0;
        var report = new MaterialReport();
        if (isEdit) report = _mrRepo.Get(dto.Id);

        var manufacturer = _mRepo.Get(dto.ManufacturerId);
        report.TestDate = dto.TestDate;
        report.OrderNo = dto.OrderNo;
        report.ManufacturerId = dto.ManufacturerId;
        report.ManufacturerName = manufacturer.Name;
        report.SpecificationTypeId = dto.TypeId;
        report.SamplePlace = dto.SamplePlace;
        report.SampleCount = dto.SampleCount;
        report.Unit = dto.Unit;
        report.Description = dto.Description;
        report.OtherDesc = dto.OtherDesc;
        report.ReportRet = dto.ReportRet ? ReportRet.合格 : ReportRet.不合格;
        report.Result = dto.Result;
        report.Checker = dto.UserId;
        report.Temperature = dto.Temperature;
        report.Humidity = dto.Humidity;

        if (isEdit)
            _mrRepo.Update(report);
        else
            _mrRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }

    public string GetTemplate(int id)
    {
        var desc = "";
        var template = _mtRepo.All().FirstOrDefault(c => c.SpecificationTypeId == id);
        if (template != null) desc = template.Description;

        return desc;
    }

    public MaterialReportEditDto GetInfo(int id)
    {
        var dto = new MaterialReportEditDto();
        var data = _mrRepo.All().Include(c => c.User).Include(c => c.SpecificationType).Include(c => c.Group)
            .ThenInclude(c => c.Specification).FirstOrDefault(c => c.Id == id);
        var nearData = _mrRepo.All().OrderByDescending(c => c.CreatedAtUtc).FirstOrDefault(c => c.SpecificationTypeId ==
            data
                .SpecificationTypeId && c.Id != id);
        var orderNo = string.IsNullOrEmpty(data.OrderNo)
            ? nearData != null ? nearData.OrderNo : ""
            : data.OrderNo;
        var manufacturerId = data.ManufacturerId == 0
            ? nearData?.ManufacturerId ?? 0
            : data.ManufacturerId;
        var samplePlace = string.IsNullOrEmpty(data.SamplePlace)
            ? nearData != null ? nearData.SamplePlace : ""
            : data.SamplePlace;
        var sampleCount = string.IsNullOrEmpty(data.SampleCount)
            ? nearData != null ? nearData.SampleCount : ""
            : data.SampleCount;
        var unit = string.IsNullOrEmpty(data.Unit) ? nearData != null ? nearData.Unit : "" : data.Unit;
        var otherDesc = string.IsNullOrEmpty(data.OtherDesc)
            ? nearData != null ? nearData.OtherDesc : ""
            : data.OtherDesc;
        var result = string.IsNullOrEmpty(data.Result) ? nearData != null ? nearData.Result : "" : data.Result;
        var manufacturerName = "";
        if (manufacturerId > 0) manufacturerName = _mRepo.Get(manufacturerId).Name;


        dto.Id = data.Id;
        dto.TestDate = data.TestDate;
        dto.TestDateStr = data.TestDate.ToString("yyyy年MM月dd日");
        dto.SpecificationName = data.Group.Specification.Name;
        dto.OrderNo = orderNo;
        dto.ManufacturerId = manufacturerId;
        dto.ManufacturerName = manufacturerName;
        dto.TypeId = data.SpecificationTypeId;
        dto.TypeName = data.SpecificationType.Name;
        dto.SamplePlace = samplePlace;
        dto.SampleCount = sampleCount;
        dto.Unit = unit;
        dto.Description = data.Description;
        dto.OtherDesc = otherDesc;
        dto.Result = result;
        dto.ReportRet = data.ReportRet == ReportRet.未完成 || data.ReportRet == ReportRet.合格;
        dto.UserId = data.Checker;
        dto.UserName = data.User.NickName;
        dto.Temperature = data.Temperature;
        dto.Humidity = data.Humidity;

        if (!string.IsNullOrEmpty(otherDesc))
        {
            var otherDescArr = dto.OtherDesc.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var desc = dto.Description.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
            var count = desc.Count + 1;
            foreach (var d in otherDescArr)
            {
                desc.Add($"{count}. {d}");
                count++;
            }

            dto.ResultDesc = string.Join("\r\n", desc);
            // desc.Join("\r\n");
        }
        else
        {
            dto.ResultDesc = dto.Description;
        }

        return dto;
    }

    public bool Add(MetricalDataEditDataDto dto)
    {
        var group = _gRepo.All().Include(c => c.Specification).FirstOrDefault(c => c.Id == dto.GroupId);
        if (group == null) return false;
        var isEdit = false;
        var report = new MaterialReport
        {
            GroupId = dto.GroupId,
            SpecificationTypeId = group.Specification.SpecificationTypeId,
            ManufacturerId = 0
        };
        if (_mrRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _mrRepo.All().FirstOrDefault(c => c.GroupId == dto.GroupId);
        }

        if (report == null) return false;

        report.OrderNo = group.OrderNo;
        var data = (JArray)JsonConvert.DeserializeObject(dto.DataInfo);
        if (data == null)
            return false;
        var index = 1;
        var sb = new StringBuilder();
        var specification = _spRepo.Get(group.SpecificationId);
        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        if (singleRules == null)
            return false;
        foreach (var rule in singleRules)
        {
            var indicator = _iRepo.Get(rule.Id);
            var name = rule.Name == "" ? indicator.Name : rule.Name;
            var tempStr = $"{index}. {name}";
            if (!string.IsNullOrEmpty(indicator.Unit))
                tempStr += $"({indicator.Unit})：";
            else
                tempStr += "：";
            var nullCount = 0;
            foreach (var item in data)
            {
                var value = item[$"{rule.Id}"];
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    tempStr += $" {value}";
                else
                    nullCount++;
            }

            if (nullCount != data.Count)
            {
                sb.AppendLine(tempStr);
                index++;
            }
        }

        report.Description = sb.ToString();
        report.TestDate = group.BeginTime;
        report.ReportRet = ReportRet.未完成;
        var userId = _accessor.HttpContext.getUserId();
        report.Checker = Convert.ToInt32(userId);
        if (isEdit)
            _mrRepo.Update(report);
        else
            _mrRepo.Add(report);

        return isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
    }
}