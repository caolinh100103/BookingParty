using Microsoft.AspNetCore.Http;
using Model.DTO;

namespace BusinessLogicLayer.Interfaces;
public interface IVnPayService 
{
    string CreatePaymentUrl(VNPayCreatedDTO vnPayCreatedDto, HttpContext context);
}