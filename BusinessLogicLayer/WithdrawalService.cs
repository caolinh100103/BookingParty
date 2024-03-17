using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class WithdrawalService : IWithdrawalService
{
    private readonly IGenericRepository<WithdrawalRequest> _withdrawalRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public WithdrawalService(IGenericRepository<WithdrawalRequest> withdrawalRepository, IMapper mapper, IGenericRepository<User> userRepository)
    {
        _withdrawalRepository = withdrawalRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<ResultDTO<bool>> Create(WithdrawalCreatedDTO withdrawalCreatedDto)
    {
        var user = await _userRepository.GetByProperty(x => x.UserId == withdrawalCreatedDto.UserId);
        if (user != null)
        {
            if (user.Balance < withdrawalCreatedDto.Amount)
            {
                return new ResultDTO<bool>()
                {
                    Data = false,
                    isSuccess = false,
                    Message = "can not withdrawal with the amount over balance"
                };
            }
        }
        WithdrawalRequest withdrawalRequest = new WithdrawalRequest()
        {
            UserId = withdrawalCreatedDto.UserId,
            Created = DateTime.Now,
            Amount = withdrawalCreatedDto.Amount
        };
        var withdrwalCre = await _withdrawalRepository.AddAsync(withdrawalRequest);
        if (withdrwalCre != null)
        {
            return new ResultDTO<bool>()
            {
                Data = true,
                Message = "Created successfully",
                isSuccess = true
            };
        }
        return new ResultDTO<bool>()
        {
            Data = false,
            Message = "Created not successfully",
            isSuccess = false
        };
    }

    public async Task<ResultDTO<ICollection<WithDrawalReponseDTO>>> GetAllWithDrawal()
    {
        ICollection<WithDrawalReponseDTO> reponseDtos = new List<WithDrawalReponseDTO>();
        var withdrawals = await _withdrawalRepository.GetAllAsync();
        foreach (var withdrawal in withdrawals)
        {
            var withdrawalMapper = _mapper.Map<WithDrawalReponseDTO>(withdrawal);
            reponseDtos.Add(withdrawalMapper);
        }

        return new ResultDTO<ICollection<WithDrawalReponseDTO>>()
        {
            Data = reponseDtos,
            isSuccess = true,
            Message = "Withdrawal requests"
        };
    }

    public async Task<ResultDTO<bool>> Confirm(int withdrawalid)
    {
        var withDrawal = await _withdrawalRepository.GetByProperty(x => x.Id == withdrawalid);
        var partyHost = await _userRepository.GetByProperty(x => x.UserId == withDrawal.UserId);
        var partyHostBalance = partyHost.Balance;
        partyHost.Balance = partyHostBalance - withDrawal.Amount;
        _ = await _userRepository.UpdateAsync(partyHost);
        return new ResultDTO<bool>()
        {
            isSuccess = true,
            Data = true,
            Message = "Confirm Successfully"
        };
    }
}