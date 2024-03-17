using BusinessLogicLayer;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingPartyproject.Controllers;
[Route("/api/sse")]
[ApiController]
public class SSEController : ControllerBase
{
    private readonly SSEService _sseService;
    
    public SSEController(SSEService sseService)
    {
        _sseService = sseService;
    }
    // [HttpGet]
    // public async Task Get()
    // {
    //     HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");
    //     await _sseService.SendNotification("abc");
    //     Task.Delay(1000);
    //     await _sseService.SendNotification("abc2");
    // }
    [HttpGet]
    public async Task Get(int userId)
    {
        Response.Headers.Add("Content-Type", "text/event-stream");

        var responseStreamWriter = new StreamWriter(Response.Body);
        _sseService.AddConnection(35, responseStreamWriter);

        try
        {
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Simulate notifications
                var notificationMessage = "No NO no";

                await responseStreamWriter.WriteLineAsync($"data: {notificationMessage}\n\n");
                await responseStreamWriter.FlushAsync();

                await Task.Delay(TimeSpan.FromSeconds(5)); // You can adjust the delay as needed
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions if any
        }
        // finally
        // {
        //     _sseService.RemoveConnection(35);
        // }
    }
}
