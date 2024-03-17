using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WithdrawalController : ControllerBase
{
    private readonly IWithdrawalService _withdrawalService;
    public WithdrawalController(IWithdrawalService withdrawalService)
    {
        _withdrawalService = withdrawalService;
    }
    [HttpPost]
    public async Task<IActionResult> Withdrawal([FromBody] WithdrawalCreatedDTO withdrawalCreatedDto)
    {
        var result = await _withdrawalService.Create(withdrawalCreatedDto);
        return Ok(result);
    }

    [HttpGet("get_all_withdrawal")]
    public async Task<IActionResult> GetAllWithdrawal()
    {
        var result = await _withdrawalService.GetAllWithDrawal();
        return Ok(result);
    }
    [HttpPut("confirm_withdrawal/{withdrawalId}")]
    public async Task<IActionResult> ConfirmWithDrawal(int withdrawalId)
    {
        var res = await _withdrawalService.Confirm(withdrawalId);
        return Ok(res);
    }
}