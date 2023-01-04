namespace Jem.DatabaseLibrary1.Sets;

public sealed class IdNameSet<T> where T : IdNamed, new()
{
    protected readonly JemDbContext context;

    protected readonly DbSet<T> dbSet;

    public IdNameSet(JemDbContext context, DbSet<T> dbSet)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
    }

    public IQueryable<T> Query() => dbSet;

    public T Access(Guid id, string name)
    {
        T? row =
            TryGet(id) ??
            TryGet(name) ??
            Add(id, name);

        return row;
    }

    public T Add(string name) => Add(Guid.NewGuid(), name);

    public T Add(Guid id, string name)
    {
        var row = new T { Id = id, Name = name };
        dbSet.Add(row);
        return row;
    }

    public T? TryGet(Guid id)
    {
        var row = dbSet.Find(id);
        return row;
    }

    public T? TryGet(string name)
    {
        var row = dbSet.FirstOrDefault(p => p.Name == name);
        return row;
    }

    public void Drop(params T[] rows)
    {
        dbSet.RemoveRange(rows);
    }
}