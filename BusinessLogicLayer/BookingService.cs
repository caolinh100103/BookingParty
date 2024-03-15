using System.Collections;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;
using System.Reflection;
using BusinessLogicLayer.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;

namespace BusinessLogicLayer;


public class BookingService : IBookingService
{
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<BookingDetail> _bookingDetailRepository;
    private readonly IGenericRepository<Service> _serviceRepository;
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IGenericRepository<ServiceAvailableInDay> _serviceAvailableRepository;
    private readonly IGenericRepository<Deposit> _depositRepository;
    private readonly IGenericRepository<TransactionHistory> _transactionRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ISSEService _sseService;

    public BookingService(IGenericRepository<Booking> bookingRepository,
        IGenericRepository<BookingDetail> bookingDetailRepository, IGenericRepository<Service> serviceRepository
        , IGenericRepository<Room> roomRepository, IGenericRepository<User> userRepository,
        IGenericRepository<Contract> contractRepository,
        IGenericRepository<Notification> notificationRepository, IMapper mapper,
        IGenericRepository<ServiceAvailableInDay> serviceAvailableRepository,
        IGenericRepository<Deposit> depositRepository,
        IGenericRepository<TransactionHistory> transactionRepository,
        IConfiguration configuration,
        ISSEService sseService)
    {
        _bookingRepository = bookingRepository;
        _bookingDetailRepository = bookingDetailRepository;
        _serviceRepository = serviceRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _serviceAvailableRepository = serviceAvailableRepository;
        _depositRepository = depositRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _configuration = configuration;
        _sseService = sseService;
    }

