using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class AuthenticationService : IAuthenticationService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public AuthenticationService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }
    public async Task<User> Login(string email, string password)
    {
        var user = await _userRepository.GetByProperty(x => x.Email.Equals(email));
        if (user == null)
        {
            return null;
        }
        else
        {
            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (SHA256Helper.verifyPassword(password, user.Password))
            {
                user.Role = role;
                return user;
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<ResultDTO<UserReponseDTO>> Register (RegisterDTO registerDto)
    {
        var userCheck = await _userRepository.GetByProperty(x => x.Email.Equals(registerDto.Email));
        if (userCheck != null)
        {
            return new ResultDTO<UserReponseDTO>()
            {
                Data = null,
                isSuccess = false,
                Message = "The Email has been registered"
            };
        }
        else
        {
            var userMapper = _mapper.Map<User>(registerDto);
            userMapper.Status = UserStatus.NON_ACTIVE;
            var user = await _userRepository.AddAsync(userMapper);
            if (user != null)
            {
                var userResponse = _mapper.Map<UserReponseDTO>(user);
                return new ResultDTO<UserReponseDTO>()
                {
                    Data = userResponse,
                    isSuccess = true,
                    Message = "Register a new user successfully"
                };
            }

            return new ResultDTO<UserReponseDTO>()
            {
                Data = null,
                isSuccess = false,
                Message = "Internal Server"
            };
        }
    }

    public async Task<ResultDTO<UserReponseDTO>> BanUser(int UserId)
    {
        var user = await _userRepository.GetByIdAsync(UserId);
        if (user != null)
        {
            if (user.Status == UserStatus.NON_ACTIVE)
            {
                return new ResultDTO<UserReponseDTO>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "The user has been banned before from this web"
                };
            }
            else
            {
                user.Status = UserStatus.NON_ACTIVE;
                var userUpdated = await _userRepository.UpdateAsync(user);
                var userReponse = _mapper.Map<UserReponseDTO>(user);
                return new ResultDTO<UserReponseDTO>()
                {
                    Data  = userReponse,
                    isSuccess = true,
                    Message = "The user has been banned"
                };
            }
        }

        return new ResultDTO<UserReponseDTO>()
        {
            Data = null,
            isSuccess = false,
            Message = "Internal Server"
        };
    }
}