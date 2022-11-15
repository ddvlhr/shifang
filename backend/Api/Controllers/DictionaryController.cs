using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// 字典
/// </summary>
public class DictionaryController : BaseController
{
    [HttpGet, Route("dict")]
    public IActionResult GetDictionary()
    {
        var permissionTypes = typeof(PermissionType).ToOptions().ToList();
        var buttonPositions = typeof(ButtonPosition).ToOptions().ToList();
        var equipmentTypes = typeof(EquipmentType).ToOptions().ToList();
        var buttonTypes = typeof(ButtonType).ToOptions().ToList();
        var defectCategories = typeof(DefectCategory).ToOptions().ToList();
        var qualityResult = typeof(QualityResult).ToOptions().ToList();
        var stateList = typeof(Status).ToOptions().ToList();
        var boolStateList = stateList.Select(state => new BaseOptionDto() { Value = Convert.ToInt32(state.Value) == 0, Text = state.Text, Type = Convert.ToInt32(state.Value) == 0 ? "success" : "danger" }).ToList();
        var qualityTypeResult = new List<BaseOptionDto>()
        {
            new BaseOptionDto() { Value = QualityResult.Quality, Text = QualityResult.Quality.toDescription(), Type = "success"},
            new BaseOptionDto() { Value = QualityResult.Grade, Text = QualityResult.Grade.toDescription(), Type = "primary"},
            new BaseOptionDto() { Value = QualityResult.Nonconforming, Text = QualityResult.Nonconforming.toDescription(), Type = "danger"}
        };
        var qualified = typeof(QualifiedStatus).ToOptions().ToList();
        var qualifiedTypes = new List<BaseOptionDto>()
        {
            new BaseOptionDto()
                { Value = QualifiedStatus.Undefined, Text = QualifiedStatus.Undefined.toDescription(), Type = "info" },
            new BaseOptionDto()
            {
                Value = QualifiedStatus.Qualified, Text = QualifiedStatus.Qualified.toDescription(), Type = "success"
            },
            new BaseOptionDto()
            {
                Value = QualifiedStatus.UnQualified, Text = QualifiedStatus.UnQualified.toDescription(), Type = "danger"
            }
        };
        var materialCheckStatus = typeof(MaterialCheckStatus).ToOptions().ToList();
        var materialCheckStatusTypes = new List<BaseOptionDto>()
        {
            new BaseOptionDto()
            {
                Value = MaterialCheckStatus.Undetected, Text = MaterialCheckStatus.Undetected.toDescription(),
                Type = "info"
            },
            new BaseOptionDto()
            {
                Value = MaterialCheckStatus.Detected, Text = MaterialCheckStatus.Detected.toDescription(),
                Type = "primary"
            },
            new BaseOptionDto()
            {
                Value = MaterialCheckStatus.Rejected, Text = MaterialCheckStatus.Rejected.toDescription(),
                Type = "danger"
            },
            new BaseOptionDto()
                { Value = MaterialCheckStatus.Done, Text = MaterialCheckStatus.Done.toDescription(), Type = "success" }
        };

        return Success(new
        {
            permissionTypes,
            buttonPositions,
            equipmentTypes,
            qualityResult,
            buttonTypes,
            defectCategories,
            stateList,
            boolStateList,
            qualityTypeResult,
            qualified,
            qualifiedTypes,
            materialCheckStatus,
            materialCheckStatusTypes
        });
    }
}