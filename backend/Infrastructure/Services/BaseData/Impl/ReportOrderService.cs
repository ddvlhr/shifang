using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class ReportOrderService : IReportOrderService
{
    private readonly IRepository<ReportOrder> _roRepo;
    private readonly IUnitOfWork _uow;

    public ReportOrderService(IRepository<ReportOrder> roRepo, IUnitOfWork uow)
    {
        _roRepo = roRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _roRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var list = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip()).Take(dto.PageSize).Select(c => new
            BaseTableDto
            {
                Id = c.Id, Name = c.Name, Remark = c.Remark, State = c.Status == Status.Enabled
            }).ToList();

        return list;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_roRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该编号已存在, 请输入其他编号";
            return false;
        }

        var reportOrder = new ReportOrder
        {
            Name = dto.Name,
            Remark = dto.Remark,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _roRepo.Add(reportOrder);
        var result = _uow.Save();
        return result > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_roRepo.All().Any(c => c.Name == dto.Name && c.Id != dto.Id))
        {
            failReason = "该编号已存在, 请输入其他编号";
            return false;
        }

        var reportOrder = _roRepo.Get(dto.Id);
        if (reportOrder == null)
        {
            failReason = "没有找到对应的数据";
            return false;
        }

        reportOrder.Name = dto.Name;
        reportOrder.Remark = dto.Remark;
        reportOrder.Status = dto.State ? Status.Enabled : Status.Disabled;

        _roRepo.Update(reportOrder);
        var result = _uow.Save();
        return result >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var options = _roRepo.All().OrderBy(c => c.Id).Where(c => c.Status == Status.Enabled).Select(c =>
            new BaseOptionDto
            {
                Text = string.IsNullOrEmpty(c.Remark) ? c.Name : $"{c.Name}({c.Remark})", ValueStr = c.Name
            }).ToList();
        return options;
    }
}