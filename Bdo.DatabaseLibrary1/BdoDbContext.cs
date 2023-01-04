using Jem.DatabaseLibrary1;

namespace Bdo.DatabaseLibrary1;

public sealed class BdoDbContext : JemDbContext
{
    public BdoDbContext() : base(@"Server=.;Database=bdo;integrated security=true;connection timeout=5;Trust Server Certificate=true;") { }
    // public BdoDbContext() : base(@"Server=.;Database=Omega;user id=omega;password=d8W2B5HG!Nq.2g8;connection timeout=5;Trust Server Certificate=true;") { }

    public BdoDbContext(string connectionString) : base(connectionString) { }

    public BdoDbContext(DbContextOptions options) : base(options) { }

    #region Banks

    public DbSet<Bank> BdoBanks { get; set; } = default!;
    public DbSet<BankCard> BdoBankCards { get; set; } = default!;
    public DbSet<BankStatement> BdoBankStatements { get; set; } = default!;
    public DbSet<BankStatementPage> BdoBankPages { get; set; } = default!;
    public DbSet<BankTransaction> BdoBankTransactions { get; set; } = default!;

    #endregion

    #region Employees

    public DbSet<Employee> BdoEmployees { get; set; } = default!;

    public Employee AddEmployee(string number, string name, DateTime? date, string folderName)
    {
        var root = new Employee { Number = number, Name = name, ConsentDate = date, FolderName = folderName };
        BdoEmployees.Add(root);
        return root;
    }

    #endregion

    #region Consents

    public DbSet<Consent> BdoConsents { get; set; } = default!;

    public Consent AddConsent(Employee employee, string number, string name, DateTime? date, string folderName)
    {
        var root = new Consent { Employee = employee, Number = number, Name = name, ConsentDate = date, FolderName = folderName };
        BdoConsents.Add(root);
        return root;
    }

    #endregion

    #region model

    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);

        ConfigBdoEmployees(m);
        ConfigBdoAccounts(m);
        ConfigBdoBanks(m);
        ConfigBdoBankStatements(m);
        ConfigBdoBankTransaction(m);
    }

    private static void ConfigBdoAccounts(ModelBuilder m)
    {
        m.Entity<BankAccount>()
            .HasMany<BankStatement>(account => account.Statements)
            .WithOne(statement => statement.Account)
            .IsRequired();
    }

    private static void ConfigBdoBanks(ModelBuilder m)
    {
        m.Entity<Bank>()
            .HasMany<BankAccount>(bank => bank.Accounts)
            .WithOne(statement => statement.Bank)
            .IsRequired();
    }

    private static void ConfigBdoBankStatements(ModelBuilder m)
    {
        var e = m.Entity<BankStatement>();
        e.Property(e => e.BalanceOpening).HasColumnType("Decimal(18,2)");
        e.Property(e => e.BalanceClosing).HasColumnType("Decimal(18,2)");
        e.Property(e => e.FundsReceived).HasColumnType("Decimal(18,2)");
        e.Property(e => e.FundsUsed).HasColumnType("Decimal(18,2)");
        e.Property(e => e.FeesCharged).HasColumnType("Decimal(18,2)");
    }

    private static void ConfigBdoBankTransaction(ModelBuilder m)
    {
        var e = m.Entity<BankTransaction>();
        e.Property(e => e.Balance).HasColumnType("Decimal(18,2)");
        e.Property(e => e.Credit).HasColumnType("Decimal(18,2)");
        e.Property(e => e.Debit).HasColumnType("Decimal(18,2)");
    }

    private static void ConfigBdoEmployees(ModelBuilder m)
    {
        m.Entity<Employee>()
            .HasMany<Consent>(employee => employee.Consents)
            .WithOne(consent => consent.Employee)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        m.Entity<Employee>()
            .HasMany<BankAccount>(employee => employee.Accounts)
            .WithOne(statement => statement.Employee)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion
}
