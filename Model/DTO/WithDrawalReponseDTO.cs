namespace Model.DTO;

public class WithDrawalReponseDTO
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public UserDTO User { get; set; }
    public string Created { get; set; }
}