public class Bank : BaseEntity
{
    public virtual ICollection<BankAccount> Accounts { get; set; } = default!;
}