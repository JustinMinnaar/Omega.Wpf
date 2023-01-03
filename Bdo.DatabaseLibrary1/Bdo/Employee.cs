public class Employee : BaseEntity
{
    [MaxLength(100)]
    public string? Number { get; set; }

    public DateTime? ConsentDate { get; set; }

    [MaxLength(1000)]
    public string? FolderName { get; set; }

    [MaxLength(1000)]
    public string? NumberSource { get; set; }

    [MaxLength(1000)]
    public string? NameSource { get; set; }

    [MaxLength(1000)]
    public string? DateSource { get; set; }

    public virtual ICollection<Consent>? Consents { get; set; }

    public virtual ICollection<BankAccount>? Accounts { get; set; }
}