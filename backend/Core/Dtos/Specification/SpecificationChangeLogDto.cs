namespace Core.Dtos.Specification;

public class SpecificationChangeLogDto
{
    public string ChangeTime { get; set; }
    public Core.Entities.Specification Before { get; set; }
    public string BeforeType { get; set; }
    public string BeforeEquipment { get; set; }
    public Core.Entities.Specification After { get; set; }
    public string AfterType { get; set; }
    public string AfterEquipment { get; set; }
    public string ChangeBy { get; set; }

    public SpecificationChangeLogDto()
    {
        
    }
    public SpecificationChangeLogDto(string time, Core.Entities.Specification before, Core.Entities.Specification after)
    {
        ChangeTime = time;
        Before = before;
        After = after;
    }
}