    public async Task<ResultDTO<BookingResponseDTO>> CreateBooking(BookingCreateDTO bookingDto, string token)
    {
        if (bookingDto.StartTime <= DateTime.Now || bookingDto.EndTIme <= DateTime.Now)
        {
            ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
            {
                Message = "Can not have Booking for the past",
                isSuccess = false,
                Data = null
            };
            return bookingResponse;
        }

        if (bookingDto.StartTime >= bookingDto.EndTIme)
        {
            ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
            {
                Message = "Can not have Booking with the time end before start",
                isSuccess = false,
                Data = null
            };
            return bookingResponse;
        }

        var days = (bookingDto.StartTime - DateTime.Now).Days;
        if (days > 270)
        {
            ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
            {
                Message = "You can not book a party over 9 months from now",
                isSuccess = false,
                Data = null
            };
            return bookingResponse;
        }
        if (days < 30)
        {
            ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
            {
                Message = "You have to book before 30 days",
                isSuccess = false,
                Data = null
            };
            return bookingResponse;
        }

        if (bookingDto.TotalPrice >= 100000000)
        {
            if (days < 60)
            {
                ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
                {
                    Message = "Your booking's Total Price is over 100 millions so Booking before 60 days",
                    isSuccess = false,
                    Data = null
                };
                return bookingResponse;
            }
        }

        var hours = (bookingDto.EndTIme - bookingDto.StartTime).TotalHours;
        if (hours > 4)
        {
            ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
            {
                Message = "Your booking have to book just less than 4 hours",
                isSuccess = false,
                Data = null
            };
            return bookingResponse;
        }

        // Decode the token to access claims
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as SecurityToken;

        // Access the user's email claim
        string emailClaim = null;
        if (securityToken is JwtSecurityToken jwtToken)
        {
            // Access the user's email claim
            emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        var user = await _userRepository.GetByProperty(x => x.Email.Equals(emailClaim));
        ICollection<ServiceDTO> services = null;
        if (bookingDto.ServiceIds == null)
        {
            services = null;
        }
        else
        {
            services = new Collection<ServiceDTO>();
        }

        if (services != null)
        {
            foreach (var serviceId in bookingDto.ServiceIds)
            {
                var service = await _serviceRepository.GetByIdAsync(serviceId);
                var serviceDtoMapper = _mapper.Map<ServiceDTO>(service);
                services.Add(serviceDtoMapper);
            }
        }

        Room room = null;
        if (bookingDto.RoomId > 0)
        {
            room = await _roomRepository.GetByIdAsync(bookingDto.RoomId);
        }

        var roomDto = _mapper.Map<RoomDTO>(room);
        // Check the same categories
        if (services != null)
        {
            for (int i = 0; i < services.Count() - 1; i++)
            {
                if (services.ElementAt(i).CategoryId == services.ElementAt(i + 1).CategoryId)
                {
                    ResultDTO<BookingResponseDTO> responseDto = new ResultDTO<BookingResponseDTO>
                    {
                        Message = "Can not have two same categories in a booking",
                        isSuccess = false,
                        Data = null
                    };
                    return responseDto;
                }
            }
        }

        //Check phong co bi trung khong
        // var room = bookingDto.Room;
        if (services != null)
        {
            foreach (var service in services)
            {
                var bookingServiceInDay = await _serviceAvailableRepository.GetByProperty(x =>
                    x.ServiceId == service.ServiceId
                    && x.Date.Date == bookingDto.StartTime.Date && x.NumberOfAvailableInDay == 0);
                if (bookingServiceInDay != null)
                {
                    ResultDTO<BookingResponseDTO> responseDto = new ResultDTO<BookingResponseDTO>
                    {
                        Message = "You have booked a service that exceed booking times today",
                        isSuccess = false,
                        Data = null
                    };
                    return responseDto;
                }
            }
        }

        var bookingDetailList =
            await _bookingDetailRepository.GetListByProperty(x =>
                x.RoomId == roomDto.RoomId); // Find the Room in booking detail

        if (bookingDetailList.Count > 0)
        {
            foreach (var bookingDetail in bookingDetailList)
            {
                if (bookingDto.StartTime <= bookingDetail.StartTime && bookingDetail.StartTime <= bookingDto.EndTIme ||
                    bookingDto.StartTime <= bookingDetail.EndTIme && bookingDetail.EndTIme <= bookingDto.EndTIme
                    || bookingDto.StartTime <= bookingDetail.StartTime && bookingDto.EndTIme >= bookingDetail.EndTIme)
                {
                    ResultDTO<BookingResponseDTO> bookingResponse = new ResultDTO<BookingResponseDTO>
                    {
                        Message = "Can not have Booking room at the same time",
                        isSuccess = false,
                        Data = null
                    };
                    return bookingResponse;
                }
            }
        }

        //Make booking
        var bookingMapper = new Booking
        {
            Status = BookingStatus.BOOKED,
            BookingDate = DateTime.Now,
            UserId = user.UserId,
            TotalPrice = bookingDto.TotalPrice
        };
        var bookingCreated = await _bookingRepository.AddAsync(bookingMapper);
        if (services != null)
        {
            foreach (var service in services)
            {
                var bookingDetail = new BookingDetail
                {
                    ServiceId = service.ServiceId,
                    BookingId = bookingCreated.BookingId,
                    StartTime = bookingDto.StartTime,
                    EndTIme = bookingDto.EndTIme,
                    RoomId = bookingDto.RoomId
                };
                var bookingDetailCreated = await _bookingDetailRepository.AddAsync(bookingDetail);
            }
        }
        else
        {
            var bookingDetail = new BookingDetail
            {
                BookingId = bookingCreated.BookingId,
                StartTime = bookingDto.StartTime,
                EndTIme = bookingDto.EndTIme,
                RoomId = bookingDto.RoomId
            };
            var bookingDetailCreated = await _bookingDetailRepository.AddAsync(bookingDetail);
        }

        var responseBooking = new BookingResponseDTO
        {
            BookingId = bookingCreated.BookingId,
            StartTime = bookingDto.EndTIme,
            EndTIme = bookingDto.EndTIme,
            Room = roomDto,
            Services = services,
            BookingDate = DateTime.Now
        };
        //make contract
        // createContract();
        // Contract contract = new Contract()
        // { 
        //     Status = 1,
        //     LinkFile = outputPathContract,
        //     BookingServiceId = bookingCreated.BookingId,
        // };
        // var conntractCreated = await _contractRepository.AddAsync(contract);

        ResultDTO<BookingResponseDTO> response = new ResultDTO<BookingResponseDTO>
        {
            Message = "Created Successfully",
            Data = responseBooking,
            isSuccess = true
            // Them cac thanh phan vao
        };
        Notification noti = new Notification()
        {
            UserId = user.UserId,
            Content = $"The Booking No.{bookingCreated.BookingId} has been created",
            Title = "Create booking",
            SentTime = DateTime.Now
        };
        var notification = await _notificationRepository.AddAsync(noti);
        // _sseService.SendNotification(notification.Content);
        if (services != null)
        {
            foreach (var service in services)
            {
                var bookingServiceInDay = await _serviceAvailableRepository.GetByProperty(x =>
                    x.ServiceId == service.ServiceId
                    && x.Date.Date == bookingDto.StartTime.Date);
                if (bookingServiceInDay == null)
                {
                    ServiceAvailableInDay serviceAvailableInDay = new ServiceAvailableInDay()
                    {
                        ServiceId = service.ServiceId,
                        Date = bookingDto.StartTime,
                        NumberOfAvailableInDay = 5,
                    };
                    bookingServiceInDay = await _serviceAvailableRepository.AddAsync(serviceAvailableInDay);
                }

                bookingServiceInDay.NumberOfAvailableInDay -= 1;
                _ = await _serviceAvailableRepository.UpdateAsync(bookingServiceInDay);
            }
        }

        return response;
    }

    public async Task<ResultDTO<ICollection<BookingResponseDTO>>> GetAllBooking()
    {
        ResultDTO<ICollection<BookingResponseDTO>> resultDto = null;
        ICollection<BookingResponseDTO> bookingResponseDtos = new List<BookingResponseDTO>();
        ICollection<ServiceDTO> services = null;
        var bookings = await _bookingRepository.GetAllAsync();
        if (!bookings.IsNullOrEmpty())
        {
            foreach (var booking in bookings)
            {
                services = new List<ServiceDTO>();
                BookingResponseDTO bookingResponse = null;
                var bookingDetailist =
                    await _bookingDetailRepository.GetListByProperty(x => x.BookingId == booking.BookingId);
                var room = await _roomRepository.GetByProperty(x => x.RoomId == bookingDetailist.ElementAt(0).RoomId);
                var roomMapper = _mapper.Map<RoomDTO>(room);
                foreach (var bookingDetail in bookingDetailist)
                {
                    var service = await _serviceRepository.GetByProperty(x => x.ServiceId == bookingDetail.ServiceId);
                    var serviceMapper = _mapper.Map<ServiceDTO>(service);
                    services.Add(serviceMapper);
                    bookingResponse = new BookingResponseDTO()
                    {
                        BookingId = booking.BookingId,
                        StartTime = bookingDetail.StartTime,
                        EndTIme = bookingDetail.EndTIme,
                        Room = roomMapper,
                        BookingDate = booking.BookingDate,
                        Services = services,
                        Status = booking.Status
                    };
                }

                bookingResponseDtos.Add(bookingResponse);
            }

            resultDto = new ResultDTO<ICollection<BookingResponseDTO>>()
            {
                Data = bookingResponseDtos,
                isSuccess = true,
                Message = "Return list of booking"
            };
        }
        else
        {
            resultDto = new ResultDTO<ICollection<BookingResponseDTO>>()
            {
                Data = bookingResponseDtos,
                isSuccess = true,
                Message = "We do not have any booking yet"
            };
        }
        return resultDto;
    }

    public async Task<ResultDTO<ICollection<BookingDetailDTO>>> GetAllBookingDetailByBookingId(int bookingId)
    {
        ResultDTO<ICollection<BookingDetailDTO>> result = null;
        ICollection<BookingDetailDTO> bookingDetailDtos = new List<BookingDetailDTO>();
        var bookingDetails = await _bookingDetailRepository.GetListByProperty(x => x.BookingId == bookingId);
        if (!bookingDetails.IsNullOrEmpty())
        {
            foreach (var bookingDetail in bookingDetails)
            {
                var bookingDetailMapper = _mapper.Map<BookingDetailDTO>(bookingDetail);
                bookingDetailDtos.Add(bookingDetailMapper);
            }

            result = new ResultDTO<ICollection<BookingDetailDTO>>()
            {
                Data = bookingDetailDtos,
                isSuccess = true,
                Message = "Return list of booking detail by booking Id"
            };
        }
        else
        {
            result = new ResultDTO<ICollection<BookingDetailDTO>>()
            {
                Data = bookingDetailDtos,
                isSuccess = true,
                Message = "No booking Detail"
            };
        }

        return result;
    }

    public async Task<ResultDTO<int>> FinishBooking(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking != null)
        {
            var anyBookingDetail = await _bookingDetailRepository.GetByProperty(x => x.BookingId == bookingId);
            if (anyBookingDetail.EndTIme < DateTime.Now)
            {
                return new ResultDTO<int>()
                {
                    Data = 0,
                    isSuccess = false,
                    Message = "The party has not been ended yet"
                };
            }
            else
            {
                var deposits = await _depositRepository.GetListByProperty(x => x.BookingId == bookingId);
                if (deposits.Count > 1)
                {
                    return new ResultDTO<int>()
                    {
                        Data = 0,
                        isSuccess = false,
                        Message = "The booking has been finished so can not finish more"
                    };
                }
                else
                {
                    var deposit = deposits.ElementAt(0);
                    if (deposit.Percentage == 50)
                    {
                        booking.Status = BookingStatus.FINISHED;
                        _ = await _bookingRepository.UpdateAsync(booking);
                        Deposit dep = new Deposit()
                        {
                            Title = "FINISH BOOKING",
                            Content = $"Finish booking No.{bookingId}",
                            Percentage = 100,
                            BookingId = bookingId
                        };
                        var depositCreated = await _depositRepository.AddAsync(dep);
                        if (depositCreated != null)
                        {
                            TransactionHistory transactionHistory = new TransactionHistory()
                            {
                                DepositId = depositCreated.DepositId,
                                Amount = booking.TotalPrice,
                                Txn_ref = null,
                                Status = TransactionStatus.FINISHED,
                                BankCode = null,
                                PaymentMethod = "COD",
                                TransactionDate = DateTime.Now.ToString("yyyyMMdd")
                            };
                            var transaction = await _transactionRepository.AddAsync(transactionHistory);
                            if (transaction != null)
                            {
                                return new ResultDTO<int>()
                                {
                                    Data = 1,
                                    isSuccess = true,
                                    Message = "Finish payment the rest for party party"
                                };
                            } 
                        }
                    }
                }
            }
        }

        return new ResultDTO<int>()
        {
            Data = 0,
            isSuccess = false,
            Message = "Internal Server"
        };
    }

