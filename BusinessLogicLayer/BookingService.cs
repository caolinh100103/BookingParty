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
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;
using iText.Forms;
using iText.Forms.Fields;
using System.IO;
using System.Reflection;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Office.Interop.Word;
using Org.BouncyCastle.Pkix;



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

    private string samplePathContract = "D:/FPT/CN7/SWD392/testPdf/sample.pdf";
    private string outputPathContract = "D:/FPT/CN7/SWD392/testPdf/output/Output.pdf";
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
        return response;
    }

    // private void CreateContract(string samplePdfPath, string outputPath)
    // {
    //     try
    //     {
    //         using (PdfReader reader = new PdfReader(samplePdfPath))
    //         {
    //             using (PdfWriter writer = new PdfWriter(outputPath))
    //             {
    //                 using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
    //                 {
    //                     // PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true);
    //                     // if (form != null)
    //                     // {
    //                     //     IDictionary<string, PdfFormField> fields = form.GetFormFields();
    //                     //     PdfFormField toset;
    //                     //
    //                     //     fields.TryGetValue("Phone:", out toset);
    //                     //     toset.SetValue("123-456");
    //                     //
    //                     //     // pdf.Close();
    //                     //     // outputStream.Position = 0;
    //                     //     // using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    //                     //     // {
    //                     //     //     outputStream.WriteTo(fileStream);
    //                     //     // }
    //                     // }
    //                     // Get the number of pages in the PDF document
    //                     int totalPages = pdfDoc.GetNumberOfPages();
    //
    //                     // Loop through each page
    //                     for (int pageNum = 1; pageNum <= totalPages; pageNum++)
    //                     {
    //                         // Extract text from the current page
    //                         string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(pageNum));
    //
    //                         // Check if the page contains the text "Phone"
    //                         if (pageText.Contains("Phone"))
    //                         {
    //                             // Get the position of "Phone" on the page
    //                             int index = pageText.IndexOf("Phone");
    //
    //                             // Get the coordinates of "Phone" on the page
    //                             IList<TextRenderInfo> renderInfos = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(pageNum), new LocationTextExtractionStrategy()).Re();
    //                             TextRenderInfo phoneTextRenderInfo = renderInfos[index];
    //                             Rectangle phoneBoundingBox = phoneTextRenderInfo.GetBoundingBox();
    //
    //                             float x = rect.GetX();
    //                             float y = rect.GetY();
    //
    //                             // Create a document
    //                             Document document = new Document(pdfDoc);
    //
    //                             // Add the modified content to the document at the same position as "Phone"
    //                             document.SetFixedPosition(x, y, 100); // Adjust the width as needed
    //                             // document.Add(new Paragraph(newText));
    //
    //                             // Close the document
    //                             document.Close();
    //                         }
    //                     }
    //                     // // Iterate through each page
    //                     // for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
    //                     // {
    //                     //     // Get the current page
    //                     //     PdfPage page = pdfDoc.GetPage(i);
    //                     //
    //                     //     // Create a canvas for the current page
    //                     //     PdfCanvas canvas = new PdfCanvas(page);
    //                     //
    //                     //     // Parse the content of the current page
    //                     //     LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
    //                     //     PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
    //                     //     parser.ProcessPageContent(page);
    //                     //
    //                     //     // Get the extracted text
    //                     //     string pageContent = strategy.GetResultantText();
    //                     //
    //                     //     // Check if the page contains the text "Phone:"
    //                     //     if (pageContent.Contains("Phone:"))
    //                     //     {
    //                     //         // Modify the text to append the dynamic value
    //                     //         string modifiedContent = pageContent.Replace("Phone:", "Phone: " + dynamicValue);
    //                     //
    //                     //         // Clear existing content and add modified text
    //                     //         canvas
    //                     //             .BeginText()
    //                     //             .SetFontAndSize(null, 12)
    //                     //             .MoveText(100, 500) // Adjust coordinates as needed
    //                     //             .ShowText(modifiedContent)
    //                     //             .EndText();
    //                     //     }
    //                     // }
    //                 }
    //             }
    //         }
    //
    //         System.Console.WriteLine("New PDF created successfully.");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         
    //     }
    //     
    // }
    //
    // private PdfDocument Contract(string path,string outputPath)
    // {
    //     
    //     if (System.IO.File.Exists(path))
    //     {
    //         var sourceFileStream = System.IO.File.OpenRead(path);
    //         var outputStream = new MemoryStream();
    //
    //         var pdf = new PdfDocument(new PdfReader(sourceFileStream), new PdfWriter(outputStream));
    //         PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true);
    //         if (form != null)
    //         {
    //             IDictionary<string, PdfFormField> fields = form.GetFormFields();
    //             PdfFormField toset;
    //
    //             fields.TryGetValue("Phone:", out toset);
    //             toset.SetValue("123-456");
    //             
    //             pdf.Close();
    //             outputStream.Position = 0;
    //             using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    //             {
    //                 outputStream.WriteTo(fileStream);
    //             }
    //             // return pdf;
    //             // return outputStream.ToArray();
    //             // return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "OutputPdf.pdf");
    //             // outputStream.Position = 0;
    //             //
    //             // // Save the modified PDF content to a new file
    //             // using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    //             // {
    //             //     outputStream.WriteTo(fileStream);
    //             // }
    //             //
    //             // return outputPath;
    //         } 
    //         // else
    //         // {
    //         //     pdf.Close();
    //         //     sourceFileStream.Close();
    //         //     return null; // Form not found, return null or handle accordingly
    //         // }
    //     }
    //
    //     return null;
    // }
    //
    // private byte[] Contract2(string samplePath)
    // {
    //     if (System.IO.File.Exists(samplePath))
    //     {
    //         using (var sourceFileStream = System.IO.File.OpenRead(samplePath))
    //         {
    //             var outputStream = new MemoryStream();
    //
    //             var pdf = new PdfDocument(new PdfReader(sourceFileStream), new PdfWriter(outputStream));
    //             PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true); // Set the second parameter to true to create the form if it doesn't exist
    //
    //             if (form != null)
    //             {
    //                 IDictionary<string, PdfFormField> fields = form.GetFormFields();
    //                 if (fields.TryGetValue("Phone", out var toSet))
    //                 {
    //                     toSet.SetValue("123-456");
    //                 }
    //                 
    //                 outputStream.Position = 0;
    //                 pdf.Close();
    //                 return outputStream.ToArray();
    //             }
    //             else
    //             {
    //                 pdf.Close();
    //                 sourceFileStream.Close();
    //                 return null; // Form not found, return null or handle accordingly
    //             }
    //         }
    //     }
    //     else
    //     {
    //         return null; // File not found, return null or handle accordingly
    //     }
    // }

    // private void FindAndReplce(string samplePath, string ouputPath, string searchText, string replaceText)
    // {
    //     try
    //     {
    //         // Initialize PDF document
    //         using (PdfReader reader = new PdfReader(samplePath))
    //         {
    //             using (PdfWriter writer = new PdfWriter(ouputPath))
    //             {
    //                 using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
    //                 {
    //                     for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
    //                     {
    //                         // Extract text from the page
    //                         string text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(pageNum));
    //
    //                         // Perform find and replace operation
    //                         text = text.Replace(searchText, replaceText);
    //
    //                         // Create a Canvas for the page
    //                         PdfCanvas canvas = new PdfCanvas(pdfDoc.GetPage(pageNum));
    //
    //                         // Add the modified text to the canvas
    //                         canvas.BeginText()
    //                             .SetFontAndSize(null, 12) // Set font and size as needed
    //                             .MoveText(100, 100) // Set position as needed
    //                             .ShowText(text)
    //                             .EndText();
    //                     }
    //                 }
    //             }
    //         }
    //
    //         Console.WriteLine("Text replaced successfully.");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("An error occurred: " + ex.Message);
    //     }
    // }

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
}