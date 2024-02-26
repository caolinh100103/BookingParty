namespace Model.DTO;

public class TransactionCreatedDTO
{
    public string? BankCode { get; set; }
    public string PaymentMethod { get; set; }
    public string? Txn_ref { get; set; }
    public string TransactionDate { get; set; }
}