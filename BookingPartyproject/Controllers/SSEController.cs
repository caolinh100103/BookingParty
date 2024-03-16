using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SSEController : ControllerBase
{
    private readonly ISSEService _sseService;
    public SSEController(ISSEService sseService)
    {
        _sseService = sseService;
    }
    [HttpGet]
    public async Task Get()
    {
        HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");
        await _sseService.SendNotification("abc");
        Task.Delay(1000);
        await _sseService.SendNotification("abc2");
    }
}
