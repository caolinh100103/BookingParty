using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class DepositService : IDepositService
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<TransactionHistory> _transactionRepository;
    private readonly IGenericRepository<Deposit> _depositRepository;
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<BookingDetail> _bookingDetailRepository;
    private readonly IGenericRepository<Service> _serviceRepository;
    public DepositService(IMapper mapper, IGenericRepository<TransactionHistory> transactionRepository,
        IGenericRepository<Deposit> depositRepository,
        IGenericRepository<Booking> bookingRepository,
        IGenericRepository<Notification> notificationRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<Room> roomRepository,
        IGenericRepository<BookingDetail> bookingDetailRepository,
        IGenericRepository<Service> serviceRepository)
    {
        _mapper = mapper;
        _transactionRepository = transactionRepository;
        _depositRepository = depositRepository;
        _bookingRepository = bookingRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _bookingDetailRepository = bookingDetailRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<ResultDTO<DepositResponseDTO>> CreateDeposit(DepositCreatedDTO depositCreatedDto)
    {
        //Check Booking co deposit dung han ko (Check theo hop dong)


        // 0 FAIL(COD),  1 Success, 2 Refunded
        var booking = await _bookingRepository.GetByProperty(x => x.BookingId == depositCreatedDto.BookingId);
        if (depositCreatedDto == null)
        {
            ResultDTO<DepositResponseDTO> resultDto = new ResultDTO<DepositResponseDTO>
            {
                Data = null,
                isSuccess = false,
                Message = "Can not pay for deposit"
            };
            return resultDto;
        }

        DepositDTO depositDto = new DepositDTO
        {
            Content = $"Deposit for booking No.{depositCreatedDto.BookingId}",
            Title = "Deposit booking",
            Percentage = 50,
            BookingId = depositCreatedDto.BookingId
        };
        var depositMapper = _mapper.Map<Deposit>(depositDto);
        var deposit = await _depositRepository.AddAsync(depositMapper);
        if (deposit != null)
        {
                TransactionCreatedDTO transactionCreatedDto = new TransactionCreatedDTO
                {
                    Txn_ref = null,
                    PaymentMethod = "COD",
                    BankCode = null,
                    TransactionDate = DateTime.Now.ToString(),
                    Amount = (booking.TotalPrice) / 2
                };
                var transactionMapper = _mapper.Map<TransactionHistory>(transactionCreatedDto);
                var transaction = await _transactionRepository.AddAsync(transactionMapper);
                transaction.DepositId = deposit.DepositId;
                transaction.Status = TransactionStatus.FAIL;
                var transactionUpdate = await _transactionRepository.UpdateAsync(transaction);
                if (transaction != null)
                {
                    // var booking = await _bookingRepository.GetByProperty(x => x.BookingId == deposit.BookingId);
                    booking.Status = BookingStatus.DEPOSITED;
                    _ = await _bookingRepository.UpdateAsync(booking);
                    DepositResponseDTO responseDto = new DepositResponseDTO()
                    {
                        DepositId = deposit.DepositId,
                        Percentage = 50,
                        Title = deposit.Title,
                        Content = deposit.Content,
                        Booking = booking
                    };
                    createNotification(depositCreatedDto);
                    var bookingDetaiList = await _bookingDetailRepository.GetListByProperty(x => x.BookingId == booking.BookingId) ;
                    var anyBookingDetail = bookingDetaiList.ElementAt(0);
                    var room = await _roomRepository.GetByProperty(x => x.RoomId == anyBookingDetail.RoomId);
                    var partyHost = await _userRepository.GetByProperty(x => x.UserId == room.UserId);
                    var balance = partyHost.Balance;
                    partyHost.Balance = balance + (transaction.Amount * 80 / 100);
                    _ = await _userRepository.UpdateAsync(partyHost);
                    // get voi service
                    foreach (var bookingDetail in bookingDetaiList)
                    {
                        var service = await _serviceRepository.GetByProperty(x => x.ServiceId == bookingDetail.ServiceId);
                        var partyHostService = await _userRepository.GetByProperty(x => x.UserId == service.UserId);
                        var balanceService = partyHost.Balance;
                        partyHost.Balance = balanceService + (transaction.Amount * 80 / 100);
                        _ = await _userRepository.UpdateAsync(partyHostService);
                    }
                    var result = new ResultDTO<DepositResponseDTO>()
                    {
                        Message = "Successfully pay with COD",
                        Data = responseDto,
                        isSuccess = true
                    };
                    return result;
                }
        }
        var response = new ResultDTO<DepositResponseDTO>()
        {
            Message = "Can not pay",
            isSuccess = false,
            Data = null
        };
        return response;
    }

    private async void updateStatusBooking(Booking booking)
    {
        if (booking.Status == BookingStatus.BOOKED)
        {
            booking.Status = BookingStatus.DEPOSITED;
            var updateBookingStatus = await _bookingRepository.UpdateAsync(booking);
        }
        else if (booking.Status == BookingStatus.DEPOSITED)
        {
            booking.Status = BookingStatus.FINISHED;
            var updateBookingSattus = await _bookingRepository.UpdateAsync(booking);
        }
    }

    private async void createNotification(DepositCreatedDTO depositCreatedDto)
    {
        var booking = await _bookingRepository.GetByProperty(x => x.BookingId == depositCreatedDto.BookingId);
        if (booking != null)
        {
            var user = await _userRepository.GetByProperty(x => x.UserId == booking.UserId);
            if (user != null)
            {
                Notification noti = new Notification()
                {
                    UserId = user.UserId,
                    Content = $"Deposit for Booking No.{booking.BookingId} successfully",
                    Title = "Create Deposit",
                    SentTime = DateTime.Now
                };
            }
        }
    }
}