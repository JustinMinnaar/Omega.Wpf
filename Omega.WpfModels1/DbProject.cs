using System.Collections.Generic;

namespace Omega.WpfModels1
{
    public class DbProject
    {
        public List<DocFolder> Folders = new();

        public DbProject(string rootPath)
        {
            this.Name = System.IO.Path.GetFileName(rootPath);

            var folders = System.IO.Directory.GetDirectories(rootPath);
            foreach (var folderPath in folders)
            {
                var folderName = System.IO.Path.GetFileName(folderPath);
                var docFolder = new DocFolder { Name = folderName };
                Folders.Add(docFolder);

                var filePaths = System.IO.Directory.GetFiles(folderPath, "*.pdf", System.IO.SearchOption.TopDirectoryOnly);
                foreach (var filePath in filePaths)
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    var docFile = new DocFile { Name = fileName };
                    docFolder.Files.Add(docFile);
                }
            }
        }

        public string Name { get; }
    }
}
