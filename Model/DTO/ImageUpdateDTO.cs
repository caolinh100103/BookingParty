using Microsoft.AspNetCore.Http;

namespace Model.DTO;

public class ImageUpdateDTO
{
    public int ImageId { get; set; }
    public IFormFile Image { get; set; }
    
    public int? ServiceId { get; set; }
    public int? RoomId { get; set; }
}