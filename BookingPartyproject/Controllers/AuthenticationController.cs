using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using BusinessLogicLayer.Interfaces;
using Model.DTO;
using Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace BookingPartyproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _service;

        private readonly IConfiguration _configuration;
        public AuthenticationController(IMapper mapper, IAuthenticationService service, IConfiguration configuration) 
        {
             _mapper = mapper;
             _service = service;
             _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO) 
        {
            if (userDTO == null || userDTO.FullName == null || userDTO.Password == null || userDTO.Email == null)
            {
                return BadRequest("Null User");
            }
            else
            {
                var user = _mapper.Map<User>(userDTO);
                int checkRegister = await _service.Register(user);
                if (checkRegister > 0)
                {
                    return Ok("Register Successfully;");
                }
                else
                {
                    return BadRequest("Can not register");
                }
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            if (user == null || user.Email.IsNullOrEmpty() || user.Password.IsNullOrEmpty())
            {
                return BadRequest("The User is null");
            }
            else
            {
                var userRespnse = await _service.Login(user.Email, user.Password);
                if (userRespnse != null)
                {
                    var token = new TokenDTO
                    {
                        FullName = userRespnse.FullName,
                        Email = userRespnse.Email,
                        RoleName = userRespnse.Role.RoleName
                    };
                    string tokenGen = GenerateToken(token);
                    return Ok(tokenGen);
                }
                else
                {
                    return BadRequest("Wrong User");
                }
            }
        }
        
        private string GenerateToken(TokenDTO userToken)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userToken.FullName),
                new Claim(ClaimTypes.Email, userToken.Email),
                new Claim(ClaimTypes.Role, userToken.RoleName)
            };
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                null,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            HttpContext.Response.Cookies.Append("token", jwtToken, 
                new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            return jwtToken;
        }
    }
}
