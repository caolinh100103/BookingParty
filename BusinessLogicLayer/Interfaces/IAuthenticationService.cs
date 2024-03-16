using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer.Interfaces;

public interface IAuthenticationService
{
    Task<User> Login(string UserName, string password);
    Task<ResultDTO<UserReponseDTO>> Register(RegisterDTO registerDto);
    Task<ResultDTO<UserReponseDTO>> BanUser(int UserId);
    Task<ResultDTO<bool>> VerifyAccount(VerifyAccountDTO verifyAccountDto);
}