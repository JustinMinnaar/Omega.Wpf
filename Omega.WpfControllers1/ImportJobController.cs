using Bdo.DatabaseLibrary1;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        public string? RootFolderPath
        {
            get => Main.Settings.WorkingFolderPath;
            set { Main.Settings.WorkingFolderPath = value; }
        }

        protected override void DoWork()
        {
            var rootPath = RootFolderPath;
            if (rootPath == null) return;

            Task.Run(() => ProcessProject(rootPath));
        }

        private async Task ProcessProject(string rootPath)
        {
            var rootName = Path.GetFileName(rootPath);

            using var db = new BdoDocDbContext();

            var solution = await db.AccessSolutionAsync(rootName);
            var project = await db.AccessProjectAsync(new(solution.Id), rootName);
            await db.SaveChangesAsync();

            var folderPaths = Directory.GetDirectories(rootPath);
            var index = 0;
            foreach (var folderPath in folderPaths)
            {
                if (IsCancellationPending) break;

                await ProcessFolder(solution, project, folderPath);

                var progress = index * 100 / folderPaths.Length;
                ReportProgress(progress, folderPath);
            }
        }

        private async Task ProcessFolder(DocSolution solution, DocProject project, string folderPath)
        {
            using var db = new BdoDocDbContext();

            var folderName = Path.GetFileName(folderPath);
            var folder = await db.AccessFolderAsync(new(project.Id), folderName);

            var filePaths = Directory.GetFiles(folderPath, "*.pdf", SearchOption.TopDirectoryOnly);
            foreach (var filePath in filePaths)
            {
                if (IsCancellationPending) break;

                var fileName = Path.GetFileName(filePath);
                var file = await db.AccessFileAsync(new(folder.Id), fileName);
            }

            await db.SaveChangesAsync();
        }

        protected override void DoCompleted()
        {
            if (!JobCancelled)
                JobProgress = 100;

            JobStatus = null;
        }
    }
}