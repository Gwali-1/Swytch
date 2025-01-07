namespace Swytch.Structures;

public class ResponseStructureModel<T>
{
    public string Status { get; set; }
    public T Data { get; set; }

    public ResponseStructureModel(string status, T data)
    {
        Status = status;
        Data = data;
    }
}

public class InternalErrortResponseModel
{
    public string Status { get; set; }
    public string Message { get; set; }

    public InternalErrortResponseModel(string status, string message)
    {
        Status = status;
        Message = message;
    }
}