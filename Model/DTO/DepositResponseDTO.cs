using Model.Entity;

namespace Model.DTO;

public class DepositResponseDTO
{
    public int DepositId { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public float Percentage { get; set; }
    // public virtual Booking Booking { get; set; }
}