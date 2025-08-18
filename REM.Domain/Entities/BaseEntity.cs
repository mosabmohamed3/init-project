namespace REM.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime Created_At { get; set; }
    public Guid Created_By { get; set; }
    public DateTime? Modified_At { get; set; }
    public Guid? Modified_By { get; set; }
    public bool Is_Deleted { get; set; }
}
