
public abstract class BaseEntity : IdNamed
{
    public override string ToString() => Name;

    public bool IsDeleted { get; set; }
}
