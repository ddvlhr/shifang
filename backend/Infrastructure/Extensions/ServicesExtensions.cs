using Infrastructure.Services.Authorize;
using Infrastructure.Services.Authorize.Impl;
using Infrastructure.Services.BaseData;
using Infrastructure.Services.BaseData.Impl;
using Infrastructure.Services.Indicator;
using Infrastructure.Services.Indicator.Impl;
using Infrastructure.Services.Material;
using Infrastructure.Services.Material.Impl;
using Infrastructure.Services.MetricalData;
using Infrastructure.Services.MetricalData.Impl;
using Infrastructure.Services.Reports;
using Infrastructure.Services.Reports.Impl;
using Infrastructure.Services.Specification;
using Infrastructure.Services.Specification.Impl;
using Infrastructure.Services.Statistics;
using Infrastructure.Services.Statistics.Impl;
using Infrastructure.Services.System;
using Infrastructure.Services.System.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServicesExtensions
{
    public static void LoadServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticateService, AuthenticateService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IWorkShopService, WorkShopService>();
        services.AddScoped<IMachineModelService, MachineModelService>();
        services.AddScoped<ITurnService, TurnService>();
        services.AddScoped<IMachineService, MachineService>();
        services.AddScoped<IMeasureTypeService, MeasureTypeService>();
        services.AddScoped<ISpecificationTypeService, SpecificationTypeService>();
        services.AddScoped<IIndicatorParentService, IndicatorParentService>();
        services.AddScoped<IIndicatorService, IndicatorService>();
        services.AddScoped<ISpecificationService, SpecificationService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMetricalDataService, MetricalDataService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IMeasureTypeIndicatorsService, MeasureTypeIndicatorsService>();
        services.AddScoped<IPhysicalReportService, PhysicalReportService>();
        services.AddScoped<IPhysicalReportAppearanceService, PhysicalReportAppearanceService>();
        services.AddScoped<IInspectionReportService, InspectionReportService>();
        services.AddScoped<IInspectionReportAppearanceService, InspectionReportAppearanceService>();
        services.AddScoped<IProductReportService, ProductReportService>();
        services.AddScoped<IProductReportAppearanceService, ProductReportAppearanceService>();
        services.AddScoped<IManufacturerService, ManufacturerService>();
        services.AddScoped<IMaterialTemplateService, MaterialTemplateService>();
        services.AddScoped<IMaterialReportService, MaterialReportService>();
        services.AddScoped<ISpecificationTypeRuleService, SpecificationTypeRuleService>();
        services.AddScoped<IWaterRecordService, WaterRecordService>();
        services.AddScoped<IFactoryReportService, FactoryReportService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<ICraftIndicatorRuleService, CraftIndicatorRuleService>();
        services.AddScoped<ICraftReportService, CraftReportService>();
        services.AddScoped<IWorShopQualityPointRuleService, WorkShopQualityPointRuleService>();
        services.AddScoped<IWorkShopQualityService, WorkShopQualityService>();
        services.AddScoped<IProductWaterStatisticService, ProductWaterStatisticService>();
        services.AddScoped<IQuantityStatisticService, QuantityStatisticService>();
        services.AddScoped<IFactorySiteService, FactorySiteService>();
        services.AddScoped<IMethodService, MethodService>();
        services.AddScoped<IAppearanceStatisticService, AppearanceStatisticService>();
        services.AddScoped<IGroupRecordService, GroupRecordService>();
        services.AddScoped<IFilterMeasureQualityService, FilterMeasureQualityService>();
        services.AddScoped<IFilterStandardDeviationStatisticService, FilterStandardDeviationStatisticService>();
        services.AddScoped<IReportOrderService, ReportOrderService>();
        services.AddScoped<IMonthCraftReportService, MonthCraftReportService>();
        services.AddScoped<ITestReportService, TestReportService>();
        services.AddScoped<ITestReportAppearanceService, TestReportAppearanceService>();
        services.AddScoped<ICraftAssessmentService, CraftAssessmentService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ITeamService, TeamService>();
    }
}