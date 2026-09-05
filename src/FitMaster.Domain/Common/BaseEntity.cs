namespace FitMaster.Domain.Common;


public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; set; } = default!;
}


public abstract class BaseEntity : BaseEntity<long>
{
}
