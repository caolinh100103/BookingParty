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
    [Authorize(Roles = "Customer,Party Host")]
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
    
    [HttpGet("/api/users")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _userService.GetAllUser();
        return Ok(result);
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserByUserId(int userId)
    {
        var result = await _userService.getUserByUserId(userId);
        return Ok(result);
    }

    [HttpPut("updateAddress/{userId}")]
    public async Task<IActionResult> UpdateUserAdress(int userid, AdressUpdatedDTO adressUpdatedDto)
    {
        var result = await _userService.UpdateAddressOfUser(adressUpdatedDto);
        return Ok(result);
    }
    
    // [HttpPut("user/{userId}")]
    // public async Task<IActionResult> UpdateUserInformation(int userId, UserDTO)
}