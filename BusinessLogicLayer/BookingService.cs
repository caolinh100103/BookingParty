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

    private string samplePathContract = "D:/FPT/CN7/SWD392/testPdf/sample.pdf";
    private string outputPathContract = "D:/FPT/CN7/SWD392/testPdf/output/Output.pdf";
        public BookingService(IGenericRepository<Booking> bookingRepository, IGenericRepository<BookingDetail> bookingDetailRepository,IGenericRepository<Service> serviceRepository
        , IGenericRepository<Room> roomRepository, IGenericRepository<User> userRepository, IGenericRepository<Contract> contractRepository,
        IGenericRepository<Notification> notificationRepository,IMapper mapper,
        IGenericRepository<ServiceAvailableInDay> serviceAvailableRepository,
        IGenericRepository<Deposit> depositRepository,
        IGenericRepository<TransactionHistory> transactionRepository,
        IConfiguration configuration)
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

        var hours = (bookingDto.EndTIme - bookingDto.StartTime).Hours;
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
        var securityToken  = tokenHandler.ReadToken(token) as SecurityToken;

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
        var bookingDetailList = await _bookingDetailRepository.GetListByProperty(x => x.RoomId == roomDto.RoomId); // Find the Room in booking detail
        
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
            Status = BookingStatus.PENDING,
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
            Services = services
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
        if (services != null) {
            foreach(var service in services)
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
        var bookings = await _bookingRepository.GetAllAsync();
        if (!bookings.IsNullOrEmpty())
        {
            foreach (var booking in bookings)
            {
                var bookingResponse = _mapper.Map<BookingResponseDTO>(booking);
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

    private Microsoft.Office.Interop.Word.Application app;
    private Microsoft.Office.Interop.Word.Document doc;
    private object objectMiss = Missing.Value;
    private object TmpFile = System.IO.Path.GetTempPath() + "sample.pdf";
    private object FileLoaction = @"D:\FPT\CN7\SWD392\testPdf\sample2.docx";

    private void createContract()
    {
        app = new Microsoft.Office.Interop.Word.Application();
        doc = app.Documents.Open(ref FileLoaction, ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss,
            ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss,
            ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss, ref objectMiss);
        ReplaceText("[Phone]", "123-456");
        doc.ExportAsFixedFormat(outputPathContract, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
        
        doc.Close(WdSaveOptions.wdDoNotSaveChanges, WdOriginalFormat.wdOriginalDocumentFormat, false);
        app.Quit(WdSaveOptions.wdDoNotSaveChanges);
    }

    private void ReplaceText(object FindText, object ReplaceText)
    {
        this.app.Selection.Find.Execute(ref FindText, true, true, false, false
            , false, true, false, 1, ref ReplaceText, 2, false, false, false, false);
    }
    
    public async Task<bool> CancelByPH(int BookingId)
    {
        
        var booking = await _bookingRepository.GetByIdAsync(BookingId);
        // var bookingDetails = await _bookingDetailRepository.GetListByProperty(x => x.BookingId == BookingId);
        // BookingDetail FirstBookingDetail = bookingDetails.ElementAt(0);
        // BookingDetailDTO bookingDetailDto = _mapper.Map<BookingDetailDTO>(FirstBookingDetail);
        // implement refund
        var deposits = await _depositRepository.GetListByProperty(x => x.BookingId == BookingId);
        if (deposits.Count() > 1)
        {
            // implement trả toàn phần in total price của booking
            foreach (var deposit in deposits)
            {
                var transaction = await _transactionRepository.GetByProperty(x => x.DepositId == deposit.DepositId);
                transaction.Status = TransactionStatus.CANCELED;
                _ = await _transactionRepository.UpdateAsync(transaction);
            }
            booking.Status = BookingStatus.CANCELED;
            _ = await _bookingRepository.UpdateAsync(booking);
            
            return true;
        }
        else if (deposits.Count() == 1)
        {
            var deposit = deposits.ElementAt(0);
            // implement get amount của transaction
            var transaction = await _transactionRepository.GetByProperty(x => x.DepositId == deposit.DepositId);
            if (transaction != null)
            {
                transaction.Status = TransactionStatus.CANCELED;
                booking.Status = BookingStatus.CANCELED;
                _ = await _transactionRepository.UpdateAsync(transaction);
                _ = await _bookingRepository.UpdateAsync(booking);
            }

            return true;
        }

        return false;
    }

    // private async Task<string> RefundWithOrderId(int bookingId)
    // {
    //     return null;
    // }
    
    //  public async Task<HttpResponseMessage> QueryVNPAY(string vnp_txnRef, long transactionDate, HttpRequest req, HttpResponse resp)
    // {
    //     try
    //     {
    //         string vnp_RequestId = VnPayHelper.GetRandomNumber(8);
    //         string vnp_Version = "2.1.0";
    //         string vnp_Command = "pay";
    //         string vnp_TmnCode = _configuration["Vnpay:Tmncode"];
    //         string vnp_TxnRef = vnp_txnRef;
    //         string vnp_OrderInfo = $"Thanh toán Booking Bitrhday Party";
    //         string vnp_TransDate = transactionDate.ToString();
    //         DateTime now = DateTime.Now;
    //         TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+7");
    //         DateTime localTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, localZone);
    //         string vnp_CreateDate = localTime.ToString("yyyyMMddHHmmss");
    //         string vnp_IpAddr = VnPayHelper.GetIpAddress(req);
    //
    //         var vnp_Params = new
    //         {
    //             vnp_RequestId,
    //             vnp_Version,
    //             vnp_Command,
    //             vnp_TmnCode,
    //             vnp_TxnRef,
    //             vnp_OrderInfo,
    //             vnp_TransactionDate = vnp_TransDate,
    //             vnp_CreateDate,
    //             vnp_IpAddr
    //         };
    //
    //         string hash_Data = $"{vnp_RequestId}|{vnp_Version}|{vnp_Command}|{vnp_TmnCode}|{vnp_TxnRef}|{vnp_TransDate}|{vnp_CreateDate}|{vnp_IpAddr}|{vnp_OrderInfo}";
    //         string vnp_SecureHash = VnPayHelper.HmacSHA512(_configuration["Vnpay:HashSecret"], hash_Data);
    //
    //         var vnpJson = JsonConvert.SerializeObject(new { vnp_Params, vnp_SecureHash });
    //
    //         var url = new Uri(VnPayHelper.vnp_apiUrl);
    //         var request = WebRequest.CreateHttp(url);
    //         request.Method = "POST";
    //         request.ContentType = "application/json";
    //
    //         using (var streamWriter = new StreamWriter(request.GetRequestStream()))
    //         {
    //             streamWriter.Write(vnpJson);
    //             streamWriter.Flush();
    //             streamWriter.Close();
    //         }
    //
    //         var httpResponse = (HttpWebResponse)await request.GetResponseAsync();
    //
    //         var responseStream = httpResponse.GetResponseStream();
    //         var streamReader = new StreamReader(responseStream ?? throw new InvalidOperationException(), Encoding.UTF8);
    //         var responseString = streamReader.ReadToEnd();
    //         
    //         return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseString) };
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message);
    //         return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { Content = new StringContent("ERROR SERVER") };
    //     }
    // }
    //  
    //  public async Task<string> QueryVNpayWithOrderID(string orderId)
    //  {
    //      try
    //      {
    //          if (!long.TryParse(orderId, out long parseOrderId))
    //          {
    //              return "Invalid order ID format";
    //          }
    //
    //          var order = await _orderRepository.GetByIdAsync(parseOrderId);
    //          if (order == null)
    //          {
    //              return "Order not found";
    //          }
    //
    //          var transaction = await _transactionRepository.GetByIdAsync(order.TransactionId);
    //          if (transaction == null)
    //          {
    //              return "Transaction not found";
    //          }
    //
    //          var callingResult = await _vnPayService.QueryVNPAY(transaction.VnpTxnRef, transaction.VnpTransactionDate);
    //          if (callingResult.IsSuccessStatusCode)
    //          {
    //              return await callingResult.Content.ReadAsStringAsync();
    //          }
    //          else
    //          {
    //              return "VNPAY SERVICE ERROR";
    //          }
    //      }
    //      catch (FormatException e)
    //      {
    //          Console.WriteLine(e.Message);
    //          return "ERROR PARSING ORDER ID";
    //      }
    //      catch (NullReferenceException e)
    //      {
    //          Console.WriteLine(e.Message);
    //          return "ERROR NULL REFERENCE";
    //      }
    //      catch (Exception e)
    //      {
    //          Console.WriteLine(e.Message);
    //          return "INTERNAL SERVER ERROR";
    //      }
    //  }
    
}