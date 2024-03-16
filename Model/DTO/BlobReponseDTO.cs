namespace Model.DTO;

public class BlobReponseDTO
{
    public BlobReponseDTO()
    {
        Blob = new BlobDTO();
    }

    public BlobDTO Blob { get; set; }
    public string? Status { get; set; }
    public bool Error { get; set; }
}