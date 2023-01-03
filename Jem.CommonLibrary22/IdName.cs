using System.ComponentModel.DataAnnotations;

namespace Jem.CommonLibrary22;

public struct ID<T> : IID
{
    public override string ToString() => Guid.ToString();

    public ID() { Guid = Guid.NewGuid(); }
    public ID(Guid guid) { Guid = guid; }
    public ID(string guid) { Guid = new Guid(guid); }
    public Guid Guid { get; set; }

    public static implicit operator Guid(ID<T> v) => v.Guid;
}


public class IdNamed : Named, IId, IName
{
    public IdNamed() { }

    public IdNamed(string name):base(name) {}
    public IdNamed(Guid id, string name) : base(name) { Id = id; }

    public override string ToString() => Name;

    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}

public class Named : CNotifyPropertyChanged, IName
{
    public Named() { }

    public Named(string name) { Name = name; }

    public override string ToString() => Name;

    [Required]
    [MaxLength(1000)]
    public string Name { get; set; } = String.Empty;
}