using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Consumes("application/json", "multipart/form-data")]
[Route("api/files")]
public class FileController : BaseController
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IWriteAbleOptions<Core.Models.Settings> _writeAbleSettings;

    public FileController(IWebHostEnvironment webHostEnvironment,
        IWriteAbleOptions<Core.Models.Settings> writeAbleSettings)
    {
        _webHostEnvironment = webHostEnvironment;
        _writeAbleSettings = writeAbleSettings;
    }

    [HttpPost]
    [Route("images")]
    public async Task<IActionResult> ImageUpload(IFormFile file)
    {
        try
        {
            if (file != null)
            {
                var rootPath = _webHostEnvironment.WebRootPath;
                var upFilePath = rootPath + "/UploadFiles/Images";

                var contentType = file.ContentType;
                var originName = file.FileName;
                var fileExtension = Path.GetExtension(originName);
                var fileName = Guid.NewGuid().ToString("d") + fileExtension;
                var storeName = Path.Combine(upFilePath, fileName);
                using (var fileStream = new FileStream(storeName, FileMode.OpenOrCreate, FileAccess
                           .ReadWrite, FileShare.ReadWrite))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                return Success(fileName);
            }

            return Error("没有找到文件");
        }
        catch (Exception exception)
        {
            return Error(exception.Message);
        }
    }

    [HttpPost]
    [Route("welcomeImages/update")]
    public IActionResult UpdateWelcomeImages([FromBody] List<string> fileNames)
    {
        _writeAbleSettings.Update(opt => { opt.WelcomeImages = fileNames[0]; });

        return Success();
    }

    [HttpGet]
    [Route("welcomeImages")]
    public IActionResult GetWelcomeImages()
    {
        var settings = _writeAbleSettings.Value;
        return Success(settings.WelcomeImages);
    }
}