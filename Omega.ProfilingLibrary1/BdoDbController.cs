using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Microsoft.EntityFrameworkCore;

namespace Omega.ProfilingLibrary1;

public class BdoDbController : CNotifyPropertyChanged
{
    public async Task EnsureDatabaseCreatedAsync()
    {
        using var db = new BdoDocDbContext();
        await db.Database.EnsureCreatedAsync();

        var user = db.AccessSysUserSettings(Environment.MachineName);
        await db.SaveChangesAsync();
    }

    //    /// <summary>Adds a new import job request to the database, so that a machine can pick it up and begin working.</summary>
    //    /// <param name="solutionName">The solution to create or work with, typically the name of the client.</param>
    //    /// <param name="rootFolderPath">The folder on the local machine to be imported from. It should contain folders and files.</param>
    //    public async Task AddImportJob(string solutionName, string rootFolderPath)
    //    {
    //        var job = new JobImportFiles
    //        {
    //            SolutionName = solutionName,
    //            RootFolderPath = rootFolderPath
    //        };
    //        using var db = new BdoDocDbContext();
    //        db.JobsImportFiles.Add(job);
    //        await db.SaveChangesAsync();
    //    }

    //    public async Task ResetJobsForFailedProcesses()
    //    {
    //        using var db = new BdoDocDbContext();

    //        // Mark all jobs currently being processed as new
    //        var jobs = from j in db.JobsImportFiles
    //                   where j.Status == EJobStatus.InProgress
    //                   orderby j.Name
    //                   select j;
    //        foreach (var job in jobs)
    //        {
    //            job.Status = EJobStatus.New;
    //        }
    //        await db.SaveChangesAsync();
    //    }

    //    /// <summary>Marks the job as being processed by this machine and sets the start date.</summary>
    //    /// <returns>The ID of the job.</returns>
    //    public async Task<Guid?> TryReserveNextImportFileJob()
    //    {
    //        try { return await ReserveNextImportFileJob(); }
    //        catch { return null; }
    //    }

    //    public async Task<Guid?> ReserveNextImportFileJob()
    //    {
    //        using var db = new BdoDocDbContext();

    //        // Is there a job waiting?
    //        var job = await db.JobsImportFiles.FirstOrDefaultAsync(
    //            j => j.Status == EJobStatus.New);
    //        if (job == null) return null;

    //        // Mark the job as in progress
    //        job.Status = EJobStatus.InProgress;
    //        job.MachineName = Environment.MachineName;
    //        await db.SaveChangesAsync();

    //        // Return the job
    //        return job.Id;
    //    }
}
