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

    public async Task<int> Register(User user)
    {
        var userCheck = await _userRepository.GetByProperty(x => x.Email.Equals(user.Email));
        if (userCheck != null)
        {
            return 0;
        }
        else
        {
            user.Status = UserStatus.ACTIVE; // chua impl verify
            user.RoleId = 1;
            var passwordHashed = SHA256Helper.Hash(user.Password);
            user.Password = passwordHashed;
            var checkRegister = await _userRepository.AddAsync(user);
            if (checkRegister != null)
            {
                return 1;
            }
        }
        return 0;
    }
}