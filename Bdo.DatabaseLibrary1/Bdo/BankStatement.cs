using Jem.DocDatabaseLibrary1;

namespace Bdo.DatabaseLibrary1;


public class BankStatement : BaseEntity
{
    public Guid? AccountId { get; set; } 
    public BankAccount? Account { get; set; }

    [MaxLength(1000)] public string? SourceFileName { get; set; }
    public int SourcePageCount { get; set; }
    public virtual ICollection<BankStatementPage>? SourcePages { get; set; }

    public int? StatementNumber { get; set; }
    public DateTime? StatementDate { get; set; }
    [MaxLength(150)] public string? StatementDateText { get; set; }
    [MaxLength(150)] public string? StatementPeriod { get; set; }
    public int StatementFrequency { get; set; }

    public decimal? BalanceOpening { get; set; }
    public decimal? BalanceClosing { get; set; }
    public decimal FundsUsed { get; set; }
    public decimal FundsReceived { get; set; }
    public decimal FeesCharged { get; set; }

    [MaxLength(150)] public string? Address1 { get; set; }
    [MaxLength(150)] public string? Address2 { get; set; }
    [MaxLength(150)] public string? Address3 { get; set; }
    [MaxLength(150)] public string? Address4 { get; set; }
    [MaxLength(150)] public string? Address5 { get; set; }
    [MaxLength(150)] public string? Address6 { get; set; }

    [MaxLength(25)] public string? VatNumber { get; set; }
}
