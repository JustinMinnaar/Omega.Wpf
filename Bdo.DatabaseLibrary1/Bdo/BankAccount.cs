public class BankAccount : BaseEntity
{
    public Guid? BankId { get; set; }
    public Bank? Bank { get; set; }

    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [MaxLength(100)] public string? AccountName { get; set; }
    [MaxLength(50)] public string? AccountReference { get; set; }
    [MaxLength(50)] public string? AccountType { get; set; }
    [MaxLength(25)] public string? AccountNumber { get; set; }

    public virtual ICollection<BankCard>? Cards { get; set; }

    public virtual ICollection<BankStatement>? Statements { get; set; }
}
