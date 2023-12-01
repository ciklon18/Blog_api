namespace BlogAPI.Models.Response;

public record PageInfoResponse(int Size, int Count, int Current)
{
    public PageInfoResponse() : this(0, 0, 0)
    {
    }
}