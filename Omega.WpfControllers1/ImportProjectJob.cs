using Bdo.DatabaseLibrary1;

using Omega.WpfModels1;
using Omega.WpfModels1.old;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omega.WpfControllers1;

internal class ImportProjectJob
{
    public bool IsCancellationPending { get; set; }

    public required SolutionModel Solution { get; init; }
    public required ProjectModel Project { get; init; }
    public required string WorkingFolder { get; init; }
    public required string ProjectFolder { get; init; }
    public required string ProjectName { get; init; }
    public required string[] ExcludeExtensions { get; init; }
    public required bool ImportFoldersInParallel { get; init; }
    public required bool ImportFilesInSubFolders { get; init; }
    public required Func<int, object?, bool> Progress { get; init; }

    // private void ReportProgress(int progress, object? value) => Progress(progress, value);
    public async Task Execute()
    {
        IsCancellationPending = Progress(0, $"Importing folder '{ProjectFolder}'...");

        var folderPaths = Directory.GetDirectories(ProjectFolder);

        var totals = new JobTotals { CountJobs = folderPaths.Length };
        if (ImportFoldersInParallel)
        {
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            await Parallel.ForEachAsync(folderPaths, parallelOptions, async (folderPath, ct) => await ProcessFolderAsync(folderPath));

            //var tasks = folderPaths.Select(folderPath => ProcessFolderAsync(folderPath));
            //await Task.WhenAll(tasks);
        }
        else
        {
            foreach (var folderPath in folderPaths)
                await ProcessFolderAsync(folderPath);
        }

        Progress(100, $"Imported {folderPaths} folders.");

        async Task ProcessFolderAsync(string folderPath)
        {
            if (IsCancellationPending) return;

            // For parallel processing, we need a separate context
            using var db = new BdoDocDbContext();

            // Create the database entry for the folder (if missing)
            var folderName = Path.GetFileName(folderPath);
            var dbFolder = await db.AccessFolderAsync(new(Project.Id), folderName);

            // Scan files if not previously completed
            await ProcessFilesAsync(folderPath);

            // Report progress
            totals.IncrementIndexJobs();
            var progressPercentage = totals.Percentage;
            IsCancellationPending = Progress(progressPercentage, totals.ToString());

            async Task ProcessFilesAsync(string folderPath)
            {
                if (dbFolder.AreFilesLoaded) return;

                // Find all files in the folder matching the include specifications
                var filePaths = Directory.GetFiles(folderPath, "*.pdf", SearchOption.TopDirectoryOnly);
                foreach (var filePath in filePaths)
                {
                    if (IsCancellationPending) break;

                    var ext = Path.GetExtension(filePath);
                    if (ExcludeExtensions.Contains(ext)) continue;

                    var fileName = Path.GetFileName(filePath);
                    var dbFile = await db.AccessFileAsync(new(dbFolder.Id), fileName);

                    var fi = new FileInfo(filePath);
                    dbFile.CreatedDate = fi.CreationTime;
                    dbFile.ModifiedDate = fi.LastWriteTime;
                    dbFile.Size = fi.Length;
                }

                // All files are loaded, or nothing is
                dbFolder.AreFilesLoaded = true;
                await db.SaveChangesAsync();
            }
        }
    }
}