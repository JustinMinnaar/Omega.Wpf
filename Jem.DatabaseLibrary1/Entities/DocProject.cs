﻿public sealed class DocProject : IdNamed
{
    public Guid? OwnerRootId { get; set; }
    public DocRoot? OwnerRoot { get; set; }

    [MaxLength(200)]
    public string? RootFolderPath { get; set; } = @"F:\BDO\E-Lifestyle Uploads";

    public bool IsLoadFoldersChecked { get; set; } = true;

    public int? FoldersCount { get; set; }
    public int? DocumentsCount { get; set; }
    public int? PagesCount { get; set; }
    public int? ImagesCount { get; set; }

    public ICollection<DocFolder> Folders { get; set; } = default!;

}