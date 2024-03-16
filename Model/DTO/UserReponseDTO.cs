using Model.Entity;

namespace Model.DTO;

public class UserReponseDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Status { get; set; }

    public RoleResponseDTO Role { get; set; }
}