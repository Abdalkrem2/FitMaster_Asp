namespace FitMaster.Domain.Common;


public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; set; } = default!;// ! NULL FORGIVING OPERATOR
}


public abstract class BaseEntity : BaseEntity<long> // ITS LIKE shortcut 
{
}
