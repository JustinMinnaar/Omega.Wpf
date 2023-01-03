public class BankTransaction
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public int StatementId { get; set; }
    public int PageNumber { get; set; }
    public int RowNumber { get; set; }

    public DateTime? Date { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Balance { get; set; }
}
