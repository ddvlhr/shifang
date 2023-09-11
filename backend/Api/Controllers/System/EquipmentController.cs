using Infrastructure.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

public class EquipmentController : BaseController
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet("equipments")]
    public IActionResult GetEquipments()
    {
        return Success(_equipmentService.GetEquipments());
    }
}