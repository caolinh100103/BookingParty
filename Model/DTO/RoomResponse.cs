namespace Model.DTO;

public class RoomResponse
{
    public int RoomId{ get; set; }
    public string RoomName { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public string Address { get; set; }
    public int Status { get; set; }
    public string imgPath { get; set; }
    public decimal Price { get; set; }
    public decimal SalePrice {get; set; }
}