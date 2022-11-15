namespace Core.Dtos;

public class BaseQueryInfoDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public string State { get; set; }
    public string Begin { get; set; }
    public string End { get; set; }

    public int Skip()
    {
        return (PageNum - 1) * PageSize;
    }
}