using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BusinessLogicLayer.Enum;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly IGenericRepository<Booking> _bookRepository;
    private readonly IGenericRepository<Deposit> _depositRepository;
    private readonly IGenericRepository<TransactionHistory> _transactionHistory;
    private readonly IMapper _mapper;
    public VnPayService(IConfiguration configuration, IGenericRepository<Booking> bookRepository,
        IMapper mapper, IGenericRepository<Deposit> depositRepository, IGenericRepository<TransactionHistory> transactionHistory)
    {
        _configuration = configuration;
        _bookRepository = bookRepository;
        _mapper = mapper;
        _depositRepository = depositRepository;
        _transactionHistory = transactionHistory;
    }
    public string CreatePaymentUrl(VNPayCreatedDTO vnPayCreatedDto, HttpContext context)
    {
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var booking =  _bookRepository.GetLast(x => x.BookingId);
        var bookingId = booking.BookingId;
        var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
        AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
        AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
        AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        AddRequestData("vnp_Amount", (vnPayCreatedDto.TotalPrice * 100 * 50 / 100).ToString());
        AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
        AddRequestData("vnp_IpAddr", GenerateRandomIPAddress());
        AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
        AddRequestData("vnp_OrderInfo", $"Thanh toán Booking Bitrhday Party");
        AddRequestData("vnp_OrderType", "Booking Birthday Party");
        AddRequestData("vnp_ReturnUrl", urlCallBack);
        AddRequestData("vnp_TxnRef", bookingId.ToString());

        var paymentUrl = CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

        return paymentUrl;
    }

    public async Task UpdatePayment(VnPayResponseDTO vnPayResponseDto)
    {
        
        var bookingId = int.Parse(vnPayResponseDto.BookingId);
        var booking = await _bookRepository.GetByIdAsync(bookingId);
        if (booking.Status == BookingStatus.CANCELED)
        {
            return;
        }
        booking.Status = BookingStatus.DEPOSITED;
        _ = await _bookRepository.UpdateAsync(booking);
        var Deposit = await _depositRepository.GetByProperty(x => x.BookingId == bookingId);
        var amount = decimal.Parse(vnPayResponseDto.Amount);
        if (Deposit == null)
        {
            DepositDTO depositDto = new DepositDTO()
            {
                Content = $"Making a deposit for Booking No.{bookingId}",
                Percentage = 50,
                Title = "Deposit for a booking",
                BookingId = bookingId
            };
            var deposit = _mapper.Map<Deposit>(depositDto);
            var depositCreated = await _depositRepository.AddAsync(deposit);
            if (depositCreated != null)
            {
                TransactionCreatedDTO transactionCreatedDto = new TransactionCreatedDTO()
                {
                    Txn_ref = bookingId,
                    BankCode = vnPayResponseDto.PaymentMethod,
                    Status = TransactionStatus.SUCCESS,
                    Amount = amount,
                    PaymentMethod = "VNPAY",
                    TransactionDate = vnPayResponseDto.PayDate,
                    DepositId = depositCreated.DepositId
                };
                var transactionHistory = _mapper.Map<TransactionHistory>(transactionCreatedDto);
                var transactionHistoryCreated = await _transactionHistory.AddAsync(transactionHistory);
            }

            return;
        }
    }

    private string GetIpAddress(HttpContext context)
    {
        var ipAddress = string.Empty;
        try
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;

            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                        .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                }

                if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                return ipAddress;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "127.0.0.1";
    }
    public string GenerateRandomIPAddress()
    {
        Random rand = new Random();
        byte[] ipBytes = new byte[4];
        rand.NextBytes(ipBytes);
        IPAddress ipAddress = new IPAddress(ipBytes);
        return ipAddress.ToString();
    }
    private void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }
    
    private string CreateRequestUrl(string baseUrl, string vnpHashSecret)
    {
        var data = new StringBuilder();

        foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        var querystring = data.ToString();

        baseUrl += "?" + querystring;
        var signData = querystring;
        if (signData.Length > 0)
        {
            signData = signData.Remove(data.Length - 1, 1);
        }

        var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnpSecureHash;

        return baseUrl;
    }
    
    private string HmacSha512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
    
    private class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
    public  void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _responseData.Add(key, value);
        }
    }
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());
    
    

}