    public async Task<ResultDTO<ICollection<BookingResponseDTO>>> GetAllBookingByUserId(int userId)
    {
        ResultDTO<ICollection<BookingResponseDTO>> resultDto = null;
        ICollection<BookingResponseDTO> bookingResponseDtos = new List<BookingResponseDTO>();
        ICollection<ServiceDTO> services = null;
        var bookings = await _bookingRepository.GetListByProperty(x => x.UserId == userId);
        if (!bookings.IsNullOrEmpty())
        {
            foreach (var booking in bookings)
            {
                services = new List<ServiceDTO>();
                BookingResponseDTO bookingResponse = null;
                var bookingDetailist =
                    await _bookingDetailRepository.GetListByProperty(x => x.BookingId == booking.BookingId);
                var room = await _roomRepository.GetByProperty(x => x.RoomId == bookingDetailist.ElementAt(0).RoomId);
                var roomMapper = _mapper.Map<RoomDTO>(room);
                foreach (var bookingDetail in bookingDetailist)
                {
                    var service = await _serviceRepository.GetByProperty(x => x.ServiceId == bookingDetail.ServiceId);
                    var serviceMapper = _mapper.Map<ServiceDTO>(service);
                    services.Add(serviceMapper);
                    bookingResponse = new BookingResponseDTO()
                    {
                        BookingId = booking.BookingId,
                        StartTime = bookingDetail.StartTime,
                        EndTIme = bookingDetail.EndTIme,
                        Room = roomMapper,
                        BookingDate = booking.BookingDate,
                        Services = services,
                        Status = booking.Status
                    };
                }

                bookingResponseDtos.Add(bookingResponse);
            }
            resultDto = new ResultDTO<ICollection<BookingResponseDTO>>()
            {
                Data = bookingResponseDtos,
                isSuccess = true,
                Message = "Return list of booking"
            };
        }
        else
        {
            resultDto = new ResultDTO<ICollection<BookingResponseDTO>>()
            {
                Data = bookingResponseDtos,
                isSuccess = true,
                Message = "We do not have any booking yet"
            };
        }
        return resultDto;
    }

