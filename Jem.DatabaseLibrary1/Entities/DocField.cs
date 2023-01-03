public sealed class DocField : IdNamed
{
    public ID<DocField> GetId() => new(this.Id);
}
