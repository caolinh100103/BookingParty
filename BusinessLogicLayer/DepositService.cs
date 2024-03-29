// using AutoMapper;
// using BusinessLogicLayer.Enum;
// using BusinessLogicLayer.Interfaces;
// using DataAccessLayer.Interface;
// using Model.DTO;
// using Model.Entity;
//
// namespace BusinessLogicLayer;
//
// public class DepositService : IDepositService
// {
//     private readonly IMapper _mapper;
//     private readonly IGenericRepository<TransactionHistory> _transactionRepository;
//     private readonly IGenericRepository<Deposit> _depositRepository;
//     private readonly IGenericRepository<Booking> _bookingRepository;
//     private readonly IGenericRepository<Notification> _notificationRepository;
//     private readonly IGenericRepository<User> _userRepository;
//
//     public DepositService(IMapper mapper, IGenericRepository<TransactionHistory> transactionRepository,
//         IGenericRepository<Deposit> depositRepository,
//         IGenericRepository<Booking> bookingRepository,
//         IGenericRepository<Notification> notificationRepository,
//         IGenericRepository<User> userRepository)
//     {
//         _mapper = mapper;
//         _transactionRepository = transactionRepository;
//         _depositRepository = depositRepository;
//         _bookingRepository = bookingRepository;
//         _notificationRepository = notificationRepository;
//         _userRepository = userRepository;
//     }
//
//     public async Task<ResultDTO<DepositResponseDTO>> CreateDeposit(DepositCreatedDTO depositCreatedDto)
//     {
//         //Check Booking co deposit dung han ko (Check theo hop dong)
//
//
//         // 0 FAIL(COD),  1 Success, 2 Refunded
//         if (depositCreatedDto == null)
//         {
//             ResultDTO<DepositResponseDTO> resultDto = new ResultDTO<DepositResponseDTO>
//             {
//                 Data = null,
//                 isSuccess = false,
//                 Message = "Can not pay for deposit"
//             };
//             return resultDto;
//         }
//
//         DepositDTO depositDto = new DepositDTO
//         {
//             Content = depositCreatedDto.Content,
//             Title = depositCreatedDto.Title,
//             Percentage = 50,
//             BookingId = depositCreatedDto.BookingId
//         };
//         var depositMapper = _mapper.Map<Deposit>(depositDto);
//         var deposit = await _depositRepository.AddAsync(depositMapper);
//         if (deposit != null)
//         {
//             if (depositCreatedDto.TransactionCreatedDto.PaymentMethod.Equals("COD"))
//             {
//                 TransactionCreatedDTO transactionCreatedDto = new TransactionCreatedDTO
//                 {
//                     Txn_ref = null,
//                     PaymentMethod = depositCreatedDto.TransactionCreatedDto.PaymentMethod,
//                     BankCode = null,
//                     TransactionDate = depositCreatedDto.TransactionCreatedDto.TransactionDate,
//                     Amount = depositCreatedDto.TransactionCreatedDto.Amount
//                 };
//                 var transactionMapper = _mapper.Map<TransactionHistory>(transactionCreatedDto);
//                 var transaction = await _transactionRepository.AddAsync(transactionMapper);
//                 transaction.DepositId = deposit.DepositId;
//                 transaction.Status = TransactionStatus.FAIL;
//                 var transactionUpdate = await _transactionRepository.UpdateAsync(transaction);
//                 if (transaction != null)
//                 {
//                     var booking = await _bookingRepository.GetByProperty(x => x.BookingId == deposit.BookingId);
//                     updateStatusBooking(booking);
//                     DepositResponseDTO responseDto = new DepositResponseDTO()
//                     {
//                         DepositId = deposit.DepositId,
//                         Percentage = 50,
//                         Title = deposit.Title,
//                         Content = deposit.Content,
//                         Booking = booking
//                     };
//                     createNotification(depositCreatedDto);
//                     var result = new ResultDTO<DepositResponseDTO>()
//                     {
//                         Message = "Successfully pay with COD",
//                         Data = responseDto,
//                         isSuccess = true
//                     };
//                     return result;
//                 }
//             }
//             else if (depositCreatedDto.TransactionCreatedDto.PaymentMethod.Equals("VNPAY") && depositCreatedDto.TransactionCreatedDto.Status == "00")
//             {
//                 TransactionCreatedDTO transactionCreatedDto = new TransactionCreatedDTO
//                 {
//                     Txn_ref = depositCreatedDto.TransactionCreatedDto.Txn_ref,
//                     PaymentMethod = depositCreatedDto.TransactionCreatedDto.PaymentMethod,
//                     BankCode = depositCreatedDto.TransactionCreatedDto.BankCode,
//                     TransactionDate = depositCreatedDto.TransactionCreatedDto.TransactionDate,
//                     Amount = depositCreatedDto.TransactionCreatedDto.Amount
//                 };
//                 var transactionMapper = _mapper.Map<TransactionHistory>(transactionCreatedDto);
//                 var transaction = await _transactionRepository.AddAsync(transactionMapper);
//                 transaction.DepositId = deposit.DepositId;
//                 transaction.Status = TransactionStatus.SUCCESS;
//                 var transactionUpdate = await _transactionRepository.UpdateAsync(transaction);
//                 if (transaction != null)
//                 {
//                     var booking = await _bookingRepository.GetByProperty(x => x.BookingId == deposit.BookingId);
//                     updateStatusBooking(booking);
//                     DepositResponseDTO responseDto = new DepositResponseDTO()
//                     {
//                         DepositId = deposit.DepositId,
//                         Percentage = 50,
//                         Title = deposit.Title,
//                         Content = deposit.Content,
//                         Booking = booking
//                     };
//                     createNotification(depositCreatedDto);
//                     var result = new ResultDTO<DepositResponseDTO>()
//                     {
//                         Message = "Successfully pay with VNPAY",
//                         Data = responseDto,
//                         isSuccess = true
//                     };
//                     return result;
//                 }
//             }
//         }
//
//         var response = new ResultDTO<DepositResponseDTO>()
//         {
//             Message = "Can not pay",
//             isSuccess = false,
//             Data = null
//         };
//         return response;
//     }
//
//     private async void updateStatusBooking(Booking booking)
//     {
//         if (booking.Status == BookingStatus.BOOKED)
//         {
//             booking.Status = BookingStatus.DEPOSITED;
//             var updateBookingStatus = await _bookingRepository.UpdateAsync(booking);
//         }
//         else if (booking.Status == BookingStatus.DEPOSITED)
//         {
//             booking.Status = BookingStatus.FINISHED;
//             var updateBookingSattus = await _bookingRepository.UpdateAsync(booking);
//         }
//     }
//
//     private async void createNotification(DepositCreatedDTO depositCreatedDto)
//     {
//         var booking = await _bookingRepository.GetByProperty(x => x.BookingId == depositCreatedDto.BookingId);
//         if (booking != null)
//         {
//             var user = await _userRepository.GetByProperty(x => x.UserId == booking.UserId);
//             if (user != null)
//             {
//                 Notification noti = new Notification()
//                 {
//                     UserId = user.UserId,
//                     Content = $"Deposit for Booking No.{booking.BookingId} successfully",
//                     Title = "Create Deposit",
//                     SentTime = DateTime.Now
//                 };
//             }
//         }
//     }
// }