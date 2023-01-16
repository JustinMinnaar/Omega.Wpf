using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Omega.WpfModels1;
using Omega.WpfModels1.old;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Omega.WpfControllers1
{
    public class ImportJobController : AbstractBackgroundJobController
    {
        #region class

        [SetsRequiredMembers]
        public ImportJobController(MainController main)
        {
            this.Main = main;
        }

        public required MainController Main { get; init; }

        #endregion

        protected override void DoWork()
        {
            Task.Run(() => ProcessProjectAsync()).Wait();
        }

        private async Task ProcessProjectAsync()
        {
            try
            {
                this.JobResult = await ProcessProjectAndReportError();
                this.JobError = null;
            }
            catch (Exception ex)
            {
                this.JobResult = null;
                this.JobError = ex;
            }
        }

        private async Task<string?> ProcessProjectAndReportError()
        {
            await Main.Settings.SaveAsync();

            var workingFolder = Main.Settings.WorkingFolderPath;
            if (workingFolder == null || workingFolder == null) return "No working folder specified!";

            if (!Directory.Exists(workingFolder))
                Directory.CreateDirectory(workingFolder);

            var importFolder = Main.Settings.ImportFolderPath;
            var importName = Path.GetFileName(importFolder);
            if (importFolder == null || importName == null) return "No import folder specified!";

            var solution = Main.Explorer.SelectedSolution;
            if (solution == null) return "No solution selected!";

            var project = Main.Explorer.SelectedProject;
            if (project == null)
            {
                using var db = new BdoDocDbContext();
                var dbProject = await db.AccessProjectAsync(new(solution.Id), importName, importFolder);
                await db.SaveChangesAsync();
                project = ProjectModel.From(dbProject);
            }

            var excludeExtensions = ("" + Main.Settings.ImportExcludeExtensions).Split(';');

            var job = new ImportProjectJob
            {
                Solution = solution,
                Project = project,
                WorkingFolder = workingFolder,
                ProjectFolder = importFolder,
                ProjectName = importName,
                ExcludeExtensions = excludeExtensions,
                ImportFoldersInParallel = Main.Settings.ImportFoldersInParallel,
                ImportFilesInSubFolders = Main.Settings.ImportFilesInSubFolders,
                Progress = (int jobProgress, object? jobState) => { ReportProgress(jobProgress, jobState); return IsCancellationPending; }
            };

            await job.Execute();

            return null;


        }

        protected override void DoCompleted()
        {
            if (!JobCancelled)
                JobProgress = 100;

            JobStatus = null;
        }
    }
}