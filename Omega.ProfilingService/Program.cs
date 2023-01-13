
using Omega.ProfilingLibrary1;

Console.WriteLine("Connecting...");

// await new BdoDbController().EnsureDatabaseCreatedAsync();

//var solutionName = "Samples";
var rootFolderPath = @"C:\DevExec\Bdo\Bdo.Samples\Samples";

//await new BdoDbController().AddImportJob(solutionName, rootFolderPath);

var pf = new ProcessFilesController { WorkingFolder = rootFolderPath };
await pf.ProcessFoldersInParallelAsync();