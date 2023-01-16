using Bdo.DatabaseLibrary1;
using Bdo.DatabaseLibrary1.Migrations;

using Jem.CommonLibrary22;
using Jem.DocDatabaseLibrary1;

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
            Main.LastError = ReportDoWork();
        }

        private string? ReportDoWork()
        {
            var workingFolder = Main.Settings.WorkingFolderPath;
            if (workingFolder == null || workingFolder == null)
                return "No working folder specified!";

            if (!Directory.Exists(workingFolder))
                Directory.CreateDirectory(workingFolder);

            var importFolder = Main.Settings.ImportFolderPath;
            var importName = Path.GetFileName(importFolder);
            if (importFolder == null || importName == null)
                return "No import folder specified!";

            var solution = Main.Explorer.SelectedSolution;
            if (solution == null)
                return "No solution selected!";

            var project = Main.Explorer.SelectedProject;
            if (project == null)
            {
                using var db = new BdoDocDbContext();
                var dbProject = db.DocProjects.FirstOrDefault(p => p.OwnerSolutionId == solution.Id && p.Name == importName);
                if (dbProject == null)
                {
                    dbProject = new DocProject
                    {
                        OwnerSolutionId = solution.Id,
                        Name = importName,
                        WorkingFolderPath = Main.Settings.WorkingFolderPath,                        
                        
                        ImportFolderPath = Main.Settings.ImportFolderPath,
                        ImportExcludeWildcards = Main.Settings.ImportExcludeWildcards,
                        ImportFilesInSubFolders = Main.Settings.ImportFilesInSubFolders,
                    };
                    db.DocProjects.Add(dbProject);
                    db.SaveChanges();
                }
                project = ProjectModel.From(dbProject);
            }

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