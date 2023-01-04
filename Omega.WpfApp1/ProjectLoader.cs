﻿using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Microsoft.EntityFrameworkCore;

using Omega.WpfModels1;
using Omega.WpfModels1.old;

using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Omega.WpfApp1
{
    public class ProjectLoaderModel : CNotifyPropertyChanged
    {
        public bool IsParallel { get; set; }

        private ICommand? _ImportProjectCommand;

        public ICommand ImportProjectCommand => _ImportProjectCommand ??= new RelayCommandAsync(ImportProjectAsync);

        public int ProgressPercentage { get; set; }

        public string? ProgressMessage { get; set; }

        public void ReportProgress(int percentage, string message)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                ProgressPercentage = percentage;
                ProgressMessage = message;
            });
        }

        public string SolutionName { get; set; } = "Transnet";

        public string ProjectPath { get; set; } = @"F:\BDO\E-Lifestyle Uploads";

        public ProjectModel? ImportedProject { get; set; }

        public async Task ImportProjectAsync()
        {
            var solutionName = this.SolutionName;
            var projectPath = this.ProjectPath;

            ImportedProject = await ImportProjectAsync(solutionName, projectPath);
        }

        public async Task<ProjectModel> ImportProjectAsync(string solutionName, string importPath)
        {
            using var db = new BdoDbContext();

            var solution = await db.AccessSolutionAsync(solutionName);
            return await ImportProjectAsync(solution.Id, importPath);
        }

        public async Task<ProjectModel> ImportProjectAsync(Guid solutionId, string importPath)
        {
            using var db = new BdoDbContext();

            var projectName = System.IO.Path.GetFileName(importPath);

            var dbProject = await db.AccessProjectAsync(new(solutionId), projectName);
            ReportProgress(0, $"Importing project '{projectName}'");

            var folderPaths = System.IO.Directory.GetDirectories(importPath);
            var totals = new JobTotals { CountJobs = folderPaths.Length };

            if (IsParallel)
            {
                var tasks = folderPaths.Select(folderPath => ProcessFolderAsync(folderPath, new(dbProject.Id), totals));
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var folderPath in folderPaths) await ProcessFolderAsync(folderPath, new(dbProject.Id), totals);
            }

            ReportProgress(100, $"Imported {totals.CountJobs} folders.");

            return ProjectModel.From(dbProject);
        }

        private async Task ProcessFolderAsync(string folderPath, ID<DocProject> projectId, JobTotals totals)
        {
            // For parallel processing, we need a separate context
            using var db = new BdoDbContext();

            // Create the folder if missing
            var folderName = System.IO.Path.GetFileName(folderPath);
            var dbFolder = await db.AccessFolderAsync(projectId, folderName);

            if (!dbFolder.AreFilesLoaded)
            {
                var filePaths = System.IO.Directory.GetFiles(folderPath, "*.pdf", System.IO.SearchOption.TopDirectoryOnly);
                foreach (var filePath in filePaths)
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    var dbFile = await db.AccessFileAsync(new(dbFolder.Id), fileName);

                    var fi = new FileInfo(filePath);
                    dbFile.CreatedDate = fi.CreationTime;
                    dbFile.ModifiedDate = fi.LastWriteTime;
                    dbFile.Size = fi.Length;
                    await db.SaveChangesAsync();
                }

                dbFolder.AreFilesLoaded = true;
                await db.SaveChangesAsync();
            }

            totals.IncrementIndexJobs();

            var progressPercentage = totals.IndexJobs * 100 / totals.CountJobs;
            ReportProgress(progressPercentage, totals.ToString());

        }


        //private async Task<DocPage> AccessFilePage(BdoDbContext db, Guid fileId, JPage page)
        //{
        //    var dbPage = await db.DocPages.FirstOrDefaultAsync(p => p.OwnerFileId == fileId && p.PageIndex == page.PageIndex);
        //    if (dbPage == null)
        //    {
        //        dbPage = new DocPage
        //        {
        //            OwnerFileId = fileId,
        //            PageIndex = page.PageIndex,
        //            IsBlank = page.IsBlank,
        //            ProfileId = page.ProfileId,
        //            ProfileName = page.ProfileName,
        //            ProfileVersion = page.ProfileVersion,
        //            IsError = page.IsError,
        //        };
        //        db.DocPages.Add(dbPage);
        //        //await db.SaveChangesAsync();
        //    }

        //    return dbPage;
        //}

    }
}