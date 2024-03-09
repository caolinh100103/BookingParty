using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Model.DTO;
using Model.Entity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace BusinessLogicLayer;

public class AuthenticationService : IAuthenticationService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IEmailService _emailService;
    public AuthenticationService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository,
        IMapper mapper,  IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _urlHelperFactory = urlHelperFactory;
        _emailService = emailService;
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
            userMapper.EmailConfirmationToken = GeneratorDigits.GenerateSixDigitCode();
            userMapper.Password = SHA256Helper.Hash(userMapper.Password); 
            var user = await _userRepository.AddAsync(userMapper);
            var role = await _roleRepository.GetByIdAsync(registerDto.RoleId);
            user.Role = role;
            if (user != null)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var actionContext = new ActionContext(request.HttpContext, new RouteData(), new ActionDescriptor());
                var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
                var userResponse = _mapper.Map<UserReponseDTO>(user);
                string frontendUrl = $"https://yourfrontendpage.com/verify?userEmail={user.Email}"; // Update with your frontend URL, The email I pass in parameter
                string emailBody = $"Please use the following verification code to verify your email: {user.EmailConfirmationToken}. " +
                                   $"Alternatively, you can click <a href=\"{frontendUrl}\">here</a> to return to the frontend page.";
                await _emailService.SendEmailAsync(user.Email, "Email Verification", emailBody);
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

    public async Task<ResultDTO<bool>> VerifyAccount(VerifyAccountDTO verifyAccountDto)
    {
        var user = await _userRepository.GetByProperty(x => x.Email.Equals(verifyAccountDto.Email));
        if (user != null)
        {
            if (user.EmailConfirmationToken.Equals(verifyAccountDto.tokenConfirm))
            {
                user.Status = UserStatus.ACTIVE;
                _ = await _userRepository.UpdateAsync(user);
                return new ResultDTO<bool>()
                {
                    Data = true,
                    isSuccess = true,
                    Message = "Verify the account successfully"
                };
            }
            else
            {
                return new ResultDTO<bool>()
                {
                    isSuccess = false,
                    Data = false,
                    Message = "The confirmation is fail due to wrong token"
                };
            }
        }

        return new ResultDTO<bool>()
        {
            Data = false,
            isSuccess = false,
            Message = "The user get from email null"
        };
    }
}