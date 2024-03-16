namespace Model.DTO;

public class VnPayResponseDTO
{
    public string PaymentMethod { get; set; }
    public string  BookingId { get; set; }
    public string TransactionId { get; set; }
    public string PayDate { get; set; }
    public string Amount { get; set; }
    public string ResponseCode { get; set; }
}