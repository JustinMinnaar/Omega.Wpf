﻿using System.Collections.Generic;

namespace Omega.WpfModels1
{
    public class DocFolder
    {
        public required string Name;
        public List<DocFile> Files = new();
    }
}
