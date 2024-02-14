using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DepositController : ControllerBase
{
    private readonly IDepositService _depositService;
    public DepositController(IDepositService depositService)
    {
        _depositService = depositService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDeposit([FromBody] DepositCreatedDTO depositCreatedDto)
    {
        var response = await _depositService.CreateDeposit(depositCreatedDto);
        return Ok(response);
    }
}