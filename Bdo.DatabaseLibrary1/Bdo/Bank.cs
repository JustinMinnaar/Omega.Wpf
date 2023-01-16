using Jem.DocDatabaseLibrary1;

namespace Bdo.DatabaseLibrary1;

public class Bank : BaseEntity
{
    public virtual ICollection<BankAccount> Accounts { get; set; } = default!;
}