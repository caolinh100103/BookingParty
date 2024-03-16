namespace BusinessLogicLayer.Helper;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
public class VnPayHelper
{
    public static string vnp_apiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";

    public static string GetRandomNumber(int len)
    {
        Random rnd = new Random();
        string chars = "0123456789";
        StringBuilder sb = new StringBuilder(len);
        for (int i = 0; i < len; i++)
        {
            sb.Append(chars[rnd.Next(chars.Length)]);
        }
        return sb.ToString();
    }
    public static string GenerateRandomIPAddress()
    {
       Random rand = new Random();
       byte[] ipBytes = new byte[4];
       rand.NextBytes(ipBytes);
       IPAddress ipAddress = new IPAddress(ipBytes);
       return ipAddress.ToString();
    }
    public static string GetIpAddress(HttpRequest request)
    {
        string ipAddress;
        try
        {
            ipAddress = request.Headers["X-FORWARDED-FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }
        catch (Exception e)
        {
            ipAddress = "Invalid IP: " + e.Message;
        }
        return ipAddress;
    }
    public static string HmacSHA512(string key, string data)
    {
        try
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException();
            }
    
            using (var hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] result = hmac512.ComputeHash(dataBytes);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
    public static string extractResponseCode(string response){
        String[] splitTransaction = response.Split(",");
        String getResponseCode = splitTransaction[2].Split(":")[1].Substring(1,3);
        return getResponseCode;
    }
    public static string HmacSHA512_2(string key, string data)
    {
        try
        {
            if (key == null || data == null)
            {
                throw new ArgumentNullException();
            }

            using (HMACSHA512 hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = hmac512.ComputeHash(dataBytes);

                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}