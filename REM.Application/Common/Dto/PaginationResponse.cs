namespace REM.Application.Common.Dto;

public class PaginationResponse<T>
{
    public int Count { get; set; }
    public IEnumerable<T> Data { get; set; } = [];

    public PaginationResponse(List<T> data, int count)
    {
        Data = data;
        Count = count;
    }
}
