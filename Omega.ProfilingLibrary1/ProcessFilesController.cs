using Jem.DocDatabaseLibrary1;

namespace Omega.ProfilingLibrary1;

public class ProcessFilesController : CNotifyPropertyChanged
{
    public required string WorkingFolder { get; set; }

    /// <summary>Should folders be processed in parallel?</summary>
    public bool ProcessFoldersInParallel { get; set; }

    public async Task ProcessFoldersInParallelAsync()
    {
        using var db = new BdoDocDbContext();
        var foldersToProcess = await
            (from f in db.DocFolders
             where !f.AreAllFilesIdentified
             orderby f.Name
             select f
             )
             .ToListAsync();

        if (ProcessFoldersInParallel)
            await Parallel.ForEachAsync(foldersToProcess,
                async (folder, _) => await ProcessFolderAsync(folder));
        else
            foreach (var folder in foldersToProcess) await ProcessFolderAsync(folder);
    }

    private async Task ProcessFolderAsync(DocFolder folder)
    {
        // Create a new context for each parallel thread.
        using var db = new BdoDocDbContext();

        var filesToProcess = await
            (from f in db.DocFiles
             where f.OwnerFolderId == folder.Id && !f.IsIdentified
             orderby f.Name
             select f
             )
             .AsNoTracking()
             .ToListAsync();

        foreach (var file in filesToProcess)
        {
            ProcessFileAsync(folder, file);
        }

        var areAllFilesIdentified = await db.DocFolders.AllAsync(f => f.AreAllFilesIdentified);
        if (!areAllFilesIdentified) return;

        folder.AreAllFilesIdentified = true;
        await db.SaveChangesAsync();
    }

    private void ProcessFileAsync(DocFolder folder, DocFile file)
    {        
        var ocrFilePath = $"{WorkingFolder}\\{folder.Name}\\{file.Name}.bocr";
        var oDocument = OcrDocument.TryLoadFromJsonFileAsync(ocrFilePath);
    }
}