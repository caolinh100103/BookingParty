using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IUserService
{
    Task<ResultDTO<UserReponseDTO>> GetUserById(string token);
    Task<ResultDTO<ICollection<UserReponseDTO>>> GetAllUser();
    Task<ResultDTO<UserReponseDTO>> getUserByUserId(int userId);
    Task<ResultDTO<UserReponseDTO>> UpdateAddressOfUser(AdressUpdatedDTO adressUpdatedDto);
    Task<ResultDTO<bool>> DisableUser(int userId);
}