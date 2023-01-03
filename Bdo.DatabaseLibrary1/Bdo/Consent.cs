public class Consent : BaseEntity
{
    [MaxLength(100)]
    public string? Number { get; set; }

    public DateTime? ConsentDate { get; set; }

    [MaxLength(1000)]
    public string? FolderName { get; set; }
    
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}