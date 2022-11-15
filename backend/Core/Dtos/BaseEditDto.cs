using System.Collections.Generic;

namespace Core.Dtos;

public class BaseEditDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Remark { get; set; }
    public string ProductOrderNo { get; set; }
    public List<int> Ids { get; set; }
    public bool State { get; set; }
}