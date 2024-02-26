using System.ComponentModel.DataAnnotations;

namespace Model.Entity;

public class Facility
{
    [Key] 
    public int FacilityId { get; set; }
    public string Content { get; set; }

    public int  RoomId { get; set; }
    public virtual Room Room { get; set; }
}