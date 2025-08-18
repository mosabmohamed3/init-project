namespace REM.Domain.Dto;

public class Pagination
{
    private int _pageNumber;
    private int _pageSize;
    public int PageNumber
    {
        get => _pageNumber == 0 ? 1 : _pageNumber;
        set => _pageNumber = value;
    }
    public int PageSize
    {
        get => _pageSize == 0 ? 10 : _pageSize;
        set => _pageSize = value;
    }
}
