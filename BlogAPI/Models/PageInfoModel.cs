namespace BlogAPI.Models;

public record PageInfoModel(int Size, int Count, int Current)
{
    public PageInfoModel() : this(0, 0, 0)
    {
    }
}