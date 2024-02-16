using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetUserById()
    {
        string tokenAuth = Request.Cookies["token"];
        if (string.IsNullOrEmpty(tokenAuth))
        {
            ResultDTO<string> response = new ResultDTO<string>
            {
                Data = null,
                isSuccess = false,
                Message = "Unauthorized"
            };
            return Unauthorized(response);
        }
        var reponse = await _userService.GetUserById(tokenAuth);
        return Ok(reponse);
    } 
}