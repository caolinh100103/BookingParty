using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IDepositService
{ 
    Task<ResultDTO<DepositResponseDTO>> CreateDeposit(DepositCreatedDTO depositCreatedDto);
}