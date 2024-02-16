using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IUserService
{
    Task<ResultDTO<UserReponseDTO>> GetUserById(string token);
}