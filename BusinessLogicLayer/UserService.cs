using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;
    public UserService(IGenericRepository<User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<ResultDTO<UserReponseDTO>> GetUserById(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken  = tokenHandler.ReadToken(token) as SecurityToken;

        // Access the user's email claim
        string emailClaim = null;
        if (securityToken is JwtSecurityToken jwtToken)
        {
            // Access the user's email claim
            emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
        var user = await _userRepository.GetByProperty(x => x.Email.Equals(emailClaim));
        UserReponseDTO userMapper = null;
        if (user != null)
        {
            userMapper = _mapper.Map<UserReponseDTO>(user);
        }

        ResultDTO<UserReponseDTO> response = new ResultDTO<UserReponseDTO>
        {
            Data = userMapper,
            isSuccess = true,
            Message = "Return user succefully"
        };
        return response;
    }

    public async Task<ResultDTO<ICollection<UserReponseDTO>>> GetAllUser()
    {
        ResultDTO<ICollection<UserReponseDTO>> result = null;
        ICollection<UserReponseDTO> userReponseDtos = new List<UserReponseDTO>();
        var users = await _userRepository.GetAllAsync();
        foreach (var user in users)
        {
            userReponseDtos.Add(_mapper.Map<UserReponseDTO>(user));
        }
        result = new ResultDTO<ICollection<UserReponseDTO>>()
        {
            Data = userReponseDtos,
            isSuccess = true,
            Message = "Return list of User"
        };
        return result;
    }
}