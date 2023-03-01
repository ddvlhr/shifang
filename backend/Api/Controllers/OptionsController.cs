using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;
using Infrastructure.Extensions;
using Infrastructure.Services.BaseData;
using Infrastructure.Services.Defect;
using Infrastructure.Services.Indicator;
using Infrastructure.Services.Specification;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
public class OptionsController : BaseController
{
    private readonly IFactorySiteService _fsService;
    private readonly IIndicatorService _iService;
    private readonly IMachineService _macService;
    private readonly IMethodService _methodService;
    private readonly IMachineModelService _mmService;
    private readonly IModelService _modelService;
    private readonly IManufacturerService _mService;
    private readonly IMeasureTypeService _mtService;
    private readonly IRoleService _roleService;
    private readonly IReportOrderService _roService;
    private readonly IDefectService _defectService;
    private readonly ISpecificationService _speService;
    private readonly ISpecificationTypeService _stService;
    private readonly ITurnService _turnService;
    private readonly IUserService _uService;
    private readonly IWorkShopService _wsService;
    private readonly IDepartmentService _departmentService;
    private readonly IDisciplineClassService _disciplineClassService;
    private readonly IDisciplineClauseService _disciplineClauseService;

    public OptionsController(
        ISpecificationService speService,
        ITurnService turnService,
        IMachineService macService,
        IMeasureTypeService mtService,
        IIndicatorService iService,
        ISpecificationTypeService stService,
        IUserService uService,
        IMachineModelService mmService,
        IWorkShopService wsService,
        IManufacturerService mService,
        IModelService modelService,
        IRoleService roleService,
        IFactorySiteService fsService,
        IMethodService methodService,
        IReportOrderService roService,
        IDefectService defectService,
        IDisciplineClassService disciplineClassService,
        IDisciplineClauseService disciplineClauseService,
        IDepartmentService departmentService)
    {
        _speService = speService;
        _turnService = turnService;
        _macService = macService;
        _mtService = mtService;
        _iService = iService;
        _stService = stService;
        _uService = uService;
        _mmService = mmService;
        _mService = mService;
        _modelService = modelService;
        _roleService = roleService;
        _fsService = fsService;
        _methodService = methodService;
        _roService = roService;
        _defectService = defectService;
        _wsService = wsService;
        _departmentService = departmentService;
        _disciplineClassService = disciplineClassService;
        _disciplineClauseService = disciplineClauseService;
    }

    [HttpGet]
    [Route("options")]
    public async Task<IActionResult> Get()
    {
        var specifications = _speService.GetOptions();

        var turns = _turnService.GetOptions();

        var machines = _macService.GetOptions();

        var measureTypes = _mtService.GetOptions();

        var specificationTypes = _stService.GetOptions();

        var measureIndicators = _iService.GetIndicatorOptions(IndicatorProject.Measure);

        var indicators = _iService.GetIndicatorOptions(IndicatorProject.None);

        var appearanceIndicators = _iService.GetIndicatorOptions(IndicatorProject.Appearance);

        var users = _uService.GetOptions();

        var machineModels = _mmService.GetOptions();

        var workShops = _wsService.GetOptions(false);

        var noRoleUsers = _uService.GetNoRoleUsers();

        var manufacturers = _mService.GetOptions();

        var models = _modelService.GetOptions();

        var roles = _roleService.GetOptions();

        var factorySites = _fsService.GetOptions();

        var methods = _methodService.GetOptions();

        var filterStatisticPlotTypes = typeof(CustomerEnum.FilterStatisticPlotType).ToOptions();

        var reportOrders = _roService.GetOptions();

        var defects = _defectService.GetOptions();

        var departments = _departmentService.GetOptions();

        var disciplineClasses = await _disciplineClassService.GetOptionsAsync();

        var disciplineClauses = await _disciplineClauseService.GetOptionsAsync();
        
        var equipmentTypes = typeof(EquipmentType).ToOptions();

        return Success(new
        {
            specifications,
            turns,
            machines,
            measureTypes,
            indicators,
            appearanceIndicators,
            measureIndicators,
            specificationTypes,
            users,
            machineModels,
            workShops,
            noRoleUsers,
            manufacturers,
            models,
            roles,
            factorySites,
            methods,
            filterStatisticPlotTypes,
            reportOrders,
            defects,
            departments,
            disciplineClasses,
            disciplineClauses,
            equipmentTypes
        });
    }
}