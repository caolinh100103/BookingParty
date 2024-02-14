namespace Model.DTO;

public class DepositCreatedDTO
{
    public string Content { get; set; }
    public string Title { get; set; }
    public float Percentage { get; set; }
    public TransactionCreatedDTO TransactionCreatedDto { get; set; }
    public int BookingId { get; set; }
}