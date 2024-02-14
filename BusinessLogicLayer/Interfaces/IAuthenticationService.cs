using Model.Entity;

namespace BusinessLogicLayer.Interfaces;

public interface IAuthenticationService
{
    Task<User> Login(string UserName, string password);
    Task<int> Register(User user);
}