    public async Task<ResultDTO<bool>> CancelByPartyHost(int BookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(BookingId);
        var deposit = await _depositRepository.GetByProperty(x => x.BookingId == BookingId && x.Percentage == 50);
        // implement get amount của transaction
            var transaction = await _transactionRepository.GetByProperty(x => x.DepositId == deposit.DepositId);
            if (transaction != null)
            {
                _ = await RefundWithOrderID(BookingId);
                var BookingDate = booking.BookingDate;
                transaction.Status = TransactionStatus.CANCELED;
                    booking.Status = BookingStatus.CANCELED;
                    _ = await _transactionRepository.UpdateAsync(transaction);
                    _ = await _bookingRepository.UpdateAsync(booking);
                    return new ResultDTO<bool>()
                    {
                        Data = true,
                        isSuccess = true,
                        Message = "Cancel booking with refund"
                    };
            }
        
            return new ResultDTO<bool>()
            {
                Data = false,
                isSuccess = false,
                Message = "Internal Error"
            };
    }

    public async Task<ResultDTO<ICollection<BookingResponseDTO>>> GetBookingByPartyHost(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as SecurityToken;

        // Access the user's email claim
        string emailClaim = null;
        if (securityToken is JwtSecurityToken jwtToken)
        {
            // Access the user's email claim
            emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        var user = await _userRepository.GetByProperty(x => x.Email.Equals(emailClaim));
        ICollection<BookingResponseDTO> response = new List<BookingResponseDTO>();
        HashSet<int> bookingIds = new HashSet<int>();
        if (user != null)
        {
            var roomBookingList = await _roomRepository.GetListByProperty(x => x.UserId == user.UserId);
            foreach (var room in roomBookingList)
            {
                var getBooking = await _bookingDetailRepository.GetByProperty(x => x.RoomId == room.RoomId);
                bookingIds.Add(getBooking.BookingId);
            }

            foreach (var bookingId in bookingIds)
            {
                var booking = await _bookingRepository.GetByProperty(x => x.BookingId == bookingId);
                var bookingMapper = _mapper.Map<BookingResponseDTO>(booking);
                response.Add(bookingMapper);
            }
        }

        return new ResultDTO<ICollection<BookingResponseDTO>>()
        {
            Data = response,
            isSuccess = true,
            Message = "The Booking of Party Host"
        };
    }

    public async Task<ResultDTO<bool>> CancelByCustomer(int BookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(BookingId);
        // var bookingDetails = await _bookingDetailRepository.GetListByProperty(x => x.BookingId == BookingId);
        // BookingDetail FirstBookingDetail = bookingDetails.ElementAt(0);
        // BookingDetailDTO bookingDetailDto = _mapper.Map<BookingDetailDTO>(FirstBookingDetail);
        // implement refund
        var deposit = await _depositRepository.GetByProperty(x => x.BookingId == BookingId && x.Percentage == 50);
        // if (deposits.Count() > 1)
        // {
        //     // implement trả toàn phần in total price của booking
        //     foreach (var deposit in deposits)
        //     {
        //         var transaction = await _transactionRepository.GetByProperty(x => x.DepositId == deposit.DepositId);
        //         transaction.Status = TransactionStatus.CANCELED;
        //         _ = await _transactionRepository.UpdateAsync(transaction);
        //     }
        //
        //     booking.Status = BookingStatus.CANCELED;
        //     _ = await _bookingRepository.UpdateAsync(booking);
        //
        //     return true;
        // }
        // else if (deposits.Count() == 1)
        // {
            // implement get amount của transaction
            var transaction = await _transactionRepository.GetByProperty(x => x.DepositId == deposit.DepositId);
            if (transaction != null)
            {
                var BookingDate = booking.BookingDate;
                var days = (DateTime.Now - BookingDate).Days;
                if (days <= 10)
                {
                    _ = await RefundWithOrderID(BookingId);
                    transaction.Status = TransactionStatus.CANCELED;
                    booking.Status = BookingStatus.CANCELED;
                    _ = await _transactionRepository.UpdateAsync(transaction);
                    _ = await _bookingRepository.UpdateAsync(booking);
                    return new ResultDTO<bool>()
                    {
                        Data = true,
                        isSuccess = true,
                        Message = "Cancel booking with refund"
                    };
                }
                transaction.Status = TransactionStatus.CANCELED;
                booking.Status = BookingStatus.CANCELED;
                _ = await _transactionRepository.UpdateAsync(transaction);
                _ = await _bookingRepository.UpdateAsync(booking);
                return new ResultDTO<bool>()
                {
                    Data = true,
                    isSuccess = true,
                    Message = "Cancel booking with no refund"
                };
            }
        
            return new ResultDTO<bool>()
            {
                Data = false,
                isSuccess = false,
                Message = "Internal Error"
            };
        // }

    }

    // private async Task<string> RefundWithOrderId(int bookingId)
    // {
    //     return null;
    // }

    public async Task<string> QueryVNPAY(string vnp_txnRef, long transactionDate)
    {
        try
        {
            string vnp_RequestId = VnPayHelper.GetRandomNumber(8);
            string vnp_Version = "2.1.0";
            string vnp_Command = "querydr";
            string vnp_TmnCode = _configuration["Vnpay:Tmncode"];
            string vnp_TxnRef = vnp_txnRef;
            string vnp_OrderInfo = $"TIm kiem Thanh Toan";
            string vnp_TransDate = transactionDate.ToString();
            DateTime now = DateTime.Now;
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+7");
            DateTime localTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, localZone);
            string vnp_CreateDate = localTime.ToString("yyyyMMddHHmmss");
            string vnp_IpAddr = VnPayHelper.GenerateRandomIPAddress();

            // var vnp_Params = new
            // {
            //     vnp_RequestId,
            //     vnp_Version,
            //     vnp_Command,
            //     vnp_TmnCode,
            //     vnp_TxnRef,
            //     vnp_OrderInfo,
            //     vnp_TransactionDate = vnp_TransDate,
            //     vnp_CreateDate,
            //     vnp_IpAddr
            // };
            
            JObject vnp_Params = new JObject();
            vnp_Params.Add("vnp_RequestId", vnp_RequestId);
            vnp_Params.Add("vnp_Version", vnp_Version);
            vnp_Params.Add("vnp_Command", vnp_Command);
            vnp_Params.Add("vnp_TmnCode", vnp_TmnCode);
            vnp_Params.Add("vnp_TxnRef", vnp_TxnRef);
            vnp_Params.Add("vnp_OrderInfo", vnp_OrderInfo);
            vnp_Params.Add("vnp_TransactionDate", vnp_TransDate);
            vnp_Params.Add("vnp_CreateDate", vnp_CreateDate);
            vnp_Params.Add("vnp_IpAddr", vnp_IpAddr);

            string hash_Data =
                $"{vnp_RequestId}|{vnp_Version}|{vnp_Command}|{vnp_TmnCode}|{vnp_TxnRef}|{vnp_TransDate}|{vnp_CreateDate}|{vnp_IpAddr}|{vnp_OrderInfo}";
            string vnp_SecureHash = VnPayHelper.HmacSHA512(_configuration["Vnpay:HashSecret"], hash_Data);

            // var vnpJson = JsonConvert.SerializeObject(new { vnp_Params, vnp_SecureHash });
            vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);
            var url = new Uri(VnPayHelper.vnp_apiUrl);
            var request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(vnp_Params.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)await request.GetResponseAsync();

            var responseStream = httpResponse.GetResponseStream();
            var streamReader = new StreamReader(responseStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var responseString = streamReader.ReadToEnd();

            return responseString;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "Error Server";
        }
    }

