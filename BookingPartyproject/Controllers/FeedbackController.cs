using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;


namespace BookingPartyproject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }
    //
    // [HttpGet("{serviceId}")]
    // public async Task<IActionResult> GetFeedBackByServiceId(int serivceId)
    // {
    //     var result = await 
    // } 

    [HttpPost("Create")]
    public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreatedDTO feedbackCreatedDto)
    {
        var result = await _feedbackService.CreateFeedback(feedbackCreatedDto);
        return Ok();
    }
}