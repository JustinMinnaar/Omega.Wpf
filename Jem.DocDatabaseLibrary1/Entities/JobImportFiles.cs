namespace Jem.DocDatabaseLibrary1;

// The Id and Name define the job to be run
public sealed class JobImportFiles : BaseEntity
{
    public EJobStatus Status { get; set; }

    /// <summary></summary>
    public required string SolutionName { get; set; }

    /// <summary></summary>
    public required string RootFolderPath { get; set; }

    /// <summary>The machine that last worked on this job.</summary>
    public string? MachineName { get; set; }

    /// <summary>When the machine started on this job.</summary>
    public DateTime? MachineStartTime { get; set; }
}

public enum EJobStatus
{
    New,
    InProgress,
    Failed,
    Completed,
}