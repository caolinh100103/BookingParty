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
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IMapper _mapper;
    public UserService(IGenericRepository<User> userRepository, IMapper mapper,
        IGenericRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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
        var userToken = await _userRepository.GetByProperty(x => x.Email.Equals(emailClaim));
        UserReponseDTO userMapper = null;
        if (userToken != null)
        {
            userMapper = _mapper.Map<UserReponseDTO>(userToken);
            var role = await _roleRepository.GetByProperty(x => x.RoleId == userToken.RoleId);
            var roleMapper = _mapper.Map<RoleResponseDTO>(role);
            userMapper.Role = roleMapper;
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
            var userMapper = _mapper.Map<UserReponseDTO>(user);
            var role = await _roleRepository.GetByProperty(x => x.RoleId == user.RoleId);
            var roleMapper = _mapper.Map<RoleResponseDTO>(role);
            userMapper.Role = roleMapper;
            userReponseDtos.Add(userMapper);
        }
        result = new ResultDTO<ICollection<UserReponseDTO>>()
        {
            Data = userReponseDtos,
            isSuccess = true,
            Message = "Return list of User"
        };
        return result;
    }

    public async Task<ResultDTO<UserReponseDTO>> getUserByUserId(int userId)
    {
        var user = await _userRepository.GetByProperty(x => x.UserId == userId);
        if (user != null)
        {
            var userResponse = _mapper.Map<UserReponseDTO>(user);
            var role = await _roleRepository.GetByProperty(x => x.RoleId == user.RoleId);
            var roleMapper = _mapper.Map<RoleResponseDTO>(role);
            userResponse.Role = roleMapper;
            return new ResultDTO<UserReponseDTO>()
            {
                Data = userResponse,
                isSuccess = true,
                Message = "Return user by userID"
            };
        }

        return new ResultDTO<UserReponseDTO>()
        {
            Data = null,
            isSuccess = false,
            Message = "Can not return user by wrong Id"
        };
    }

    public async Task<ResultDTO<UserReponseDTO>> UpdateAddressOfUser(AdressUpdatedDTO adressUpdatedDto)
    {
        User userUpdated = null;
        var user = await _userRepository.GetByProperty(x => x.UserId == adressUpdatedDto.UserId);
        if (user != null)
        {
            user.Address = adressUpdatedDto.Address;
            userUpdated = await _userRepository.UpdateAsync(user);
        }

        if (userUpdated != null)
        {
            var userReponse = _mapper.Map<UserReponseDTO>(userUpdated);
            var role = await _roleRepository.GetByProperty(x => x.RoleId == userUpdated.RoleId);
            var roleReponse = _mapper.Map<RoleResponseDTO>(role);
            userReponse.Role = roleReponse;
            return new ResultDTO<UserReponseDTO>()
            {
                Data = userReponse,
                isSuccess = true,
                Message = "Return user updated"
            };
        }

        return new ResultDTO<UserReponseDTO>()
        {
            Data = null,
            isSuccess = false,
            Message = "The Id is not exist"
        };
    }
}