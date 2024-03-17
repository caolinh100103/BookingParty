using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IWithdrawalService
{
    Task<ResultDTO<bool>> Create(WithdrawalCreatedDTO withdrawalCreatedDto);
    Task<ResultDTO<ICollection<WithDrawalReponseDTO>>> GetAllWithDrawal();
    Task<ResultDTO<bool>> Confirm(int withdrawalid);
}