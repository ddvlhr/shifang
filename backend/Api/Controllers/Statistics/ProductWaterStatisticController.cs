using Core.Dtos.ProductWaterStatistic;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Statistics;

[Authorize]
public class ProductWaterStatisticController : BaseController
{
    private readonly IProductWaterStatisticService _pwsService;

    public ProductWaterStatisticController(IProductWaterStatisticService pwsService)
    {
        _pwsService = pwsService;
    }

    [HttpGet]
    [Route("productWaterStatistic")]
    public IActionResult Get([FromQuery] ProductWaterStatisticQueryInfoDto dto)
    {
        return Success(_pwsService.GetInfo(dto));
    }
}