    public async Task<string> QueryVNPayWithVnpTxtRef(string vnp_txnRef) 
    {
        try
        {
            // Fetch transaction details from the repository
            var getTransaction = await _transactionRepository.GetByProperty(x => x.Txn_ref.ToString() == vnp_txnRef);
            string getVnp_txnRef = getTransaction.Txn_ref.ToString();
            string transactionDate = getTransaction.TransactionDate; // chu y cho nay
            long transactionDateLong = long.Parse(transactionDate);
            // Call VNPay service
            string callingResult = await QueryVNPAY(getVnp_txnRef, transactionDateLong);
    
            // Return the result
            return callingResult;
        }
        catch (Exception e)
        {
            // Log the exception or handle it accordingly
            Console.WriteLine(e.Message);
            return "ERROR SERVER";
        }
    }
    public async Task<string> RefundWithOrderID(int bookingId)
    {
        try
        {
            var getBooking = await _bookingRepository.GetByIdAsync(bookingId);
            var user = await _userRepository.GetByProperty(x => x.UserId == getBooking.UserId);
            var getTransaction = await _transactionRepository.GetByProperty(x => x.Txn_ref == getBooking.BookingId);
            var vnp_txtRef = getTransaction.Txn_ref.ToString();
            string transactionDate = getTransaction.TransactionDate;
            int amount = (int)getTransaction.Amount;
            
            var responseQuery = await QueryVNPayWithVnpTxtRef(vnp_txtRef);
                    
            string getResponseCode = VnPayHelper.extractResponseCode(responseQuery);
            
                
            // string getTransactionStatus = VnPayHelper.EX(getQuery);
            
            var callingResult = await 
                RefundAsync(vnp_txtRef, transactionDate, amount, user.FullName);
            // if (callingResult == "SUCCESS")
            // {
            //     Console.WriteLine("yes refund sent and success, wait for the bank to response");
            //     orderService.SetOrderStatus(getOrder, OrderStatus.CANCEL);
            //     SetTransactionStatus(getTransaction, TransactionStatus.REFUNDED);
            //     vnPayService.DeleteOrderUrlMapItem(getOrder_Id);
            //     return "refund Success";
            // }
            // else
            // {
            //     Console.WriteLine("fail to refund, try again");
            //     return "FAIL TO REFUND, TRY AGAIN";
            // }
            return callingResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "ERROR SERVER";
        }
    }
    
