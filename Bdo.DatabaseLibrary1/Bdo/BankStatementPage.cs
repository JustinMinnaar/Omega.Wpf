﻿public class BankStatementPage
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? StatementId { get; set; }
    public Guid? PageId { get; set; }
}
