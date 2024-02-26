using System.ComponentModel.DataAnnotations;

namespace Model.Entity;

public class ServiceAvailableInDay
{
    [Key] public int  ServiceAvailableInDayId { get; set; }
    public DateTime Date { get; set; }
    public int NumberOfAvailableInDay { get; set; }
    
    public int ServiceId { get; set; }
    public virtual Service Service { get; set; }
}