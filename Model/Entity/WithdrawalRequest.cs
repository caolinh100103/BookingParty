using System.ComponentModel.DataAnnotations;

namespace Model.Entity;

public class WithdrawalRequest
{
    [Key] public int Id { get; set; }
    public DateTime Created { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
}