using System.Net;
using System.Text;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VNPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;

    public VNPayController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }
    [HttpPost]
    public  IActionResult CreatePaymentUrl(VNPayCreatedDTO vnPayCreatedDto) 
    {
        var url =  _vnPayService.CreatePaymentUrl(vnPayCreatedDto, HttpContext);
            
        return Ok(Redirect(url));
    }
    [HttpGet("IPN")]
    public async Task<IActionResult> GetPayment()
    {
        // var vnPayData = Request.QueryString;
        // VNPayHelper helper = new VNPayHelper();
        // foreach (var s in vnPayData)
        // {
        //     
        // }
        VnPayResponseDTO vnPayResponseDto = new VnPayResponseDTO
        {
            BookingId = HttpContext.Request.Query["vnp_TxnRef"].ToString(),
            PayDate = HttpContext.Request.Query["vnp_PayDate"].ToString(),
            Amount = HttpContext.Request.Query["vnp_Amount"].ToString(),
            PaymentMethod = HttpContext.Request.Query["vnp_BankCode"].ToString(),
            TransactionId = HttpContext.Request.Query["vnp_BankCode"].ToString(),
            ResponseCode = HttpContext.Request.Query["vnp_ResponseCode"].ToString()
            // BookingId = 55,
            // PayDate = "20220304",
            // Amount = 100000,
            // PaymentMethod = "VNPAY",
            // ResponseCode = "00",
            // TransactionId = "605154"
        };
        if (vnPayResponseDto.ResponseCode.Equals("00"))
        {
            await _vnPayService.UpdatePayment(vnPayResponseDto);
            return Ok();
        }

        return BadRequest();
    }

    [HttpPost("test")]
    public async Task<IActionResult> Test([FromBody] int bookingId)
    {
        try
        {
            string vnp_RequestId = VnPayHelper.GetRandomNumber(8);
            string vnp_Version = "2.1.0";
            string vnp_Command = "querydr";
            string vnp_TmnCode = "V1X9392U";
            string vnp_TxnRef = "235";
            string vnp_OrderInfo = $"Kiem tra giao dich";
            string vnp_TransDate = "20240303094041l";
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
            string vnp_SecureHash = VnPayHelper.HmacSHA512("FJXGDKPXZDMWLJPJZQSWBMPNINSUUHBO", hash_Data);
            vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);
            // var vnpJson = JsonConvert.SerializeObject(new { vnp_Params, vnp_SecureHash });

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

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
}