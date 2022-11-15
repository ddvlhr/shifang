using Infrastructure.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("/api")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success(object data = null, string msg = "")
    {
        return Result(0, msg, data);
    }

    protected IActionResult Error(string msg = "", object data = null)
    {
        return Result(1, msg, data);
    }

    private IActionResult Result(int code, string msg, object data = null)
    {
        return new JsonResult(new Response(code, msg, data));
    }
}