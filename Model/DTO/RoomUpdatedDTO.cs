namespace Model.DTO;

public class RoomUpdatedDTO
{
    public string RoomName { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public string Address { get; set; }
    public float Area { get; set; }
    public decimal Price { get; set; }
    public int UserId { get; set; }
    public ICollection<ImageUpdateDTO> Images { get; set; }
}