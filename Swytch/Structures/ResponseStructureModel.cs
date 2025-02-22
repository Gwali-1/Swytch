namespace Swytch.Structures;


/// <summary>
/// Response structure of responses when using any of the RequestContext response extension methods.
/// </summary>
/// <typeparam name="T"></typeparam>
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
/// Response structure of error response when using any of the RequestContext response extension methods.

public class InternalErrorResponseModel
{
    public string Status { get; set; }
    public string Message { get; set; }

    public InternalErrorResponseModel(string status, string message)
    {
        Status = status;
        Message = message;
    }
}