    public async Task<string> RefundAsync(string vnp_txnRef, string transactionDate, int getAmount, string UserName)
{
    try
    {
        string vnp_RequestId = VnPayHelper.GetRandomNumber(8);
        string vnp_Version = "2.1.0";
        string vnp_Command = "refund";
        string vnp_TmnCode = _configuration["Vnpay:Tmncode"];
        string vnp_TxnRef = vnp_txnRef;
        string vnp_TransactionType = "02";
        int amount = getAmount;
        string vnp_Amount = amount.ToString();
        string vnp_OrderInfo = $"Hoan tien GD";
        string vnp_TransactionNo = "";
        string vnp_TransactionDate = transactionDate;
        string vnp_CreateBy = UserName; // implement ten user
        DateTime now = DateTime.Now;
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+7");
        DateTime localTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, localZone);
        string vnp_CreateDate = localTime.ToString("yyyyMMddHHmmss");
        string vnp_IpAddr = VnPayHelper.GenerateRandomIPAddress();
        

        //
        // var vnp_Params = new
        // {
        //     vnp_RequestId,
        //     vnp_Version,
        //     vnp_Command,
        //     vnp_TmnCode,
        //     vnp_TransactionType,
        //     vnp_TxnRef,
        //     vnp_Amount,
        //     vnp_OrderInfo,
        //     vnp_TransactionDate,
        //     vnp_CreateBy,
        //     vnp_CreateDate,
        //     vnp_IpAddr
        // };
        JObject vnp_Params = new JObject();

        //63562614
        //20230616094041

        vnp_Params.Add("vnp_RequestId", vnp_RequestId);
        vnp_Params.Add("vnp_Version", vnp_Version);
        vnp_Params.Add("vnp_Command", vnp_Command);
        vnp_Params.Add("vnp_TmnCode", vnp_TmnCode);
        vnp_Params.Add("vnp_TransactionType", vnp_TransactionType);
        vnp_Params.Add("vnp_TxnRef", vnp_TxnRef);
        vnp_Params.Add("vnp_Amount", vnp_Amount);
        vnp_Params.Add("vnp_OrderInfo", vnp_OrderInfo);

        vnp_Params.Add("vnp_TransactionDate", vnp_TransactionDate);
        vnp_Params.Add("vnp_CreateBy", vnp_CreateBy);
        vnp_Params.Add("vnp_CreateDate", vnp_CreateDate);
        vnp_Params.Add("vnp_IpAddr", vnp_IpAddr);

        string hash_Data = $"{vnp_RequestId}|{vnp_Version}|{vnp_Command}|{vnp_TmnCode}|{vnp_TransactionType}|{vnp_TxnRef}|{vnp_Amount}|{vnp_TransactionNo}|{vnp_TransactionDate}|{vnp_CreateBy}|{vnp_CreateDate}|{vnp_IpAddr}|{vnp_OrderInfo}";
        string vnp_SecureHash = VnPayHelper.HmacSHA512(_configuration["Vnpay:HashSecret"], hash_Data);
        vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);
        // var vnpJson = JsonConvert.SerializeObject(new { vnp_Params, vnp_SecureHash });

        using (var httpClient = new HttpClient())
        {
            var content = new StringContent(vnp_Params.ToString(), Encoding.UTF8, "application/json");
            var httpResponse = await httpClient.PostAsync(VnPayHelper.vnp_apiUrl, content);

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsStringAsync();
            }
            else
            {
                return "VNPAY SERVICE ERROR, TRY AGAIN LATER";
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return "ERROR SERVER";
    }
}
}

