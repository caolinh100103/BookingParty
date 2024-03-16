namespace Model.DTO;

public class UserDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
}