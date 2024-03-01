using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingPartyproject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpGet]
    public async Task<IActionResult> ListAllBlobs()
    {
        var result = await _fileService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var result = await _fileService.UploadAsync(file);
        return Ok(result);
    }

    [HttpGet]
    [Route("filename")]
    public async Task<IActionResult> Download(string fileName)
    {
        var result = await _fileService.DownloadAsync(fileName);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> Delete(string fileName)
    {
        return Ok();
    }
}
