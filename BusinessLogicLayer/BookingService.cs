using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class BookingService : IBookingService
{
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<BookingDetail> _bookingDetailRepository;
    private readonly IGenericRepository<Service> _serviceRepository;
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Contract> _contractRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IMapper _mapper;

    private readonly string samplePathContract = "D:/FPT/CN7/SWD392/testPdf/sample.pdf";
    private readonly string outputPathContract = "D:/FPT/CN7/SWD392/testPdf/output/Output.pdf";
        public BookingService(IGenericRepository<Booking> bookingRepository, IGenericRepository<BookingDetail> bookingDetailRepository,IGenericRepository<Service> serviceRepository
        , IGenericRepository<Room> roomRepository, IGenericRepository<User> userRepository, IGenericRepository<Contract> contractRepository,
        IGenericRepository<Notification> notificationRepository,IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _bookingDetailRepository = bookingDetailRepository;
        _serviceRepository = serviceRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _contractRepository = contractRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }
    
    public async Task<ResultDTO<BookingResponseDTO>> CreateBooking(BookingCreateDTO bookingDto, string token)
    {
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
        if (bookingDto.ServiceIds.ElementAt(0).Equals(0))
        {
            services = null;
        }
        else
        {
            services = new Collection<ServiceDTO>();
        }

        foreach (var serviceId in bookingDto.ServiceIds)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            var serviceDtoMapper = _mapper.Map<ServiceDTO>(service);
            services.Add(serviceDtoMapper);
        }

        Room room = null;
        if (bookingDto.RoomId > 0)
        {
            room = await _roomRepository.GetByIdAsync(bookingDto.RoomId);
        }
        var roomDto = _mapper.Map<RoomDTO>(room);
        // Check the same categories
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

        //Check phong co bi trung khong
        // var room = bookingDto.Room;
        var bookingDetailList = await _bookingDetailRepository.GetListByProperty(x => x.RoomId == roomDto.RoomId); // Find the Room in booking detail
        if (bookingDetailList.Count > 0)
        {
            foreach (var bookingDetail in bookingDetailList)
            {
                if (bookingDto.StartTime <= bookingDetail.StartTime && bookingDetail.StartTime <= bookingDto.EndTIme ||
                    bookingDto.StartTime <= bookingDetail.EndTIme && bookingDetail.EndTIme <= bookingDto.EndTIme)
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
        //
        var exactAddress = checkAddress(bookingDto);
        
        //Make booking
        var bookingMapper = new Booking
        {
            Status = BookingStatus.PENDING,
            BookingDate = DateTime.Now,
            UserId = user.UserId,
            TotalPrice = bookingDto.TotalPrice
        };
        var bookingCreated = await _bookingRepository.AddAsync(bookingMapper);
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

        var responseBooking = new BookingResponseDTO
        {
            BookingId = bookingCreated.BookingId,
            StartTime = bookingDto.EndTIme,
            EndTIme = bookingDto.EndTIme,
            ExactAddress = exactAddress,
            Room = roomDto,
            Services = services
        };
        //make contract
        Dictionary<string, string> fieldData = new Dictionary<string, string>
        {
            { "Phone", "123-456" }
        };
/*        CreateContract(samplePathContract, outputPathContract, "123-456");
        Contract contract = new Contract()
        {
            Status = 1,
            LinkFile = outputPathContract,
            BookingServiceId = bookingCreated.BookingId,
        };
        var conntractCreated = await _contractRepository.AddAsync(contract);*/
        
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
        return response;
    }

    private string checkAddress(BookingCreateDTO bookingDto)
    {
        if (bookingDto.Ward == null || bookingDto.District == null)
        {
            return bookingDto.ExactAddress;
        }
        else
        {
            string exactAddress = "Ward " + bookingDto.Ward + ", " + "District " + bookingDto.District + ", " +
                                  bookingDto.ExactAddress;
            return exactAddress;
        }
    }

    private void CreateContract(string samplePdfPath, string outputPath, string dynamicValue)
    {
        using (PdfReader reader = new PdfReader(samplePdfPath))
        {
            using (PdfWriter writer = new PdfWriter(outputPath))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    // Iterate through each page
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        // Get the current page
                        PdfPage page = pdfDoc.GetPage(i);

                        // Create a canvas for the current page
                        PdfCanvas canvas = new PdfCanvas(page);

                        // Parse the content of the current page
                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                        PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                        parser.ProcessPageContent(page);

                        // Get the extracted text
                        string pageContent = strategy.GetResultantText();

                        // Check if the page contains the text "Phone:"
                        if (pageContent.Contains("Phone:"))
                        {
                            // Modify the text to append the dynamic value
                            string modifiedContent = pageContent.Replace("Phone:", "Phone: " + dynamicValue);

                            // Clear existing content and add modified text
                            canvas
                                .BeginText()
                                .SetFontAndSize(null, 12)
                                .MoveText(100, 500) // Adjust coordinates as needed
                                .ShowText(modifiedContent)
                                .EndText();
                        }
                    }
                }
            }
        }
    }
}