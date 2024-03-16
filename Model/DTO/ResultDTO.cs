namespace Model.DTO;

public class ResultDTO<T>
{
    public T? Data { get; set; }
    public bool isSuccess { get; set; }
    public string Message { get; set; }
}