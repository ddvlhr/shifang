using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class ModelService : IModelService
{
    private readonly IRepository<Model> _mRepo;
    private readonly IUnitOfWork _uow;

    public ModelService(IRepository<Model> mRepo, IUnitOfWork uow)
    {
        _mRepo = mRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _mRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = Convert.ToInt32(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.CreatedAtUtc).OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c => new BaseTableDto
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该机型已存在, 请使用其他名称";
            return false;
        }

        var model = new Model
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _mRepo.Add(model);
        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该机型已存在, 请使用其他名称";
            return false;
        }

        var model = _mRepo.Get(dto.Id);
        model.Name = dto.Name;
        model.Status = dto.State ? Status.Enabled : Status.Disabled;

        _mRepo.Update(model);
        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _mRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
        {
            Value = c.Id, Text = c.Name
        }).ToList();
        return data;
    }
}